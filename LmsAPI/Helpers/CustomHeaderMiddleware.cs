namespace LMSAPI.Helpers
{
    public class CustomHeaderMiddleware
    {
        private readonly RequestDelegate _next;
        public CustomHeaderMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            // Read header
            var apiKey = context.Request.Headers["X-API-Key"].FirstOrDefault();

            if (string.IsNullOrEmpty(apiKey) || apiKey != "12345-ABCDE")
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid or missing API Key");
                return;
            }

            // Add response header
            context.Response.OnStarting(() =>
            {
                context.Response.Headers.Add("X-Security-Checked", "true");
                return Task.CompletedTask;
            });

            await _next(context);
        }
    }
}
