namespace VelikiyPrikalel.OLLAMACHAT.Application.Extensions;

/// <summary>
/// Методы расширения.
/// </summary>
public static class EntitiesExtensions
{
    /// <summary>
    /// Рекурсивно обходит все дочерние сущности и возвращает плоский список всех сущностей
    /// </summary>
    /// <param name="roots">Корневые сущности</param>
    /// <returns>Плоский список всех сущностей, включая дочерние</returns>
    public static List<ParsedEntity> FlattenAllTrees(this IEnumerable<ParsedEntity> roots) =>
        roots.SelectMany(root =>
            new[] { root }
                .Concat(root.Children?.SelectMany(x => FlattenAllTrees([x])) ?? [])
        ).ToList();
}
