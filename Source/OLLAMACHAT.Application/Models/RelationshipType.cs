namespace VelikiyPrikalel.OLLAMACHAT.Application.Models;

/// <summary>
/// Определяет тип отношения между сущностями.
/// </summary>
public enum RelationshipType
{
    /// <summary>
    /// Наследование.
    /// </summary>
    Inherits,

    /// <summary>
    /// Реализация интерфейса.
    /// </summary>
    Implements,

    /// <summary>
    /// Вызов метода.
    /// </summary>
    Calls,

    /// <summary>
    /// Ссылка.
    /// </summary>
    References
}