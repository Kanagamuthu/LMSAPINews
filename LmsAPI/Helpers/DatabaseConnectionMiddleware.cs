using LMSAPI.Models;
using System.Text.Json;

namespace LMSAPI.Helpers
{
    public class DatabaseConnectionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<DatabaseConnectionMiddleware> _logger;

        public DatabaseConnectionMiddleware(RequestDelegate next, ILogger<DatabaseConnectionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, LmsdbNewContext dbContext)
        {
            try
            {
                // Check if database can connect before proceeding
                if (!await dbContext.Database.CanConnectAsync())
                {
                    _logger.LogError("❌ Database connection lost.");
                    var response = new
                    {
                        success = false,
                        message = "Database connection error.",
                        data = (object)null,
                        errorCode = "503"
                    };

                    context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                    return; // stop pipeline
                }

                // Continue to next middleware if DB is connected
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "⚠️ Unexpected error while checking DB connection.");

                var response = new
                {
                    success = false,
                    message = "Unexpected server error.",
                    data = (object)null,
                    errorCode = "500"
                };
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
