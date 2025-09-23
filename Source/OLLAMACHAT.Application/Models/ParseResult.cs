namespace VelikiyPrikalel.OLLAMACHAT.Application.Models;

/// <summary>
/// Представляет полный результат парсинга одного файла.
/// </summary>
/// <param name="FilePath">Путь к файлу.</param>
/// <param name="Language">Язык программирования.</param>
/// <param name="Entities">Список сущностей.</param>
/// <param name="Relationships">Список отношений.</param>
/// <param name="ContentHash">Хэш содержимого файла для кэширования.</param>
/// <param name="ParseTimeMs">Время парсинга в миллисекундах.</param>
/// <param name="Errors">Список ошибок.</param>
/// <param name="Metadata">Метаданные файла.</param>
public record ParseResult(
    string FilePath,
    ParseResultLanguage Language,
    List<ParsedEntity> Entities,
    List<Relationship>? Relationships,
    string ContentHash,
    int? ParseTimeMs,
    List<ParseError>? Errors,
    FileMetadata? Metadata
);