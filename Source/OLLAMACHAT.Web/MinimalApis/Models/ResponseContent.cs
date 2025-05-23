namespace VelikiyPrikalel.OLLAMACHAT.Web.MinimalApis.Models;

/// <summary>
/// Ответ чат-бота.
/// </summary>
/// <param name="Type">Тип ("image"/"message").</param> // TODO: использовать enum
/// <param name="Content">Содержимое. Если картинка - её название.</param>
public record ResponseContent(string Type, string Content);