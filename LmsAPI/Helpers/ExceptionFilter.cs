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

            context.Result = new ObjectResult(new ApiResponse
            {
                Message = context.Exception.InnerException?.Message !=null? context.Exception.InnerException?.Message: context.Exception.Message,
            })
            {
                StatusCode = 500
            };
        }
    }
}

