namespace VelikiyPrikalel.OLLAMACHAT.Web.Controllers;

using Index = VelikiyPrikalel.OLLAMACHAT.Web.Views.Home.Index;

/// <summary>
/// index контроллер.
/// </summary>
[Route("/")]
[ApiExplorerSettings(IgnoreApi = true)] // убираем из swagger
public class HomeController(ILogger<HomeController> logger) : Controller
{
    /// <summary>
    /// Главная страница.
    /// </summary>
    /// <returns><see cref="ViewResult"/>.</returns>
    [Route("/")]
    public IActionResult Index()
    {
        logger.LogInformation("Index requested");
        return View(new Index()
        {
            AvailableModels =
            [
                "deepseek-v3",
                "gpt-4.1",
                "bidara",
                "deepseek-r1",
                "mirexa",
                "sur",
                "gpt-4.1-mini"
            ] // TODO. move to other place
        });
    }
}