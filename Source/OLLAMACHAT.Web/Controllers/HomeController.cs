namespace VelikiyPrikalel.OLLAMACHAT.Web.Controllers;

using Index = VelikiyPrikalel.OLLAMACHAT.Web.Views.Home.Index;

/// <summary>
/// index контроллер.
/// </summary>
[Route("/")]
[ApiExplorerSettings(IgnoreApi = true)] // убираем из swagger
public class HomeController(
    ILogger<HomeController> logger,
    IMediator mediator) : Controller
{
    /// <summary>
    /// Главная страница.
    /// </summary>
    /// <returns><see cref="ViewResult"/>.</returns>
    [Route("/")]
    public async Task<IActionResult> Index()
    {
        logger.LogInformation("Index requested");
        return View(new Index()
        {
            AvailableModels = (await mediator.Send(new GetAvailableModels.Query())).ToList()
        });
    }
}