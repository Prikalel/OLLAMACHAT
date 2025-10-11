namespace VelikiyPrikalel.OLLAMACHAT.Application.Services.Parser;

/// <summary>
/// Фасад для парсера.
/// </summary>
public interface IRoslynParsingService
{
    /// <summary>
    /// Выполнить парсинг файла.
    /// </summary>
    /// <param name="filePath">Файл.</param>
    /// <param name="options">Опции парсинга.</param>
    /// <returns>Результат парсинга.</returns>
    Task<ParseResult> ParseFileAsync(string filePath, ParserOptions? options);

    /// <summary>
    /// Зарезолвить путь using выражения.
    /// </summary>
    /// <param name="importPath">using выражение.</param>
    /// <param name="filePath">Путь до файла где объявлено.</param>
    /// <returns>Список импортированных файлов.</returns>
    Task<List<string>> ResolveImportPathAsync(string importPath, string filePath);
}
