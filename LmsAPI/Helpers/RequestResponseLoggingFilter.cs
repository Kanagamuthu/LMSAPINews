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
                try { arguments = JsonSerializer.Serialize(Sanitize(context.ActionArguments)); }
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

        // Values that must never reach a plain-text log file.
        private static readonly string[] SensitiveKeys =
            { "otp", "verificationcode", "password", "token", "accesstoken", "signature", "receipt", "secret" };

        /// <summary>
        /// Replaces the value of any sensitive-looking property with a mask, so OTPs and
        /// tokens are not written to the request log.
        /// </summary>
        private static Dictionary<string, object?> Sanitize(IDictionary<string, object?> args)
        {
            var clean = new Dictionary<string, object?>();

            foreach (var kv in args)
            {
                if (kv.Value == null) { clean[kv.Key] = null; continue; }

                if (IsSensitive(kv.Key)) { clean[kv.Key] = "***"; continue; }

                var type = kv.Value.GetType();
                if (type.IsPrimitive || kv.Value is string || kv.Value is DateTime || kv.Value is decimal)
                {
                    clean[kv.Key] = kv.Value;
                    continue;
                }

                // Complex model: mask sensitive properties one level down.
                var masked = new Dictionary<string, object?>();
                foreach (var prop in type.GetProperties())
                {
                    if (!prop.CanRead) continue;
                    object? value;
                    try { value = prop.GetValue(kv.Value); } catch { value = "(unreadable)"; }
                    masked[prop.Name] = IsSensitive(prop.Name) && value != null ? "***" : value;
                }
                clean[kv.Key] = masked;
            }

            return clean;
        }

        private static bool IsSensitive(string name)
        {
            var n = name.Replace("_", "").ToLowerInvariant();
            foreach (var s in SensitiveKeys)
                if (n.Contains(s)) return true;
            return false;
        }

        // One process-wide gate: File.AppendAllText from concurrent requests otherwise
        // throws on a locked file and the entry is silently dropped by the outer catch.
        private static readonly object _logGate = new object();

        private void AppendLog(string entry)
        {
            var documentPath = _configuration.GetSection("DocumentPath")?.Value ?? Directory.GetCurrentDirectory();
            var logFolder = Path.Combine(documentPath, "Logs");
            Directory.CreateDirectory(logFolder);
            var logFile = Path.Combine(logFolder, $"RequestLog_{DateTime.Now:yyyy-MM-dd}.txt");
            lock (_logGate)
            {
                File.AppendAllText(logFile, entry);
            }
        }
    }
}
