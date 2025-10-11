namespace VelikiyPrikalel.OLLAMACHAT.Data.Parser;

/// <summary>
/// Сущность для хранения информации о UnityEvent в Unity проектах.
/// </summary>
public class UnityEvent : IEntity
{
    /// <summary>
    /// Уникальный идентификатор события.
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public required string Id { get; set; }

    /// <summary>
    /// Имя события.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Полное имя события включая пространство имен.
    /// </summary>
    public required string FullName { get; set; }

    /// <summary>
    /// Тип события (UnityEvent, UnityEvent{T}).
    /// </summary>
    public required string EventType { get; set; }

    /// <summary>
    /// Массив типов аргументов для UnityEvent{T}.
    /// </summary>
    public string[]? GenericTypeArguments { get; set; }

    /// <summary>
    /// Хэш от массива типов аргументов для быстрого поиска.
    /// </summary>
    public required string ArgumentsHash { get; set; }

    /// <summary>
    /// Путь к файлу, где определено событие (базовый класс).
    /// </summary>
    public required string FilePath { get; set; }

    /// <summary>
    /// True, если событие унаследовано от базового класса.
    /// </summary>
    public required bool IsInherited { get; set; }

    /// <summary>
    /// Полное имя базового класса, если унаследовано.
    /// </summary>
    public string? BaseClassFullName { get; set; }

    /// <summary>
    /// Время последнего изменения записи.
    /// </summary>
    public DateTimeOffset LastModified { get; private set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Флаг удаления для мягкого удаления.
    /// </summary>
    public required bool IsDeleted { get; set; }

    /// <summary>
    /// Связи с обработчиками событий.
    /// </summary>
    public ICollection<EventHandlerRelationship> HandlerRelationships { get; } = [];

    /// <summary>
    /// Обновить время последнего изменения.
    /// </summary>
    public void UpdateLastModified()
    {
        LastModified = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Пометить как удаленное (мягкое удаление).
    /// </summary>
    public void MarkAsDeleted()
    {
        IsDeleted = true;
        UpdateLastModified();
    }
}
