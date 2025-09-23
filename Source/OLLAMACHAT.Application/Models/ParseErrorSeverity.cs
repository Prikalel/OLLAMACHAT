namespace VelikiyPrikalel.OLLAMACHAT.Application.Models;

/// <summary>
/// Определяет серьезность ошибки парсинга.
/// </summary>
public enum ParseErrorSeverity
{
    /// <summary>
    /// Ошибка.
    /// </summary>
    Error,

    /// <summary>
    /// Предупреждение.
    /// </summary>
    Warning,

    /// <summary>
    /// Информация.
    /// </summary>
    Info
}