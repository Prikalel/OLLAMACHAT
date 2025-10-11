namespace VelikiyPrikalel.OLLAMACHAT.Application.Services.Parser;

/// <summary>
/// Сервис получения ошибок парсинга.
/// </summary>
public interface IErrorService
{
    /// <summary>
    /// Получить ошибки парсинга.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <returns>Список ошибок.</returns>
    Task<IEnumerable<ParseError>> GetDiagnosticsAsync(Document document);
}
