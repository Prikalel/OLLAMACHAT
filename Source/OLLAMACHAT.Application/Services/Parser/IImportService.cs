namespace VelikiyPrikalel.OLLAMACHAT.Application.Services.Parser;

/// <summary>
/// Сервис получения путей до файлов по using выражению.
/// </summary>
public interface IImportService
{
    /// <summary>
    /// Резолвинг пути.
    /// </summary>
    /// <param name="importPath">Строка с using statement.</param>
    /// <param name="document">Документ со строкой.</param>
    /// <returns>Список путей до файлов, которые были импортированы.</returns>
    Task<List<string>> ResolveImportPathAsync(string importPath, Document document);

    /// <summary>
    /// Получить csproj и GlobalUsings файлы.
    /// </summary>
    /// <param name="repoPath">Путь до папки с решением.</param>
    /// <returns>Список абсолютных путей файлов.</returns>
    Task<List<string>> FindCommonInitFilesAsync(string repoPath);
}
