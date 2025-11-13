namespace LMSAPI.DTO
{
    public class OTPVerificationDto
    {
        public string? EmailId { get; set; }
        public string? deviceMacId { get; set; }
        public string? Otp { get; set; }
    }
}
