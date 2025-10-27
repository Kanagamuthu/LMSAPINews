using FluentValidation;
using LmsAPI.Models;
using LMSAPI.DTO;
using LMSAPI.Helpers;
using LMSAPI.Models;
using LMSAPI.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LmsAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class UsersController : ControllerBase
    {
  
        private readonly IStudentsRepository _studentsRepository;
        private readonly ILoggerManager _logger;
        private readonly IValidator<string> _emailValidator;
        private readonly IValidator<StudentRegisterDto> _validator;
        private readonly EmailService _emailService;
        private readonly JwtTokenService _jwtTokenService;
        public UsersController(IStudentsRepository studentsRepository, ILoggerManager logger, IValidator<string> emailValidator,
            IValidator<StudentRegisterDto> validator,EmailService emailService, JwtTokenService jwtTokenService)
        {
            _studentsRepository = studentsRepository;
            _logger = logger;
            _emailValidator = emailValidator;
            _validator = validator;
            _emailService = emailService;
            _jwtTokenService = jwtTokenService;
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
                    return BadRequest(new { Message = "Invalid email format.", Errors = validationResult.Errors.Select(e => e.ErrorMessage) });
                }

                var student = await _studentsRepository.GetStudentByEmailAsync(email);
                if (student == null)
                {
                    return NotFound(new { Message = "Student not found." });
                }
                else
                {
                    var token = _jwtTokenService.GenerateToken(student.EmailId);
                    string base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
                    _logger.LogInfo($"Student found: {student.EmailId}, Status: {student.ActiveStatus}");
                    SetStudentSession(student);

                    return Ok(new ApiResponse
                    {
                        Success = true,
                        Message = "Email found. Please verify your email.",
                        Data = new
                        {
                            student.StudentUserId,
                            student.Username,
                            student.EmailId,
                            student.Mobile,
                            student.ActiveStatus,
                            Token = base64Encoded
                        }
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
            try
            {
                // Validate the incoming student data
                var validationResult = await _validator.ValidateAsync(studentDto);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new { Message = "Validation failed.", Errors = validationResult.Errors.Select(e => e.ErrorMessage) });
                }
                if (studentDto == null) return BadRequest("Invalid data");
                // Check existing user
                var existing = await _studentsRepository.GetStudentByEmailAsync(studentDto.EmailId);

                if (existing != null)
                {
                    return BadRequest("Email already registered");
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
                        GeneratedTime = DateTime.UtcNow,
                        ActionType = 1, // 1 for registration
                        UserType = 2, // 2 for student
                    };
                    await _studentsRepository.SaveOtpAsync(otpRecord);
                    //send email
                    await _emailService.SendEmailAsync(student.EmailId, "Your OTP Code", $"Your OTP code is: {otp}");


                    // TODO: send OTP via email (using a mail service)
                    return Ok(new { Message = "OTP sent to your email", Otp = otp }); // return OTP only for testing
                }
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here as needed
                return StatusCode(500, new { Message = "An error occurred while processing your request.", Details = ex.Message });
            }
        }

        #endregion

        #region Validate OTP
        [HttpPost("ValidateOtp")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ValidateOtp([FromBody] OtpValidationDto otpDto)
        {
            try
            {
                if (otpDto == null || string.IsNullOrEmpty(otpDto.EmailId) || string.IsNullOrEmpty(otpDto.Otp))
                {
                    return BadRequest(new { Message = "Email and OTP are required." });
                }
                var student = await _studentsRepository.GetStudentByEmailAsync(otpDto.EmailId);
                if (student == null)
                {
                    return NotFound(new { Message = "Student not found." });
                }
                var otpRecord = await _studentsRepository.GetLatestOtpAsync((int)student.StudentUserId, 1, 2); // actionType=1 (registration), userType=2 (student)
                if (otpRecord == null || otpRecord.VerificationCode != otpDto.Otp)
                {
                    //return BadRequest(new { Message = "Invalid OTP." });
                    return BadRequest(new ApiResponse(false,"Enter valid OTP",null,StatusCodes.Status400BadRequest.ToString()));
                }
                //if (string.IsNullOrEmpty(otpDto.Otp))
                //{
                //    return BadRequest(new { Message = "OTP is required." });
                //}
                //else if (otpDto.Otp.Length < 6)
                //{
                //    return BadRequest(new { Message = "OTP must be 6 digits." });
                //}
                //else if (otpDto.Otp.Length > 6)
                //{
                //    return BadRequest(new { Message = "OTP cannot exceed 6 digits." });
                //}
                //else if (!Regex.IsMatch(otpDto.Otp, @"^\d{6}$"))
                //{
                //    return BadRequest(new { Message = "OTP must be exactly 6 digits." });
                //}

                // Check if OTP is expired (valid for 10 minutes)
                if (otpRecord.GeneratedTime.AddMinutes(10) < DateTime.UtcNow)
                {
                    //return BadRequest(new { Message = "OTP has expired." });
                    return BadRequest(new ApiResponse(false, "OTP has expired", null, StatusCodes.Status400BadRequest.ToString()));
                }
                // Mark student as active
                student.ActiveStatus = 1; // active
                await _studentsRepository.UpdateStudentAsync(student);
                // Optionally, you can delete the OTP record after successful validation
                await _studentsRepository.DeleteOtpAsync(otpRecord.UserId);
                //return Ok(new { Message = "OTP validated successfully. Your account is now active." });
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Email found. Account not activated. Please verify your email.",
                    Data = new
                    {
                        student.StudentUserId,
                        student.Username,
                        student.EmailId,
                        student.Mobile,
                        student.ActiveStatus
                    }
                });
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here as needed
                return StatusCode(500, new { Message = "An error occurred while processing your request.", Details = ex.Message });
            }
        }
        #endregion

        #region Validate trail periode from activated date
        [HttpGet("check-trial-period")]
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
                var currentDate = DateTime.UtcNow;
                var difference = currentDate - activationDate;
                int daysSinceActivation = (int)difference.TotalDays;
                int daysLeft = trialDays - daysSinceActivation;

                if (daysLeft > 0)
                {
                    return Ok(new ApiResponse(true, "Student is in trial period.", new { InTrialPeriod = true,DaysLeft = daysLeft}, StatusCodes.Status200OK.ToString()));
                }
                else
                {
                    return Ok(new ApiResponse(true, "Student trial period has ended.", new {
                            InTrialPeriod = false, DaysLeft = 0
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

    }
}
