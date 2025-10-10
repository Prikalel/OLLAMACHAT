namespace VelikiyPrikalel.OLLAMACHAT.Application.Models;

/// <summary>
/// Определяет тип отношения между сущностями.
/// </summary>
public enum RelationshipType
{
    /// <summary>
    /// Вызов метода.
    /// </summary>
    Calls,

    /// <summary>
    /// Является базовым для.
    /// </summary>
    IsBaseFor,

    /// <summary>
    /// Подписывается на.
    /// </summary>
    SubscribesTo,

    /// <summary>
    /// Наблюдается.
    /// </summary>
    ObservedBy
}

/// <summary>
/// То же самое что и <see cref="Relationship"/> только укороченная.
/// </summary>
public enum SimpleRelationshipType
{
    /// <summary>
    /// Вызов метода.
    /// </summary>
    Calls,

    /// <summary>
    /// Является базовым для.
    /// </summary>
    IsBaseFor
}
