using Microsoft.AspNetCore.Mvc;

namespace LMSAPI.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class BasicConfigurationController : Controller
    {
        [HttpGet("GetFlags")]
        public async Task<IActionResult> GetFlags()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://restcountries.com/v3.1/all?fields=name,flags,idd");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var Result = await response.Content.ReadAsStringAsync();
            return Ok(Result);
        }
    }
}
