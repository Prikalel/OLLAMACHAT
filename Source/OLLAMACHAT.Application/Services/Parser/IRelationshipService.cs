namespace VelikiyPrikalel.OLLAMACHAT.Application.Services.Parser;

/// <summary>
/// Сервис получения отношений между файлами <see cref="SimpleRelationship"/>.
/// </summary>
public interface IRelationshipService
{
    /// <summary>
    /// Инициализирует кеши типов и путей к файлам для оптимизации поиска
    /// </summary>
    /// <param name="solution">Решение.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task InitializeCaches(Solution solution);

    /// <summary>
    /// Проанализировать сущности на отношения между ними.
    /// Определяет отношения, определённые в <see cref="SimpleRelationshipType"/>.
    /// </summary>
    /// <param name="entities">Сущности (дерево).</param>
    /// <param name="document">Документ, в котором они объявлены.</param>
    /// <returns>Список отношений.</returns>
    Task<IEnumerable<SimpleRelationship>> AnalyzeRelationshipsAsync(IEnumerable<ParsedEntity> entities, Document document);
}
