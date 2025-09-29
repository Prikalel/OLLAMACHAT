namespace VelikiyPrikalel.OLLAMACHAT.Application.Models;

/// <summary>
/// Определяет тип сущности в коде.
/// </summary>
public enum ParsedEntityType
{
    /// <summary>
    /// Класс.
    /// </summary>
    Class,

    /// <summary>
    /// Метод.
    /// </summary>
    Method,

    /// <summary>
    /// Интерфейс.
    /// </summary>
    Interface,

    /// <summary>
    /// Using-директива.
    /// </summary>
    UsingStatement,

    /// <summary>
    /// Свойство или поле.
    /// </summary>
    Property,

    /// <summary>
    /// Перечисление.
    /// </summary>
    Enum,

    /// <summary>
    /// Структура.
    /// </summary>
    Struct,

    /// <summary>
    /// Пространство имен.
    /// </summary>
    Namespace,

    /// <summary>
    /// Unity-событие.
    /// </summary>
    UnityEvent
}