using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VelikiyPrikalel.OLLAMACHAT.Web.Views.Home;

public class Index : PageModel
{
    public List<string> AvailableModels { get; set; }
    public List<ChatMessageDto> History { get; set; }
}
