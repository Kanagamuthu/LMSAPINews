using LMSAPI.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LMSAPI.Helpers
{
    public class ExceptionFilter: IExceptionFilter
    {
        public readonly ILogger<ExceptionFilter> _logger;

        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            _logger = logger;
        }
        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "Unhandled exception occurred in API");

            // The detail stays in the log. Returning the raw exception message leaks SQL,
            // schema and file paths to the caller.
            context.Result = new ObjectResult(new ApiResponse
            {
                Success = false,
                Message = "An unexpected error occurred. Please try again.",
                Data = null,
                ErrorCode = "500"
            })
            {
                StatusCode = 500
            };
        }
    }
}

