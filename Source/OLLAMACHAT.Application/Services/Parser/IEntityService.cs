namespace VelikiyPrikalel.OLLAMACHAT.Application.Services.Parser;

/// <summary>
/// Сервис выгрузки объявленных сущностей из документа.
/// </summary>
public interface IEntityService
{
    /// <summary>
    /// Очистить кеш.
    /// </summary>
    void ClearCache();

    /// <summary>
    /// Выгрузить сущности.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="options">Опции выгрузки.</param>
    /// <returns>Дерево сущностей.</returns>
    Task<IEnumerable<ParsedEntity>> ExtractEntitiesAsync(Document document, ParserOptions? options = null);
}
