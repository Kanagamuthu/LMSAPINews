using LmsAPI.Models;
using LMSAPI.DTO;
using LMSAPI.Helpers;
using LMSAPI.Models;
using LMSAPI.Repository;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LMSAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class DashboardController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IDashboardRepository _dashboardRepository;
        private readonly IStudentsRepository _studentsRepository;
        public DashboardController(ILoggerManager logger, IDashboardRepository dashboardRepository, IStudentsRepository studentsRepository)
        {
            _logger = logger;
            _dashboardRepository = dashboardRepository;
            _studentsRepository = studentsRepository;
        }

        #region validate student is validate or not from session
        [HttpGet("IsValidStudent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult IsValidStudent()
        {
            try
            {
                var userEmail = HttpContext.Session.GetString("UserEmail");
                if (string.IsNullOrEmpty(userEmail))
                {
                    _logger.LogWarn("User email not found in session");
                    return Unauthorized("User email not found in session");
                }
                var isValidStudent = _dashboardRepository.IsValidStudent(userEmail);
                if (isValidStudent)
                {
                    return Ok(new ApiResponse { Success = true, Message = "Account verified", Data = new { isValidStudent = true }, ErrorCode = null });
                }
                else
                {
                    return Ok(false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in IsValidStudent method");
                return StatusCode(500, "Internal server error");
            }
        }

        #endregion

        #region post-register student info trade & department
        [HttpPost("Post-RegisterStudentTrade")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostRegisterStudentTrade([FromBody] StudentTradeDepartmentDTO studentTradeDepartmentDTO)
        {
            try
            {
                //check user email from session
                var email = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(email))
                {
                    return Ok(new ApiResponse { Success = false, Message = "User email not found in session", Data = null, ErrorCode = "401" });
                }
                else
                {
                    await _dashboardRepository.PostRegisterStudentTradeDepartment(email, studentTradeDepartmentDTO);
                    return Ok(new ApiResponse { Success = true, Message = "Student department updated successfully", Data = null, ErrorCode = null });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in PostRegisterStudentTradeDepartment method");
                return StatusCode(500, "Internal server error");
            }
        }

        #endregion

        #region list subjects based on students trade(iti)/department(diploma,engineering) from subject master
        [HttpGet("GetSubjectsByStudentTrade")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSubjectsByStudentTrade()
        {
            try
            {
                //check user email from session
                var userEmail = HttpContext.Session.GetString("UserEmail");
                var tradeID = HttpContext.Session.GetInt32("trade_id");
                int _tradeID = Convert.ToInt16(tradeID);

                if (string.IsNullOrEmpty(userEmail))
                {
                    return Ok(new ApiResponse { Success = false, Message = "User email not found in session", Data = null, ErrorCode = "401" });
                }
                else
                {
                    var subjects = await _dashboardRepository.GetSubjectsByStudentTrade(userEmail, _tradeID);
                    return Ok(new ApiResponse { Success = true, Message = "Subjects fetched successfully", Data = subjects, ErrorCode = "200" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GetSubjectsByStudentDepartment method");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #region List all subject which is avilabe
        [HttpGet("GetAllSubjects")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllSubjects()
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                //check user email from session
                var userEmail = HttpContext.Session.GetString("UserEmail");
                if (string.IsNullOrEmpty(email))
                {
                    return Ok(new ApiResponse { Success = false, Message = "User email not found in session", Data = null, ErrorCode = "401" });
                }
                else
                {
                    var subjects = await _dashboardRepository.GetAllSubjects();
                    return Ok(new ApiResponse { Success = true, Message = "Subjects fetched successfully", Data = subjects, ErrorCode = null });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GetAllSubjects method");
                return StatusCode(500, "Internal server error");
            }
        }

        #endregion

        #region add book info per students
        [HttpPost("AddBooksToStudent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddBooksToStudent([FromBody] List<int> bookIds)
        {
            try
            {
                //check user email from session
                var userEmail = HttpContext.Session.GetString("UserEmail");
                if (string.IsNullOrEmpty(userEmail))
                {
                    return Ok(new ApiResponse { Success = false, Message = "User email not found in session", Data = null, ErrorCode = "401" });
                }
                else
                {
                    var student = await _studentsRepository.GetStudentByEmailAsync(userEmail);
                    if (student == null)
                    {
                        return Ok(new ApiResponse { Success = false, Message = "Student not found", Data = null, ErrorCode = "404" });
                    }

                    //validate no of books per student
                    int maxBooksAllowed = await _dashboardRepository.GetBookLimitPerStudentAsync();
                    int currentBookCount = await _dashboardRepository.GetCurrentBookCountForStudentAsync(Convert.ToInt16(student.StudentUserId));
                    if (currentBookCount + bookIds.Count > maxBooksAllowed)
                    {
                        return Ok(new ApiResponse
                        {
                            Success = false,
                            Message = $"Book limit exceeded. You can only add {maxBooksAllowed - currentBookCount} more books.",
                            Data = null,
                            ErrorCode = "400"
                        });
                    }
                    else
                    {
                        //add books to student
                        await _dashboardRepository.AddBooksToStudent(userEmail, bookIds);
                        return Ok(new ApiResponse { Success = true, Message = "Books added to student successfully", Data = null, ErrorCode = null });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in AddBooksToStudent method");
                return StatusCode(500, "Internal server error");
            }

        }
        #endregion

        #region get list of active trade from Tbl_student_trial_subject
        [HttpGet("GetActiveTradesByUserEmail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetActiveTrades()
        {
            try
            {
                var userEmail = HttpContext.Session.GetString("UserEmail");
                var userID = HttpContext.Session.GetInt32("UserId");
                var _userid = Convert.ToInt64(userID);
                if (string.IsNullOrEmpty(userEmail))
                {
                    return Ok(new ApiResponse { Success = false, Message = "User email not found in session", Data = null, ErrorCode = "401" });
                }
                else
                {
                    var student = await _studentsRepository.GetStudentByEmailAsync(userEmail);
                    if (student == null)
                    {
                        return Ok(new ApiResponse { Success = false, Message = "Student not found", Data = null, ErrorCode = "404" });
                    }
                }

                //var _listOfSubjects = await _dashboardRepository.GetActiveTradesByUserIDAsync(_userid);
                //var _listOfSubjects = await _dashboardRepository.GetActiveTradesByUserIDAsync(_userid);
                //if (_listOfSubjects != null && _listOfSubjects.Any())
                //{
                //    // There are active trades
                //    foreach (var trade in _listOfSubjects)
                //    {
                //        Console.WriteLine($"Trade ID: {trade.Id}, Book ID: {trade.BookId}");
                //    }
                //}
                //else
                //{
                //    // No active trades
                //    Console.WriteLine("No active trades found for this user.");
                //}



            }
            catch (Exception ex)
            {
                // log ex here if needed
                return StatusCode(500, new { Message = "An error occurred.", Details = ex.Message });
            }
            return Ok();
        }
        #endregion

        #region check trial period for remove content or trade from table from account is verified
        [HttpGet("CheckTrialPeriodAndRemoveContent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CheckTrialPeriodAndRemoveContent()
        {
            try
            {
                var userEmail = HttpContext.Session.GetString("UserEmail");
                var userID = HttpContext.Session.GetInt32("UserId");
                var _userid = Convert.ToInt64(userID);

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
                    return Ok(new ApiResponse(true, "Student is in trial period.", new { InTrialPeriod = true, DaysLeft = daysLeft }, StatusCodes.Status200OK.ToString()));
                }
                else
                {
                    //set trade active stats is 0 against the user
                    var SetInActive = _dashboardRepository.SetInactive(_userid);
                    //remove Trail button from clients

                    return Ok(new ApiResponse(true, "Your trial period has been ended & contents are get limited.", new { InTrialPeriod = false, DaysLeft = 0 }, StatusCodes.Status200OK.ToString()));
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

        #region Payment
        [HttpPost("CreateSubscription")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateSubscription(PaymentPayload model)
        {

            if (model == null)
                return BadRequest(new { Message = "Validation failed.", Errors = "Invalid request payload." });

            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(model.subjectCode))
                errors.Add("SubjectCode is required.");

            if (model.SubscribeMaster?.UserId == null || model.SubscribeMaster.UserId <= 0)
                errors.Add("UserId is required.");

            if (model.SubscribeMaster?.Amount == null || model.SubscribeMaster.Amount <= 0)
                errors.Add("Amount is required.");

            if (string.IsNullOrWhiteSpace(model.SubscribeMaster?.PaymentStatus))
                errors.Add("PaymentStatus is required.");

            if (errors.Any())
                return BadRequest(new { Message = "Validation failed.", Errors = errors });

            var Message = ""; bool flag = false;
            var subjectCodes = model?.subjectCode?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
            var UserSubscribeMaster = await _dashboardRepository.GetUserSubscribeMasterAsync();
             var getData = UserSubscribeMaster.Where(x => x.UserSubscribeMaster.Amount == model?.SubscribeMaster?.Amount && x.UserSubscribeMaster.UserId == model?.SubscribeMaster?.UserId && x.UserSubjectActivationHistory.DepartmentId == model?.DepartmentId && x.UserSubscribeMaster.PaymentStatus == "Pending" && subjectCodes.Contains(x?.UserSubjectActivationHistory?.SubjectCode)).FirstOrDefault()?.UserSubscribeMaster ?? new TblUserSubscribeMaster();
            model.SubscribeMaster.CreatedOn = DateTime.Now;

            if (!string.IsNullOrEmpty(model?.SubscribeMaster?.PaymentRefNo) && getData.UserSubscribeMasterId > 0)
            {
                Message = "Subscription successfully.";
                getData.PaymentStatus = model.SubscribeMaster.PaymentStatus;
                getData.PaymentRefNo = model.SubscribeMaster.PaymentRefNo;
                getData.PaymentOn = DateTime.Now;
                await _dashboardRepository.UpdateUserSubscribeMasterAsync(getData);
            }
            else if (string.IsNullOrEmpty(model?.SubscribeMaster?.PaymentRefNo) && getData.UserSubscribeMasterId > 0)
            {
                flag = true;
                Message = "Subscription created successfully.";
                getData.CreatedOn = model?.SubscribeMaster.CreatedOn;
                await _dashboardRepository.UpdateUserSubscribeMasterAsync(getData);
            }
            else
            {
                flag = true;
                Message = "Subscription created successfully.";
                await _dashboardRepository.AddUserSubscribeMasterAsync(model.SubscribeMaster);
            }

            if (flag)
            {
                if (getData.UserSubscribeMasterId > 0)
                    await _dashboardRepository.DeleteUserSubjectActivationHistoryAsync(getData.UserSubscribeMasterId);
                foreach (var code in subjectCodes)
                {
                    var subject = await _dashboardRepository.GetPaymentSubject(code);
                    if (subject == null) continue;

                    var activation = new TblUserSubjectActivationHistory
                    {
                        SubjectId = Convert.ToInt32(subject.SubjectId),
                        SubjectCode = subject.SubjectCode,
                        SubjectName = subject.SubjectName,
                        SubjectVersion = subject.SubjectVersion,
                        DepartmentId = model.DepartmentId,
                        UserId = Convert.ToInt32(model.SubscribeMaster.UserId),
                        ActivatedBy = Convert.ToInt32(model.SubscribeMaster.UserId),
                        ActivatedOn = DateTime.Now,
                        TusmId = model.SubscribeMaster.UserSubscribeMasterId == 0 ? getData.UserSubscribeMasterId : model.SubscribeMaster.UserSubscribeMasterId
                    };
                    await _dashboardRepository.AddUserSubjectActivationHistoryAsync(activation);
                }

            }
            return Ok(new
            {
                Message = Message,
                Data = model
            });
        }
        #endregion

        #region get subject list for dashboard
        [HttpGet("GetDashboardSubjects")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDashboardSubjects()
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(email))
                {
                    return Ok(new ApiResponse { Success = false, Message = "User not logged in", Data = null, ErrorCode = "401" });
                }
                var student = await _studentsRepository.GetStudentByEmailAsync(email);
                if (student == null)
                {
                    return Ok(new ApiResponse(false, "Student not found.", null, StatusCodes.Status404NotFound.ToString()));
                }
                var subjects = await _dashboardRepository.GetAllDepartmentSubjects();
                return Ok(new ApiResponse(true, "Subjects fetched successfully.", subjects, StatusCodes.Status200OK.ToString()));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetDashboardSubjects: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, "An error occurred while fetching dashboard subjects.", null, StatusCodes.Status500InternalServerError.ToString()));
            }
        }
        #endregion
    }
}
