namespace VelikiyPrikalel.OLLAMACHAT.Application.Models;

/// <summary>
/// Представляет ошибку парсинга.
/// </summary>
/// <param name="Message">Сообщение об ошибке.</param>
/// <param name="Severity">Серьезность ошибки.</param>
/// <param name="Location">Расположение ошибки в коде.</param>
public record ParseError(string? Message, ParseErrorSeverity? Severity, Location? Location);