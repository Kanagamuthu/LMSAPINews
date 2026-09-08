using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LMSAPI.Helpers
{
    // Global — logs every controller action's request and response to a plain-text file,
    // mirroring ExceptionFilter's DocumentPath\Logs convention.
    public class RequestResponseLoggingFilter : IActionFilter
    {
        private readonly IConfiguration _configuration;

        public RequestResponseLoggingFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                var request = context.HttpContext.Request;
                var userId = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "Anonymous";
                var userName = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Anonymous";              
                var controller = context.RouteData.Values["controller"]?.ToString() ?? "";
                var action = context.RouteData.Values["action"]?.ToString() ?? "";

                string arguments;
                try { arguments = JsonSerializer.Serialize(context.ActionArguments); }
                catch { arguments = "(unavailable)"; }

                var entry =
                    new string('-', 100) + Environment.NewLine +
                    $"Date         : {DateTime.Now:dd-MMM-yyyy HH:mm:ss}{Environment.NewLine}" +
                    $"Type         : REQUEST{Environment.NewLine}" +
                    $"HTTP Method  : {request.Method}{Environment.NewLine}" +
                    $"Path         : {request.Path}{Environment.NewLine}" +
                    $"Query String : {request.QueryString}{Environment.NewLine}" +
                    $"User         : {userName} (Id: {userId}){Environment.NewLine}" +
                    $"Controller   : {controller}{Environment.NewLine}" +
                    $"Action       : {action}{Environment.NewLine}" +
                    $"Arguments    : {arguments}{Environment.NewLine}";

                AppendLog(entry);
            }
            catch
            {
                // Never let logging break the actual request.
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            try
            {
                var controller = context.RouteData.Values["controller"]?.ToString() ?? "";
                var action = context.RouteData.Values["action"]?.ToString() ?? "";
                var statusCode = context.HttpContext.Response.StatusCode;

                string response = "(no result)";
                if (context.Result is OkObjectResult jr)
                {
                    try { response = JsonSerializer.Serialize(jr.Value); }
                    catch { response = "(unavailable)"; }
                }
                else if (context.Result is ViewResult vr)
                {
                    string model;
                    try { model = JsonSerializer.Serialize(vr.Model); }
                    catch { model = "(unavailable)"; }
                    response = $"View: {vr.ViewName ?? action} | Model: {model}";
                }
                else if (context.Result is RedirectToActionResult ra)
                {
                    response = $"Redirect: {ra.ControllerName}/{ra.ActionName}";
                }
                else if (context.Result is RedirectResult rr)
                {
                    response = $"Redirect: {rr.Url}";
                }
                else if (context.Result is FileResult)
                {
                    response = "(file result)";
                }

                bool isError = statusCode >= 400 || context.Exception != null;

                var body =
                    $"Type         : RESPONSE{Environment.NewLine}" +
                    $"Controller   : {controller}{Environment.NewLine}" +
                    $"Action       : {action}{Environment.NewLine}" +
                    $"Status Code  : {statusCode}{Environment.NewLine}" +
                    $"Response     : {response}{Environment.NewLine}" +
                    (context.Exception != null ? $"Exception    : {context.Exception.Message}{Environment.NewLine}" : "");

                var entry = isError
                    ? $"*** ERROR RESPONSE (Status {statusCode}) ***{Environment.NewLine}" + body +
                      $"*** END ERROR ***{Environment.NewLine}" + new string('-', 100) + Environment.NewLine + Environment.NewLine
                    : body + new string('-', 100) + Environment.NewLine + Environment.NewLine;

                AppendLog(entry);
            }
            catch
            {
                // Never let logging break the actual response.
            }
        }

        private void AppendLog(string entry)
        {
            var documentPath = _configuration.GetSection("DocumentPath")?.Value ?? Directory.GetCurrentDirectory();
            var logFolder = Path.Combine(documentPath, "Logs");
            Directory.CreateDirectory(logFolder);
            var logFile = Path.Combine(logFolder, $"RequestLog_{DateTime.Now:yyyy-MM-dd}.txt");
            File.AppendAllText(logFile, entry);
        }
    }
}
