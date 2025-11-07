using LMSAPI.DTO;
using LMSAPI.Helpers;
using LMSAPI.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace LMSAPI.Controllers
{
    [ApiController]
    [TypeFilter(typeof(ExceptionFilter))]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class BasicConfigurationController : Controller
    {
        private readonly string _serverKey = "YOUR_FCM_SERVER_KEY_HERE"; // from Firebase console
        private readonly string _fcmUrl = "https://fcm.googleapis.com/fcm/send";
        private readonly ILoggerManager _logger;
        private readonly IMeUserRepository _meUserRepository;

        public BasicConfigurationController(ILoggerManager logger,IMeUserRepository meUserRepository)
        {
                
            _logger = logger;
            _meUserRepository = meUserRepository;
        }

        [HttpGet("GetFlags")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFlags()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://restcountries.com/v3.1/all?fields=name,flags,idd");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var Result = await response.Content.ReadAsStringAsync();
            return Ok(Result);
        }

        #region send notification
        [HttpPost("SendNotification")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SendNotification([FromBody] FcmNotificationDto model)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            int _userid = Convert.ToInt32(userId);

            if (string.IsNullOrEmpty(model.DeviceToken))
                return BadRequest("DeviceToken is required");

            using var client = new HttpClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"key={_serverKey}");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

            var payload = new
            {
                to = model.DeviceToken,
                notification = new
                {
                    title = model.Title,
                    body = model.Body,
                    sound = "default" // works on Android & iOS
                },
                data = model.Data // optional custom payload
            };
            //save notification to db
           var notificationRecord = new LMSAPI.Models.TblUserNotificationDetail
            {
                UserId = _userid,
                Isread = 0,
                NotificationOn = DateTime.UtcNow,
                // Assuming NotificationId is generated elsewhere or is not required here
            };


            await _meUserRepository.AddNotificationRecordAsync(notificationRecord);

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(_fcmUrl, content);
            var result = await response.Content.ReadAsStringAsync();

            return Ok(result);
        }
        #endregion

        #region notification api
        [HttpGet("GetNotifications")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetNotifications()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://fcm.googleapis.com/fcm/send/eJ8bX1fQ3Yw:APA91bH4cX2nYI0cwz4x3Wn1qX5nO1kz3k5x6Y7Z8A9B0C1D2E3F4G5H6I7J8K9L0M1N2O3P4Q5R6S7T8U9V0W1X2Y3Z4");
            request.Headers.Add("Authorization", "key=AAAA2xXyZzY:APA91bH4cX2nYI0cwz4x3Wn1qX5nO1kz3k5x6Y7Z8A9B0C1D2E3F4G5H6I7J8K9L0M1N2O3P4Q5R6S7T8U9V0W1X2Y3Z4");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var Result = await response.Content.ReadAsStringAsync();
            return Ok(Result);
        }
        #endregion
    }
}
