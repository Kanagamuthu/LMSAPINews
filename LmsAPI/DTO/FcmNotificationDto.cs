namespace LMSAPI.DTO
{
    public class FcmNotificationDto
    {
        public string DeviceToken { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public object? Data { get; set; }

    }
}
