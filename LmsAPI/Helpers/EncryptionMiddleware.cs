using Azure;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LMSAPI.Helpers
{
    public class EncryptionMiddleware
    {
        private readonly RequestDelegate _next;

        public EncryptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Optional: only encrypt/decrypt when requested
            bool enableEncryption = context.Request.Headers.ContainsKey("X-Encrypted");
           
            if (!enableEncryption)
            {
                await _next(context);
                return;
            }
            var user = context.User;
            var userId = user.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var Key = "InfoplusInfoplusInfoplusInfoplus";
            string finalKey = $"{userId}{Key}";
            Key = finalKey.Length > 32 ? finalKey.Substring(0, 32) : finalKey;

            string IV = "";
            if (context.Request.Headers.TryGetValue("X-Timestamp", out var timestampHeader))
                IV = timestampHeader.ToString();


            var originalBodyStream = context.Response.Body;
            using var newResponseBody = new MemoryStream();
            context.Response.Body = newResponseBody;

            try
            {
                // 🔹 Handle request decryption
                if (context.Request.ContentLength > 0)
                {
                    context.Request.EnableBuffering();
                    using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                    string encryptedBody = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0;

                    if (!string.IsNullOrWhiteSpace(encryptedBody))
                    {
                        try
                        {
                            string decrypted = EncryptionHelper.Decrypt(encryptedBody, Key, IV);
                            byte[] bytes = Encoding.UTF8.GetBytes(decrypted);
                            context.Request.Body = new MemoryStream(bytes);
                        }
                        catch
                        {
                            // not encrypted, ignore
                        }
                    }
                }

                // 🔹 Process next middleware (controller, etc.)
                await _next(context);

                // 🔹 Encrypt the outgoing response
                newResponseBody.Seek(0, SeekOrigin.Begin);
                var plainResponse = await new StreamReader(newResponseBody).ReadToEndAsync();

                // 16 bytes = AES block size
                IV = DateTime.Now.ToString("yyyyMMddHHmmss") + "In";
                string encryptedResponse = EncryptionHelper.Encrypt(plainResponse, Key, IV);
                byte[] responseBytes = Encoding.UTF8.GetBytes(encryptedResponse);

                context.Response.ContentType = "application/json";
                context.Response.ContentLength = responseBytes.Length;
                context.Response.Headers.Append("X-Timestamp", IV);

                newResponseBody.Seek(0, SeekOrigin.Begin);
                await originalBodyStream.WriteAsync(responseBytes, 0, responseBytes.Length);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync($"{{\"error\": \"Encryption failed: {ex.Message}\"}}");
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }
    }
}
