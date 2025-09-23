namespace VelikiyPrikalel.OLLAMACHAT.Application.Models;

/// <summary>
/// Представляет запрос на парсинг C# файла.
/// </summary>
/// <param name="FilePath">Путь к C# файлу для парсинга, относительно корня репозитория.</param>
/// <param name="RepoPath">Абсолютный путь к корню репозитория для разрешения контекста.</param>
/// <param name="Options">Опции парсера.</param>
public record ParserRequest(string FilePath, string RepoPath, ParserOptions? Options);