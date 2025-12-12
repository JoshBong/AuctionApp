using Microsoft.AspNetCore.Mvc;

namespace AuctionApp.Controllers
{
    public class TestController : Controller
    {
        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Simple()
        {
            return Content(@"
<!DOCTYPE html>
<html>
<head><title>Test</title></head>
<body>
    <h1>Form Test</h1>
    <form method='post' action='/Test/Simple'>
        <input type='text' name='username' placeholder='Username' /><br/>
        <input type='password' name='password' placeholder='Password' /><br/>
        <button type='submit'>Submit</button>
    </form>
</body>
</html>", "text/html");
        }

        [HttpPost]
        public IActionResult Simple(string username, string password)
        {
            _logger.LogInformation("=== SIMPLE TEST POST ===");
            _logger.LogInformation($"Username: '{username}'");
            _logger.LogInformation($"Password: '{password}'");

            foreach (var key in Request.Form.Keys)
            {
                _logger.LogInformation($"Form['{key}'] = '{Request.Form[key]}'");
            }

            return Content($"Received: Username='{username}', Password='{password}'", "text/plain");
        }
    }
}