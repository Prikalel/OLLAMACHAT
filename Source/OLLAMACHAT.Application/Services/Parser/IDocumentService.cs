namespace VelikiyPrikalel.OLLAMACHAT.Application.Services.Parser;

/// <summary>
/// Сервис документов.
/// </summary>
public interface IDocumentService
{
    /// <summary>
    /// Получить документ.
    /// </summary>
    /// <param name="filePath">Путь до файла cs.</param>
    /// <returns>Документ roslyn.</returns>
    Task<Document?> GetDocumentAsync(string filePath);

    /// <summary>
    /// Посчитать hash документа.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <returns>Hash сумма.</returns>
    Task<string> GetContentHashAsync(Document document);
}
