using Microsoft.AspNetCore.Mvc;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class TestsController : ControllerBase
{
    [HttpGet]
    public IActionResult RunTests() => Ok("API test successful: " + DateTime.UtcNow);
}
