namespace LMSAPI.DTO
{
    public class OtpRegenerateDto
    {
        public string? EmailId { get; set; }
        public string? Otp { get; set; } // e.g., "Email", "SMS"
    }
}
