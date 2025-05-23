namespace VelikiyPrikalel.OLLAMACHAT.Web.Controllers;

/// <summary>
/// Api контроллер для hello world.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class HelloController(ILogger<HelloController> logger) : Controller
{
    [HttpGet("hello")]
    public string SayHello()
    {
        logger.LogInformation("hello world");
        return "hello world!";
    }
}