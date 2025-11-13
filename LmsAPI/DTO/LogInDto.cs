namespace LMSAPI.DTO
{
    public class LogInDto
    {
        public string EmailId { get; set; } = string.Empty;
        public string DeviceMac { get; set; } = string.Empty;
        public string OTP { get; set; } = string.Empty;
    }
}
