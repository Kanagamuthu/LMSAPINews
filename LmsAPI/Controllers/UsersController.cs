using FluentValidation;
using LMSAPI.DTO;
using LMSAPI.Helpers;
using LMSAPI.Models;
using LMSAPI.Repository;
using log4net.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using static System.Net.WebRequestMethods;

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

        #region student register
        [HttpPost("RegisterStudent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterStudent([FromBody] StudentRegisterDto studentDto)
        {
            var validationResult = await _validator.ValidateAsync(studentDto);
            if (!validationResult.IsValid)
                return Ok(new ApiResponse { Success = false, Message = string.Join(",", validationResult.Errors.Select(e => e.ErrorMessage)), ErrorCode = "400" });
            var existing = await _studentsRepository.GetStudentByEmailAsync(studentDto.EmailId);


            if (existing != null && existing?.ActiveStatus == 1) return Ok(new ApiResponse { Success = false, Message = "Email already registered", ErrorCode = "" });
            else if (existing != null && existing?.ActiveStatus == 0)
            {
                //update user with new details
                existing.Username = studentDto.Username;
                existing.UserFirstName = studentDto.Username;
                existing.Mobile = studentDto.Mobile;
                existing.PrimaryMac = studentDto.DeviceMacId;
                existing.CountryCode = studentDto.CountryCode;
                await _studentsRepository.UpdateStudentAsync(existing);
                // 1) Optionally delete OTP
                await _studentsRepository.DeleteOtpAsync((int)existing.StudentUserId);
            }
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
                //SetStudentSession(student);
                await _studentsRepository.AddStudentAsync(student);
                existing = await _studentsRepository.GetStudentByEmailAsync(studentDto.EmailId);
            }

            var _againotp = GenOPT(6);
            var otpRecord = new TblUserRandomPass
            {
                UserRandomId = 0,
                UserId = (int)existing.StudentUserId,
                VerificationCode = _againotp,
                GeneratedTime = DateTime.Now,
                ActionType = 1,
                UserType = 2,
            };
            //user info in session
            //SetStudentSession(existing);
            bool is_saved = await _studentsRepository.SaveOtpAsync(otpRecord);
            //validate db otp save or not
            if (is_saved == false)
            {
                return Ok(new ApiResponse { Success = false, Message = "Failed to generate OTP. Please try again.", ErrorCode = "500" });
            }
            else
            {
                //send email
                await _emailService.SendEmailAsync(existing.EmailId, "Your OTP Code", $"Your OTP code is: {_againotp}");
                // TODO: send OTP via email (using a mail service)
                return Ok(new ApiResponse { Success = true, Message = "OTP again sent to your email", Data = _againotp });
            }
        }

        #endregion

        #region student login
        [HttpPost("Student-login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStudentInfo(StudentLoginDto logInDto)
        {
            //check mail, decice mac null
            if (string.IsNullOrEmpty(logInDto.EmailId) || string.IsNullOrEmpty(logInDto.DeviceMac))
            {
                return BadRequest(new ApiResponse(false, "Email and Device MAC ID are required.", null, "400"));
            }
            //validate email format
            var emailValidation = await _emailValidator.ValidateAsync(logInDto.EmailId);
            if (!emailValidation.IsValid)
            {
                return Ok(new ApiResponse(false, "Invalid email format.", emailValidation.Errors.Select(e => e.ErrorMessage), "400"));
            }
            var student = await _studentsRepository.GetStudentByEmailAsync(logInDto.EmailId);
            //check student info is active or incative
            if (student != null && student.ActiveStatus == 1)
            {
                //here cheack same device or different device
                var isSameDevice = await _studentsRepository.ValidDeviceAsync(logInDto.EmailId, logInDto.DeviceMac);
                if (!isSameDevice)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Login attempt from a different device. do you want login?",
                        Data = null,
                        ErrorCode = "403"
                    });
                }
                else
                {
                    // 1) Optionally delete OTP
                    await _studentsRepository.DeleteOtpAsync((int)student.StudentUserId);
                    //create otp
                    var _againotp = GenOPT(6);
                    var otpRecord = new TblUserRandomPass
                    {
                        UserRandomId = 0,
                        UserId = (int)student.StudentUserId,
                        VerificationCode = _againotp,
                        GeneratedTime = DateTime.Now,
                        ActionType = 1,
                        UserType = 2,
                    };
                    bool is_saved = await _studentsRepository.SaveOtpAsync(otpRecord);
                    //validate db otp save or not
                    if (is_saved == false)
                    {
                        return Ok(new ApiResponse { Success = false, Message = "Failed to generate OTP. Please try again.", ErrorCode = "500" });
                    }
                    else
                    {
                        await _emailService.SendEmailAsync(student.EmailId, "Your OTP Code", $"Your OTP code is: {_againotp}");
                        return Ok(new ApiResponse { Success = true, Message = "OTP sent to your email", Data = _againotp });
                    }
                }
            }

            else if ((student == null || student != null) || student?.ActiveStatus == 0)
            {
                _logger.LogInfo($"Student not found: {logInDto.EmailId}");
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "Email not registered. Please register first.",
                    Data = null,
                    ErrorCode = "404"
                });
            }
            //check mi-match email and device mac id
            else
            {
                var isSameDevice = await _studentsRepository.ValidDeviceAsync(logInDto.EmailId, logInDto.DeviceMac);
                if (!isSameDevice)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Login attempt from a different device. do you want login?",
                        Data = null,
                        ErrorCode = "403"
                    });
                }
                else
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Email not registered. Please register first.",
                        Data = null,
                        ErrorCode = "404"
                    });
                }

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

            // 1️) Validate input
            if (otpDto == null || string.IsNullOrEmpty(otpDto.EmailId) || string.IsNullOrEmpty(otpDto.Otp))
                return Ok(new ApiResponse(false, "Email and OTP are required.", null, errorCode));

            var emailValidation = await _emailValidator.ValidateAsync(otpDto.EmailId);
            if (!emailValidation.IsValid)
                return Ok(new ApiResponse(false, string.Join(", ", emailValidation.Errors.Select(e => e.ErrorMessage)), null, errorCode));

            // 2️) Get student
            var student = await _studentsRepository.GetStudentByEmailAsync(otpDto.EmailId);
            if (student == null)
                return Ok(new ApiResponse(false, "Student not found.", null, errorCode));

            // 3️) Get latest OTP
            var otpRecord = await _studentsRepository.GetLatestOtpAsync((int)student.StudentUserId, 1, 2);
            if (otpRecord == null || otpRecord.VerificationCode != otpDto.Otp)
                return Ok(new ApiResponse(false, "Enter valid OTP", null, errorCode));

            if (otpRecord.GeneratedTime.AddMinutes(10) < DateTime.Now)
                return Ok(new ApiResponse(false, "OTP has expired", null, errorCode));
            // 4️) Activate student
            student.ActiveStatus = 1;
            student.Istrail = true;
            student.AccActiveOn = DateTime.Now;
            var token = _jwtTokenService.GenerateToken(student.EmailId, student.StudentUserId.ToString(), student.Username);
            student.Token = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
            await _studentsRepository.UpdateStudentAsync(student);
            // 5️) Optionally delete OTP
            await _studentsRepository.DeleteOtpAsync(otpRecord.UserId);

            //validate student 
            if (student!=null)
            {
                var result = new
                {
                    StudentUserId = student.StudentUserId.ToString(),
                    UserFirstName = student.UserFirstName,
                    Username = student.Username,
                    EmailId = student.EmailId,
                    Mobile = student.Mobile,
                    ActiveStatus = student.ActiveStatus.ToString(),
                    AccActiveOn = student.AccActiveOn,
                    CreatedBy = student.CreatedBy?.ToString(),
                    CreatedOn = student.CreatedOn,
                    Collegename = student.Collegename,
                    DepartmentId = student.DepartmentId?.ToString(),
                    DepartmentName = student.DepartmentName,
                    Batchyear = student.Batchyear?.ToString(),
                    Country = student.Country,
                    PrimaryImei = student.PrimaryImei,
                    PrimaryMac = student.PrimaryMac,
                    CountryCode = student.CountryCode,
                    EduType = student.EduType?.ToString(),
                    TradeId = student.TradeId?.ToString(),
                    Token = student.Token,
                    Istrail = student.Istrail
                };
                return Ok(new ApiResponse(true, "Account activated successfully.", data: result, errorCode: "200"));
            }
            else
            {
                return Ok(new ApiResponse(false, "Failed to activate account.", "", errorCode: "500"));
            }


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

            await _studentsRepository.DeleteOtpAsync((int)student.StudentUserId);
            await _studentsRepository.RegenerateOtpAsync(otpRecord);
            // Send email
            await _emailService.SendEmailAsync(student.EmailId, "Your New OTP Code", $"Your new OTP code is: {otp}");

            return Ok(new ApiResponse(true, "New OTP sent to your email", otpRecord.VerificationCode));
        }

        #endregion

        [HttpPost("RefreshToken")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequest model)
        {
            if (string.IsNullOrEmpty(model.AccessToken))
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

        #region validate email id, device id wether same device or not
        [HttpPost("Validate-MultiDevice")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ValidateMultiDevice([FromBody] ValidMailDeviceDto request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.DeviceId))
            {
                return BadRequest(new ApiResponse(false, "Email and Device MAC are required.", null, "400"));
            }
            bool issame_device = await _studentsRepository.ValidDeviceAsync(request.Email, request.DeviceId);
            if (issame_device == false)
            {
                return Ok(new ApiResponse(true, "This is diffrent device", new { IsSameDevice = false }));
            }
            else
            {
                return Ok(new ApiResponse(true, "Device validated successfully.", new { IsSameDevice = true }));
            }
        }
        #endregion

        #region login
        [HttpPost("Enter-OTP")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UserLogin(OTPVerificationDto oTPVerificationDto)
        {
            if (string.IsNullOrEmpty(oTPVerificationDto.EmailId) || string.IsNullOrEmpty(oTPVerificationDto.deviceMacId) || string.IsNullOrEmpty(oTPVerificationDto.Otp))
            {
                return BadRequest(new ApiResponse(false, "Email, Device MAC, and OTP are required.", null, "400"));
            }
            var student = await _studentsRepository.GetStudentByEmailAsync(oTPVerificationDto.EmailId);
            if (student == null || student.ActiveStatus != 1)
            {
                return Ok(new ApiResponse(false, "Invalid student or inactive account.", null, "401"));
            }
            var isValidOtp = await _studentsRepository.ValidateOtpAsync((int)student.StudentUserId, oTPVerificationDto.Otp);
            if (!isValidOtp)
            {
                return Ok(new ApiResponse(false, "Invalid OTP. Please try again.", null, "402"));
            }
            //update mac-id
            student.PrimaryMac = oTPVerificationDto.deviceMacId;
            bool isUpdated = await _studentsRepository.UpdateStudentAsync(student);
            if (!isUpdated)
            {
                return Ok(new ApiResponse(false, "Failed to update device information.", null, "500"));
            }
            // If we reach here, the login is successful
            SetStudentSession(student);
            return Ok(new ApiResponse(true, "Login successful.", null));
        }
        #endregion

        #region otp generation
        [HttpPost("Generate-OTP")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GenerateOtp(GenerateOtpDto generateOtp)
        {
            if (string.IsNullOrEmpty(generateOtp.EmailId) || string.IsNullOrEmpty(generateOtp.deviceMacId))
            {
                return BadRequest(new ApiResponse(false, "Email, Device MAC are required.", null, "400"));
            }
            var student = await _studentsRepository.GetStudentByEmailAsync(generateOtp.EmailId);
            if (student == null || student.ActiveStatus != 1)
            {
                return Ok(new ApiResponse(false, "Invalid student or inactive account.", null, "401"));
            }
            // 1) Optionally delete OTP
            await _studentsRepository.DeleteOtpAsync((int)student.StudentUserId);
            //create otp
            var _againotp = GenOPT(6);
            var otpRecord = new TblUserRandomPass
            {
                UserRandomId = 0,
                UserId = (int)student.StudentUserId,
                VerificationCode = _againotp,
                GeneratedTime = DateTime.Now,
                ActionType = 1,
                UserType = 2,
            };
            bool is_saved = await _studentsRepository.SaveOtpAsync(otpRecord);
            //validate db otp save or not
            if (is_saved == false)
            {
                return Ok(new ApiResponse { Success = false, Message = "Failed to generate OTP. Please try again.", ErrorCode = "500" });
            }
            else
            {
                await _emailService.SendEmailAsync(student.EmailId, "Your OTP Code", $"Your OTP code is: {_againotp}");
                return Ok(new ApiResponse { Success = true, Message = "OTP sent to your email", Data = _againotp });
            }
        }
        #endregion
    }
}
