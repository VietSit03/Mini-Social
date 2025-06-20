using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MiniSocialAPI.MiniSocialAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private readonly IConfiguration _config;

        public TestController(ILogger<TestController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }


        [HttpGet]
        [Route("/checklog")]
        public IActionResult CheckLog()
        {
            _config.GetSection("JWT:Issuer");
            _logger.LogInformation($"TestController check log - {DateTime.UtcNow}");
            return Ok(new { message = "Done!" });
        }
    }
}
