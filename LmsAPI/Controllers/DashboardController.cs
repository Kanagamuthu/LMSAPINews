using LMSAPI.DTO;
using LMSAPI.Helpers;
using LMSAPI.Models;
using LMSAPI.Repository;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static LMSAPI.DTO.LessonConverter;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LMSAPI.Controllers
{
    [Authorize]
    [ApiController]
    [TypeFilter(typeof(ExceptionFilter))]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class DashboardController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IDashboardRepository _dashboardRepository;
        private readonly IStudentsRepository _studentsRepository;
        private readonly IDistributedCache _cache;
        private readonly IConfiguration _configuration;
        private readonly LmsdbNewContext _context;

        public DashboardController(ILoggerManager logger, IDashboardRepository dashboardRepository, IStudentsRepository studentsRepository, IDistributedCache cache, IConfiguration configuration, LmsdbNewContext context)
        {
            _logger = logger;
            _dashboardRepository = dashboardRepository;
            _studentsRepository = studentsRepository;
            _cache = cache;
            _configuration = configuration;
            _context = context;
        }

        #region validate student is validate or not from session
        [HttpGet("IsValidStudent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult IsValidStudent()
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

        #endregion


        #region post-register student info trade & department
        [HttpPost("Post-RegisterStudent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostRegisterStudent([FromBody] StudentTradeDepartmentDTO obj)
        {
            var errors = new List<string>
            {
                (obj.edutype == null || obj.edutype <= 0) ? "Education type is required." : null,
                string.IsNullOrWhiteSpace(obj.department_name)  ? "Department is required." : null,
                string.IsNullOrWhiteSpace(obj.batchyear)  ? "Batch year is required." : null,
                string.IsNullOrWhiteSpace(obj.collegename)  ? "College name is required." : null,

            };



            errors.RemoveAll(e => e == null);

            if (errors.Count > 0)
                return Ok(new ApiResponse { Success = false, Message = string.Join(", ", errors), Data = "", ErrorCode = "400" });
            var email = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _dashboardRepository.PostRegisterStudentTradeDepartment(email, obj);
            return Ok(new ApiResponse { Success = true, Message = "Education details added successfully.", Data = result, ErrorCode = "200" });

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
        #endregion

        #region List all subject which is avilabe
        [HttpGet("GetAllSubjects")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllSubjects()
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
                string cacheKey = $"AllSubjects";
                var cachedSubjects = await _cache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(cachedSubjects))
                {
                    var getsubjects = System.Text.Json.JsonSerializer.Deserialize<List<TblSubjectMaster>>(cachedSubjects);
                    return Ok(new ApiResponse { Success = true, Message = "Subjects fetched successfully (from cache)", Data = getsubjects });
                }

                var subjects = await _dashboardRepository.GetAllSubjects();

                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                };

                var jsonData = System.Text.Json.JsonSerializer.Serialize(subjects);
                await _cache.SetStringAsync(cacheKey, jsonData, cacheOptions);


                return Ok(new ApiResponse { Success = true, Message = "Subjects fetched successfully", Data = subjects, ErrorCode = null });
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

            //check user email from session
            var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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
        #endregion

        #region get list of active trade from Tbl_student_trial_subject
        [HttpGet("GetActiveTradesByUserEmail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetActiveTrades()
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

            var userId = Convert.ToInt64(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value);
            var email = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var student = await _studentsRepository.GetStudentByEmailAsync(email);

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
                var SetInActive = _dashboardRepository.SetInactive(userId);
                //remove Trail button from clients

                return Ok(new ApiResponse(true, "Your trial period has been ended & contents are get limited.", new { InTrialPeriod = false, DaysLeft = 0 }, StatusCodes.Status200OK.ToString()));
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
            string Message = ""; var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(model.Type))
                errors.Add("Type is required.");
            else if (model.packageId == null || model.packageId <= 0)
                errors.Add("packageId is required.");

            if (errors.Any())
                return Ok(new ApiResponse { Success = false, Message = string.Join(",", errors), ErrorCode = "400" });

            var getpaymentPackage = await _dashboardRepository.GetpaymentPackage(model.packageId ?? 0);
            var userId = Convert.ToInt64(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value);
            //if (model.Type.ToLower() == "insert")
            //{
            TblUserSubscribeMaster obj = new TblUserSubscribeMaster();
            obj.UserId = userId;
            obj.PackageId = model.packageId;
            obj.Amount = getpaymentPackage?.FirstOrDefault()?.packagemaster.SellingPrice;
            obj.CreatedOn = DateTime.Now;
            obj.TransactionType = "Pay";
            obj.PaymentOn = DateTime.Now;
            obj.PaymentRefNo = model.PaymentRefNo;
            obj.PaymentStatus = model.PaymentStatus;

            await _dashboardRepository.AddUserSubscribeMasterAsync(obj);
            //Message = "Subscription created successfully.";
            //return Ok(new ApiResponse(true, Message, obj, ""));
            //}
            //else
            //{
            //var UserSubscribeMaster = await _dashboardRepository.GetUserSubscribeMasterAsync();
            //var getData = UserSubscribeMaster.Where(x => x.UserSubscribeMaster.Amount == getpaymentPackage?.FirstOrDefault()?.packagemaster.SellingPrice && x.UserSubscribeMaster.UserId == userId && x.UserSubscribeMaster.PaymentStatus == "Pending").FirstOrDefault()?.UserSubscribeMaster ?? new TblUserSubscribeMaster();
            //getData.PaymentOn = DateTime.Now;
            //getData.PaymentRefNo = model.PaymentRefNo;
            //getData.PaymentStatus = model.PaymentStatus;
            //await _dashboardRepository.UpdateUserSubscribeMasterAsync(getData);

            List<TblUserSubjectActivationHistory> obj2 = new List<TblUserSubjectActivationHistory>();

            foreach (var item in getpaymentPackage?.FirstOrDefault()?.subjectmaster)
            {
                TblUserSubjectActivationHistory obj1 = new TblUserSubjectActivationHistory();
                var DepartmentId = getpaymentPackage?.FirstOrDefault()?.packagedetails.FirstOrDefault(x => x.SubjectId == item.SubjectId)?.DepartmentId;
                obj1.TusmId = obj.UserSubscribeMasterId;
                obj1.SubjectId = Convert.ToInt32(item.SubjectId);
                obj1.SubjectCode = item.SubjectCode;
                obj1.SubjectName = item.SubjectName;
                obj1.SubjectVersion = item.SubjectVersion;
                obj1.UserId = Convert.ToInt32(userId);
                obj1.DepartmentId = DepartmentId;
                if (model.PaymentStatus.ToLower() == "success")
                {
                    obj1.SubjectExpiryDate = DateTime.Now.AddDays(getpaymentPackage?.FirstOrDefault()?.packagemaster.PackageDurationDays ?? 0);
                    obj1.ActivatedOn = DateTime.Now;
                    obj1.ActivatedBy = Convert.ToInt32(userId);
                }
                obj2.Add(obj1);
            }

            await _dashboardRepository.AddUserSubjectActivationHistoryAsync(obj2);
            Message = "Subscription Added successfully";
            return Ok(new ApiResponse(true, Message, obj, ""));

            //}
        }
        #endregion

        #region get subject list for dashboard
        [HttpGet("GetDashboardSubjects")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDashboardSubjects(string? SubjectCode)
        {

            var email = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                return Ok(new ApiResponse { Success = false, Message = "User not logged in", Data = null, ErrorCode = "401" });
            }

            string cacheKey = $"DashboardUniqueSubjects";
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                var cachedSubjects = System.Text.Json.JsonSerializer.Deserialize<List<DepartmentSubjectDTO>>(cachedData.ToString());
                if (!string.IsNullOrEmpty(SubjectCode))
                    cachedSubjects = cachedSubjects.Where(x => x?.subjectMaster?.SubjectCode == SubjectCode).ToList();
                return Ok(new ApiResponse(true, "Subjects fetched successfully (from cache).", cachedSubjects, ""));
            }

            var subjects = await _dashboardRepository.GetAllDepartmentSubjects();
            var jsonData = System.Text.Json.JsonSerializer.Serialize(subjects);
            if (!string.IsNullOrEmpty(SubjectCode))
                subjects = subjects.Where(x => x?.subjectMaster?.SubjectCode == SubjectCode).ToList();

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
            };
            await _cache.SetStringAsync(cacheKey, jsonData, cacheOptions);


            return Ok(new ApiResponse(true, "Subjects fetched successfully.", subjects, ""));

        }
        #endregion



        #region get All Package list for dashboard
        [HttpGet("GetAllPackage")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllPackage()
        {
            var email = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //get package id based on user id
            var stddepartment = await _dashboardRepository.GetAllPackageByUserEmailAsync(email);
            int education = stddepartment.FirstOrDefault()?.EduType ?? 0;

            //get department id based on user email
            if (education > 0)
            {
                var res = await _dashboardRepository.GetPackageDetailsByUserEmailAsync(education);
                return Ok(new ApiResponse { Success = true, Message = "Departments fetched successfully for the user", Data = res, ErrorCode = "200" });
            }
            else
            {
                return Ok(new ApiResponse { Success = false, Message = "There is no package available for the user", Data = "", ErrorCode = "404" });
            }

        }
        #endregion

        #region get Particular Package list 
        [HttpPost("GetPackageDetailsPackageId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPackageDetails(GetPackageByIdDto PackageId)
        {
            int userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value);
            if (PackageId == null || string.IsNullOrEmpty(PackageId.PackageID))
                return Ok(new ApiResponse { Success = false, Message = "PackageId Reuired", Data = "", ErrorCode = "400" });
            else
            {
                var getAllPackage = await _dashboardRepository.GetPackageDetails(PackageId.PackageID, userId);
                if (getAllPackage == null)
                    return Ok(new ApiResponse(false, "No package found with the given PackageId.", "", errorCode: "404"));
                else

                    return Ok(new ApiResponse(true, "Packages details fetched successfully.", getAllPackage, errorCode: "200"));

            }
        }
        #endregion
        //04/11/2025
        #region list the degrees
        [HttpGet("GetAllDegrees")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllDegrees()
        {

            var email = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var degrees = new List<TblDegreeMaster>();
            if (string.IsNullOrEmpty(email))
            {
                return Ok(new ApiResponse { Success = false, Message = "User not logged in", Data = null, ErrorCode = "401" });
            }
            else
            {
                string cacheKey = $"AllDegrees";
                var cachedDegrees = await _cache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(cachedDegrees))
                {
                    var getdegrees = System.Text.Json.JsonSerializer.Deserialize<List<TblDegreeMaster>>(cachedDegrees);
                    return Ok(new ApiResponse { Success = true, Message = "Degrees fetched successfully.", Data = getdegrees });
                }
                degrees = await _dashboardRepository.GetAllDegrees();
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                };
                var jsonData = System.Text.Json.JsonSerializer.Serialize(degrees);
                await _cache.SetStringAsync(cacheKey, jsonData, cacheOptions);

            }
            return Ok(new ApiResponse { Success = true, Message = "Degrees fetched successfully.", Data = degrees, ErrorCode = null });


        }
        #endregion

        //#region user subject trail periods information
        //[HttpPost("AddSubjectToStudent")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> AddSubjectToStudent([FromBody] List<int> subjectId)
        //{
        //    try
        //    {
        //        //check user email from session
        //        var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //        if (string.IsNullOrEmpty(userEmail))
        //        {
        //            return Ok(new ApiResponse { Success = false, Message = "User email not found in session", Data = null, ErrorCode = "401" });
        //        }
        //        else
        //        {
        //            var student = await _studentsRepository.GetStudentByEmailAsync(userEmail);
        //            if (student == null)
        //            {
        //                return Ok(new ApiResponse { Success = false, Message = "Student not found", Data = null, ErrorCode = "404" });
        //            }

        //            //validate no of books per student
        //            int maxBooksAllowed = await _dashboardRepository.GetBookLimitPerStudentAsync();
        //            int currentBookCount = await _dashboardRepository.GetCurrentBookCountForStudentforTrailAsync(Convert.ToInt16(student.StudentUserId));
        //            if (currentBookCount + subjectId.Count > maxBooksAllowed)
        //            {
        //                return Ok(new ApiResponse { Success = false, Message = $"Book limit exceeded. You can only add {maxBooksAllowed - currentBookCount} more books.", Data = null, ErrorCode = "400" });
        //            }
        //            else
        //            {
        //                //add books to student
        //                await _dashboardRepository.AddBooksToStudent(userEmail, subjectId);
        //                return Ok(new ApiResponse { Success = true, Message = "Books added to student successfully", Data = null, ErrorCode = null });
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error occurred in AddBooksToStudent method");
        //        return StatusCode(500, "Internal server error");
        //    }

        //}
        //#endregion

        [HttpPost("TrailSubscription")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> TrailSubscription(PaymentPayload model)
        {
            string Message = ""; var errors = new List<string>();


            if (model.packageId == null || model.packageId <= 0)
                errors.Add("packageId is required.");

            if (errors.Any())
                return Ok(new ApiResponse { Success = false, Message = string.Join(",", errors), ErrorCode = "400" });

            var getpaymentPackage = await _dashboardRepository.GetpaymentPackage(model.packageId ?? 0);
            var userId = Convert.ToInt64(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value);

            TblUserSubscribeMaster obj = new TblUserSubscribeMaster();
            obj.UserId = userId;
            obj.PackageId = model.packageId;
            obj.Amount = getpaymentPackage?.FirstOrDefault()?.packagemaster.SellingPrice;
            obj.PaymentStatus = "success";
            obj.CreatedOn = DateTime.Now;
            obj.PaymentOn = DateTime.Now;
            obj.TransactionType = "Trail";

            await _dashboardRepository.AddUserSubscribeMasterAsync(obj);

            List<TblUserSubjectActivationHistory> obj2 = new List<TblUserSubjectActivationHistory>();

            foreach (var item in getpaymentPackage?.FirstOrDefault()?.subjectmaster)
            {
                TblUserSubjectActivationHistory obj1 = new TblUserSubjectActivationHistory();
                var DepartmentId = getpaymentPackage?.FirstOrDefault()?.packagedetails.FirstOrDefault(x => x.SubjectId == item.SubjectId)?.DepartmentId;
                obj1.TusmId = obj.UserSubscribeMasterId;
                obj1.SubjectId = Convert.ToInt32(item.SubjectId);
                obj1.SubjectCode = item.SubjectCode;
                obj1.SubjectName = item.SubjectName;
                obj1.SubjectVersion = item.SubjectVersion;
                obj1.UserId = Convert.ToInt32(userId);
                obj1.DepartmentId = DepartmentId;
                if (obj.PaymentStatus.ToLower() == "success")
                {
                    var activeDate = await _dashboardRepository.GetActiveOnDateByUserId(userId);
                    var trialDays = await _dashboardRepository.GetTrialPeriodDaysAsync();
                    obj1.SubjectExpiryDate = (activeDate ?? DateTime.Now).AddDays(trialDays);

                    obj1.ActivatedOn = DateTime.Now;
                    obj1.ActivatedBy = Convert.ToInt32(userId);
                }
                obj2.Add(obj1);


            }
            await _dashboardRepository.AddUserSubjectActivationHistoryAsync(obj2);
            Message = "Subscription Added successfully";
            return Ok(new ApiResponse(true, Message, obj, ""));
        }


        [HttpGet("GetDepartmentMaster")]
        public async Task<IActionResult> GetDepartmentMaster()
        {
            var registerList = await _dashboardRepository.GetRegisterDropdwonList();
            return Ok(new ApiResponse(true, "Department list fetched successfully.", registerList, ""));
        }

        #region get education list
        [HttpGet("GetEducationTypeList")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEducationTypeList()
        {
            var educationTypeList = await _dashboardRepository.GetEducationTypeListAsync();
            return Ok(new ApiResponse(true, "Education type list fetched successfully.", educationTypeList, ""));
        }
        #endregion


        #region get student purchase item list
        [HttpGet("GetUserPurchaseItems")]
        public async Task<IActionResult> GetUserPurchaseItems()
        {
            var userId = Convert.ToInt64(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value);
            var result = await _dashboardRepository.GetUserPurchaseExpiryAsync(userId);
            if (result == null || result.Count == 0)
                return Ok(new ApiResponse(false, "No purchase items found for the user.", "", "404"));
            else
                return Ok(new ApiResponse(true, "User purchase items fetched successfully.", result, "200"));
        }
        #endregion



        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder(PaymentRequest req)
        {
            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value);
            var razorpayKey = _configuration["razorpay:key"];
            var razorpaySecret = _configuration["razorpay:secret"];
            //var email = _configuration["razorpay:email"];
            //var phone = _configuration["razorpay:phone"];
            var client = new Razorpay.Api.RazorpayClient(razorpayKey, razorpaySecret);

            var GetPackage = _context.TblPackageMasters.FirstOrDefault(x => x.PackageId == req.ProductId);
            var Getstudent = _context.TblStudentUserMasters.FirstOrDefault(x => x.StudentUserId == userId);
            var price = Convert.ToDouble(GetPackage?.SellingPrice??"0") * 100;

            var options = new Dictionary<string, object>
            {
                  { "amount", price }, // amount in paise
                  { "currency", "INR" },
                  { "receipt", "order_rcptid_" + req.ProductId },
                  { "payment_capture", 1 }
            };
            Razorpay.Api.Order order = client.Order.Create(options);
            //cerate a record in databse with order details and status as created
            var res = await _dashboardRepository.CreateRazorpayOrderRecord(req.ProductId, order?["id"]?.ToString(), userId, "created");
            return Ok(new
            {
                success = true,
                orderId = order["id"].ToString(),
                key = razorpayKey,
                email = Getstudent.EmailId,
                phone = Getstudent.CountryCode + "-" + Getstudent.Mobile,
                amount = price,
            });
        }

        [HttpPost("Verify")]
        public async Task<IActionResult> VerifyPayment([FromBody] VerifyRequest req)
        {
            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value);

            var razorpayKey = _configuration["razorpay:key"];
            var razorpaySecret = _configuration["razorpay:secret"];

            var client = new Razorpay.Api.RazorpayClient(razorpayKey, razorpaySecret);

            var attributes = new Dictionary<string, string>
             {
         { "razorpay_order_id", req.OrderId },
         { "razorpay_payment_id", req.PaymentId },
         { "razorpay_signature", req.Signature }
            };

            bool isValid = Razorpay.Api.Utils.ValidatePaymentSignature(attributes);

            if (isValid)
            {
                //update payment status in database as successful
                var res = await _dashboardRepository.UpdateRazorpayOrderStatus(req.OrderId, req.PaymentId, req.Signature, userId, "successful");


                PaymentPayload obj = new PaymentPayload();
                obj.packageId = res.PackageId;
                obj.PaymentRefNo = req.PaymentId;
                obj.PaymentStatus = "success";
                obj.Type = "insert";
                var subscriptionResult = await CreateSubscription(obj);
                dynamic result = subscriptionResult is string ? JsonConvert.DeserializeObject(subscriptionResult.ToString()) : subscriptionResult;
                var data = result?.Value?.Data;
                return Ok(new ApiResponse(true, "Payment verified successfully.", data, "200"));
            }
            return BadRequest(new ApiResponse(false, "Signature mismatch", null, "400"));
        }

    }
}
