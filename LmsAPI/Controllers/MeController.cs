using LMSAPI.DTO;
using LMSAPI.Helpers;
using LMSAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        [HttpGet("GetStudentDashboard")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Task<IActionResult> GetStudentDashboard()
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogError("User email claim is missing.");
                    return Task.FromResult<IActionResult>(BadRequest(new ApiResponse { Success = false, Message = "User email claim is missing." }));

                }
                return Task.FromResult<IActionResult>(BadRequest(new ApiResponse { Success = false, Message = "User email claim is missing." }));

            }
            catch (Exception er)
            {
                _logger.LogError(er);
                throw;
            }
        }


    }
}
