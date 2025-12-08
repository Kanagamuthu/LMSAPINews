using LMSAPI.DTO;
using LMSAPI.Helpers;
using LMSAPI.Models;
using LMSAPI.Repository;
using log4net.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LMSAPI.Controllers
{
    [Authorize]
    [ApiController]
    [TypeFilter(typeof(ExceptionFilter))]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class MeController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IDashboardRepository _dashboardRepository;
        private readonly IStudentsRepository _studentsRepository;
        public MeController(ILoggerManager logger, IDashboardRepository dashboardRepository, IStudentsRepository studentsRepository)
        {
            _logger = logger;
            _dashboardRepository = dashboardRepository;
            _studentsRepository = studentsRepository;
        }

        #region Add Read History list for user
        [HttpPost("AddHistory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddHistory(TblReadHistory obj)
        {
            var errors = new List<string>();

            if (obj.SubjectId > 0)
                errors.Add("SubjectCode is required.");
            else if (string.IsNullOrWhiteSpace(obj.Type))
                errors.Add("Type is required.");
            else if (obj.SubjectId > 0)
                errors.Add("Url is required.");

            if (errors.Any())
                return Ok(new ApiResponse { Success = false, Message = "Validation failed.", Data = null, ErrorCode = string.Join(",", errors) });

            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            obj.CreatedDate = DateTime.Now;
            obj.Status = true;
            obj.Readby = Convert.ToInt32(userId);
            bool flag = await _dashboardRepository.GetReadHistory(obj);
            if (flag)
                return Ok(new ApiResponse(true, "History already exists.", obj, ""));
            else
            {
                await _dashboardRepository.AddReadHistoryAsync(obj);

                return Ok(new ApiResponse(true, "Histroy added successfully.", obj, ""));
            }
        }
        #endregion

        #region get Read History list for user

        [HttpGet("StudentReadHistory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> StudentReadHistory()
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                if (string.IsNullOrEmpty(email))
                {
                    return Ok(new ApiResponse { Success = false, Message = "User not logged in", Data = null, ErrorCode = "401" });
                }

                var GetList = await _dashboardRepository.ReadHistory(Convert.ToInt32(userId));
                //if (GetList == null)
                //{
                //    return Ok(new ApiResponse { Success = false, Message = "No read history found.", Data = "", ErrorCode = "404" });
                //}
                //else
                //{
                var ServerTime = DateTime.Now.ToString("yyyy-MM-dd");
                _logger.LogInfo($"ReadHistory fetched successfully for user: {email}");
                return Ok(new { Success = true, Message = "ReadHistory fetched successfully.", ServerTime = ServerTime, Data = GetList, ErrorCode = "200" });

                // }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in ReadHistory: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message, null, StatusCodes.Status500InternalServerError.ToString()));
            }
        }
        #endregion


        #region list the student purchesd items

        #endregion

    }
}
