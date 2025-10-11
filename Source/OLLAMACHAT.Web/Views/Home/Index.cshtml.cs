using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VelikiyPrikalel.OLLAMACHAT.Web.Views.Home;

/// <summary>
/// Модель для индекса.
/// </summary>
public class Index : PageModel
{
    /// <summary>
    /// Модели llm доступные для выбора.
    /// </summary>
    public required List<string> AvailableModels { get; set; }

    /// <summary>
    /// История переписки с чатом.
    /// </summary>
    public required List<ChatMessageDto> History { get; set; }
}
