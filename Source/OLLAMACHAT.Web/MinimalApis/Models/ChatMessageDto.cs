namespace VelikiyPrikalel.OLLAMACHAT.Web.MinimalApis.Models;

/// <summary>
/// Сообщение чата.
/// </summary>
/// <param name="Role">Роль ("user"/"assistant").</param> // TODO: использовать enum
/// <param name="Content">Сообщение (перекодированное в html).</param>
public record ChatMessageDto(string Role, string Content);