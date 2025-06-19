using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MiniSocialAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }


        [HttpGet]
        [Route("/checklog")]
        public IActionResult CheckLog()
        {
            _logger.LogInformation($"TestController check log - {DateTime.UtcNow}");
            return Ok(new { message = "Done!" });
        }
    }
}
