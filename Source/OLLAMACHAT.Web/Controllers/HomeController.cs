namespace VelikiyPrikalel.OLLAMACHAT.Web.Controllers;

using Index = VelikiyPrikalel.OLLAMACHAT.Web.Views.Home.Index;

/// <summary>
/// index контроллер.
/// </summary>
[Route("/")]
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
        return View(new Index());
    }
}