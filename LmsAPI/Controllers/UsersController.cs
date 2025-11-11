using FluentValidation;
using LMSAPI.DTO;
using LMSAPI.Helpers;
using LMSAPI.Models;
using LMSAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Claims;
using System.Text;

namespace LmsAPI.Controllers
{
    [ApiController]
    [TypeFilter(typeof(ExceptionFilter))]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class UsersController : ControllerBase
    {
        private readonly IStudentsRepository _studentsRepository;
        private readonly ILoggerManager _logger;
        private readonly IValidator<string> _emailValidator;
        private readonly IValidator<StudentRegisterDto> _validator;
        private readonly EmailService _emailService;
        private readonly JwtTokenService _jwtTokenService;
        private readonly IDistributedCache _cache;
        private readonly LmsdbNewContext _context;
        public UsersController(IStudentsRepository studentsRepository, ILoggerManager logger, IValidator<string> emailValidator,
            IValidator<StudentRegisterDto> validator, EmailService emailService, JwtTokenService jwtTokenService, IDistributedCache cache, LmsdbNewContext context)
        {
            _studentsRepository = studentsRepository;
            _logger = logger;
            _emailValidator = emailValidator;
            _validator = validator;
            _emailService = emailService;
            _jwtTokenService = jwtTokenService;
            _cache = cache;
            _context = context;
        }

        #region student login
        [HttpGet("Student-login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetStudentInfo(string email)
        {
            try
            {
                // Validate the email format
                var validationResult = await _emailValidator.ValidateAsync(email);
                if (!validationResult.IsValid)
                {
                    return Ok(new { Message = "Invalid email format.", Errors = validationResult.Errors.Select(e => e.ErrorMessage) });
                }

                var student = await _studentsRepository.GetStudentByEmailAsync(email);
                if (student == null)
                {
                    return NotFound(new { Message = "Student not found." });
                }
                else
                {
                    var token = _jwtTokenService.GenerateToken(student.EmailId, student.StudentUserId.ToString(), student.Username);
                    string base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
                    student.Token = base64Encoded;
                    await _studentsRepository.UpdateStudentAsync(student);
                    _logger.LogInfo($"Student found: {student.EmailId}, Status: {student.ActiveStatus}");
                    SetStudentSession(student);

                    return Ok(new ApiResponse
                    {
                        Success = true,
                        Message = "Email found. Please verify your email.",
                        Data = new
                        { student.StudentUserId, student.Username, student.EmailId, student.Mobile, student.ActiveStatus, Token = base64Encoded }
                    });
                }
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here as needed
                return StatusCode(500, new { Message = "An error occurred while processing your request.", Details = ex.Message });
            }
        }
        #endregion

        #region student register
        [HttpPost("RegisterStudent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterStudent([FromBody] StudentRegisterDto studentDto)
        {
            var validationResult = await _validator.ValidateAsync(studentDto);
            if (!validationResult.IsValid)
                return Ok(new ApiResponse { Success = false, Message = string.Join(",", validationResult.Errors.Select(e => e.ErrorMessage)), ErrorCode = "400" });

            var existing = await _studentsRepository.GetStudentByEmailAsync(studentDto.EmailId);

            if (existing != null) return Ok(new ApiResponse { Success = false, Message = "Email already registered", ErrorCode = "400" });
            else
            {
                var student = new TblStudentUserMaster
                {
                    Username = studentDto.Username,
                    UserFirstName = studentDto.Username,
                    Mobile = studentDto.Mobile,
                    EmailId = studentDto.EmailId,
                    PrimaryMac = studentDto.DeviceMacId,
                    CountryCode = studentDto.CountryCode,
                    CreatedOn = DateTime.Now,
                    ActiveStatus = 0, // inactive until email verification
                };
                //user info in session
                SetStudentSession(student);

                await _studentsRepository.AddStudentAsync(student);
                //var otp = new Random().Next(100000, 999999).ToString();
                // Generate OTP
                var otp = GenOPT(6);
                var otpRecord = new TblUserRandomPass
                {
                    UserRandomId = 0,
                    UserId = (int)student.StudentUserId,
                    VerificationCode = otp,
                    GeneratedTime = DateTime.Now,
                    ActionType = 1, // 1 for registration
                    UserType = 2, // 2 for student
                };
                await _studentsRepository.SaveOtpAsync(otpRecord);
                //send email
                await _emailService.SendEmailAsync(student.EmailId, "Your OTP Code", $"Your OTP code is: {otp}");
                // TODO: send OTP via email (using a mail service)
                return Ok(new ApiResponse { Success = true, Message = "OTP sent to your email", Data = otp }); // return OTP only for testing
            }
        }

        #endregion

        #region Validate OTP
        [HttpPost("ValidateOtp")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ValidateOtp([FromBody] OtpValidationDto otpDto)
        {
            const string errorCode = "400";

            // 1️⃣ Validate input
            if (otpDto == null || string.IsNullOrEmpty(otpDto.EmailId) || string.IsNullOrEmpty(otpDto.Otp))
                return Ok(new ApiResponse(false, "Email and OTP are required.", null, errorCode));

            var emailValidation = await _emailValidator.ValidateAsync(otpDto.EmailId);
            if (!emailValidation.IsValid)
                return Ok(new ApiResponse(false, string.Join(", ", emailValidation.Errors.Select(e => e.ErrorMessage)), null, errorCode));

            // 2️⃣ Get student
            var student = await _studentsRepository.GetStudentByEmailAsync(otpDto.EmailId);
            if (student == null)
                return Ok(new ApiResponse(false, "Student not found.", null, errorCode));

            // 3️⃣ Get latest OTP
            var otpRecord = await _studentsRepository.GetLatestOtpAsync((int)student.StudentUserId, 1, 2);
            if (otpRecord == null || otpRecord.VerificationCode != otpDto.Otp)
                return Ok(new ApiResponse(false, "Enter valid OTP", null, errorCode));

            if (otpRecord.GeneratedTime.AddMinutes(10) < DateTime.Now)
                return Ok(new ApiResponse(false, "OTP has expired", null, errorCode));

            // 4️⃣ Activate student
            student.ActiveStatus = 1;
            student.Istrail = true;
            student.AccActiveOn = DateTime.Now;

            var token = _jwtTokenService.GenerateToken(student.EmailId, student.StudentUserId.ToString(), student.Username);
            student.Token = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));

            await _studentsRepository.UpdateStudentAsync(student);

            // 5️⃣ Optionally delete OTP
            await _studentsRepository.DeleteOtpAsync(otpRecord.UserId);

            return Ok(new ApiResponse(true, "Account activated successfully.", student));
        }

        #endregion

        #region Validate trail periode from activated date
        [HttpGet("check-trial-period")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> IStudentInTrialPeriod()
        {
            try
            {
                var userEmail = HttpContext.Session.GetString("UserEmail");
                if (string.IsNullOrEmpty(userEmail))
                {
                    return Unauthorized(new ApiResponse(false, "User not logged in.", null, StatusCodes.Status401Unauthorized.ToString()));
                }

                var student = await _studentsRepository.GetStudentByEmailAsync(userEmail);
                if (student == null)
                {
                    return Ok(new ApiResponse(false, "Student not found.", null, StatusCodes.Status404NotFound.ToString()));
                }

                var trialDays = await _studentsRepository.GetTrialPeriodDaysAsync();
                if (trialDays <= 0)
                {
                    return Ok(new ApiResponse(false, "Trial period not configured.", null, StatusCodes.Status400BadRequest.ToString()));
                }

                if (student.AccActiveOn == null)
                {
                    return Ok(new ApiResponse(false, "Account not activated.", null, StatusCodes.Status400BadRequest.ToString()));
                }

                var activationDate = student.AccActiveOn.Value;
                var currentDate = DateTime.Now;
                var difference = currentDate - activationDate;
                int daysSinceActivation = (int)difference.TotalDays;
                int daysLeft = trialDays - daysSinceActivation;

                if (daysLeft > 0)
                {
                    return Ok(new ApiResponse(true, "Student is in trial period.", new { InTrialPeriod = true, DaysLeft = daysLeft }, StatusCodes.Status200OK.ToString()));
                }
                else
                {
                    return Ok(new ApiResponse(true, "Student trial period has ended.", new
                    {
                        InTrialPeriod = false,
                        DaysLeft = 0
                    },
                        StatusCodes.Status200OK.ToString()
                    ));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in IStudentInTrialPeriod: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, "An error occurred while checking trial period.", null, StatusCodes.Status500InternalServerError.ToString()));
            }
        }
        #endregion

        #region GenOPT
        private string GenOPT(int otplength)
        {
            string allowedChars = "123456789";
            char[] chars = new char[otplength];
            Random rd = new Random();

            for (int i = 0; i < otplength; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }
            return new string(chars);
        }
        #endregion

        //Sessions
        private void SetStudentSession(TblStudentUserMaster student)
        {
            HttpContext.Session.SetString("UserEmail", student.EmailId);
            HttpContext.Session.SetInt32("UserId", (int)student.StudentUserId);
            HttpContext.Session.SetString("UserName", student.Username ?? "");
            HttpContext.Session.SetInt32("UserStatus", (int)student.ActiveStatus);
            HttpContext.Session.SetInt32("trade_id", student.TradeId ?? 0);
        }

        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token))
                return Ok("No token found.");

            var email = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var student = await _studentsRepository.GetStudentByEmailAsync(email);
            student.Token = "";
            await _studentsRepository.UpdateStudentAsync(student);

            await _cache.RemoveAsync("AllSubjects");
            await _cache.RemoveAsync("DashboardUniqueSubjects");

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Logged out successfully.",
                Data = null
            });
        }

        [Authorize]
        [HttpGet("GetStudent-byemail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByEmail([FromQuery] string email)
        {
            try
            {
                var user = await _studentsRepository.GetStudentByEmailAsync(email);
                if (user == null)
                    return NotFound(new { success = false, message = "User not found." });

                return Ok(new { success = true, data = user });
            }
            catch (ArgumentException ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("Setting-UserInfo-Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUserFields([FromBody] TblStudentUserMaster request)
        {
            var result = await _studentsRepository.UpdateStudentAsync(request);
            return Ok(new ApiResponse { Success = true, Message = "User fields updated successfully.", Data = result });
        }

        [Authorize]
        [HttpPost("TicketCreate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> TicketCreate([FromBody] TblSupportTicket request)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(request.Subject))
                errors.Add("Subject is required.");
            else if (string.IsNullOrWhiteSpace(request.Message))
                errors.Add("Message is required.");

            if (errors.Any())
                return Ok(new ApiResponse { Success = false, Message = string.Join(",", errors), ErrorCode = "400" });

            var email = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (email == null)
                return NotFound(new { success = false, message = "User not found." });
            request.EmailId = email;
            request.Createdon = DateTime.Now;
            request.ActiveStatus = true;
            request.ReadBy = Convert.ToInt32(userId);
            await _studentsRepository.TicketCreateAsync(request);
            return Ok(new { success = true, message = "Ticket created", data = request });

        }

        [Authorize]
        [HttpGet("GetTickets")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetActiveTickets()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var activeTickets = await _studentsRepository.GetTicketByIdAsync(Convert.ToInt32(userId));
            return Ok(new ApiResponse(true, "Ticket fetched successfully", activeTickets));
        }

        //04/11/2025

        #region re-generate OPT
        [HttpPost("RegenerateOtp")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegenerateOtp([FromBody] OtpRegenerateDto otpDto)
        {
            if (otpDto == null || string.IsNullOrEmpty(otpDto.EmailId))
                return Ok(new ApiResponse(false, "Email is required.", null, "400"));

            var emailValidation = await _emailValidator.ValidateAsync(otpDto.EmailId);
            if (!emailValidation.IsValid)
                return Ok(new ApiResponse(false, "Invalid email format.", emailValidation.Errors.Select(e => e.ErrorMessage), "400"));

            var student = await _studentsRepository.GetStudentByEmailAsync(otpDto.EmailId);
            if (student == null)
                return Ok(new ApiResponse(false, "Student not found.", null, "400"));

            // Generate and save new OTP
            var otp = GenOPT(6);
            var otpRecord = new TblUserRandomPass
            {
                UserRandomId = 0,
                UserId = (int)student.StudentUserId,
                VerificationCode = otp,
                GeneratedTime = DateTime.Now
            };
            await _studentsRepository.RegenerateOtpAsync(otpRecord);

            // Send email
            await _emailService.SendEmailAsync(student.EmailId, "Your New OTP Code", $"Your new OTP code is: {otp}");

            return Ok(new ApiResponse(true, "New OTP sent to your email", otpRecord.VerificationCode));
        }

        #endregion


        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequest model)
        {
            if(string.IsNullOrEmpty(model.AccessToken))
                return Ok(new ApiResponse(false, "Token is required", null, "400"));

            string tokendecoded = Encoding.UTF8.GetString(Convert.FromBase64String(model.AccessToken));
            var principal = _jwtTokenService.GetPrincipalFromExpiredToken(tokendecoded);
            var userId = principal.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

            var student = await _context.TblStudentUserMasters.FirstOrDefaultAsync(x => x.StudentUserId.ToString() == userId);

            if (student == null)
                return Unauthorized(new ApiResponse(false, "Invalid student", null, "401"));

            var token = _jwtTokenService.GenerateToken(student.EmailId, student.StudentUserId.ToString(), student.Username);
            string base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
            student.Token = base64Encoded;
            await _studentsRepository.UpdateStudentAsync(student);
            return Ok(new ApiResponse { Success = true, Message = "Token refreshed successfully", Data = new { AccessToken = base64Encoded } });
        }

    }
}
