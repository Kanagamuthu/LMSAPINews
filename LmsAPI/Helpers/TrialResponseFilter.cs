using LMSAPI.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LMSAPI.Helpers
{
    public class TrialResponseFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // No-op
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult objectResult && objectResult.Value is ApiResponse apiResponse && context.HttpContext.Items.ContainsKey("TrialInfo"))
            {
                var trialInfo = context.HttpContext.Items["TrialInfo"];

                var responseWithTrial = new
                {
                    success = apiResponse.Success,
                    message = apiResponse.Message,
                    trialStatus = trialInfo,
                    data = apiResponse.Data,
                    errorCode = apiResponse.ErrorCode
                };

                context.Result = new ObjectResult(responseWithTrial)
                {
                    StatusCode = objectResult.StatusCode
                };
            }
        }

    }

}
