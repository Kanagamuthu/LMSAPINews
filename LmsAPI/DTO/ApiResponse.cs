namespace LMSAPI.DTO
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
        public string? ErrorCode { get; set; }

        public ApiResponse() { }

        public ApiResponse(bool success, string message, object? data = null, string? errorCode = null)
        {
            Success = success;
            Message = message;
            Data = data;
            ErrorCode = errorCode;
        }
        // ✅ Helper methods for cleaner controller usage
        public static ApiResponse Ok(object? data = null, string message = "Success") => new ApiResponse(true, message, data);
        public static ApiResponse Fail(string message, string? errorCode = null) => new ApiResponse(false, message, null, errorCode);
    }
}
