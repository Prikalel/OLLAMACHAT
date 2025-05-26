namespace VelikiyPrikalel.OLLAMACHAT.Web.MinimalApis.Models;

/// <summary>
/// Отправка сообщения чат-боту.
/// </summary>
/// <param name="Message">Сообщение.</param>
public record SendMessageRequestDto(string Message);
