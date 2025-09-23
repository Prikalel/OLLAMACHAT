namespace VelikiyPrikalel.OLLAMACHAT.Application.Models;

/// <summary>
/// Представляет запрос на разрешение импорта.
/// </summary>
/// <param name="ImportPath">Путь импорта/using для разрешения.</param>
/// <param name="FilePath">Путь к файлу, содержащему импорт.</param>
/// <param name="RepoPath">Абсолютный путь к корню репозитория.</param>
public record ResolveImportRequest(string ImportPath, string FilePath, string RepoPath);