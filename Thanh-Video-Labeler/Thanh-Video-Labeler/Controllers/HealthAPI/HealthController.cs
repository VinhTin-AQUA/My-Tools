using Microsoft.AspNetCore.Mvc;

namespace Thanh_Video_Labeler.Controllers.HealthAPI
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class HealthController : ControllerBase
    {
        [HttpGet()]
        public IActionResult Ping()
        {
            return Ok(new { status = "OK", timestamp = DateTime.UtcNow });
        }
    }
}