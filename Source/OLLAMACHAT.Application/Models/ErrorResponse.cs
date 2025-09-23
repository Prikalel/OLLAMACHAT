namespace VelikiyPrikalel.OLLAMACHAT.Application.Models;

/// <summary>
/// Представляет ответ с ошибкой.
/// </summary>
/// <param name="Code">Код ошибки.</param>
/// <param name="Message">Сообщение об ошибке.</param>
/// <param name="Details">Детали ошибки.</param>
public record ErrorResponse(string Code, string Message, string? Details);