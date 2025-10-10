namespace VelikiyPrikalel.OLLAMACHAT.Data.Parser;

/// <summary>
/// Сущность для хранения информации о методах-обработчиках событий.
/// </summary>
public class EventHandler : IEntity
{
    /// <summary>
    /// Уникальный идентификатор обработчика.
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public required string Id { get; set; }

    /// <summary>
    /// Имя метода-обработчика.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Полное имя метода включая пространство имен.
    /// </summary>
    public required string FullName { get; set; }

    /// <summary>
    /// Массив типов параметров метода.
    /// </summary>
    public required string[] ParameterTypes { get; set; }

    /// <summary>
    /// Хэш от массива типов параметров для быстрого поиска.
    /// </summary>
    public required string ArgumentsHash { get; set; }

    /// <summary>
    /// Путь к файлу, где определен обработчик (базовый класс).
    /// </summary>
    public required string FilePath { get; set; }

    /// <summary>
    /// True, если обработчик унаследован от базового класса.
    /// </summary>
    public required bool IsInherited { get; set; }

    /// <summary>
    /// Полное имя базового класса, если унаследовано.
    /// </summary>
    public string? BaseClassFullName { get; set; }

    /// <summary>
    /// True, если метод публичный.
    /// </summary>
    public required bool IsPublic { get; set; }

    /// <summary>
    /// Время последнего изменения записи.
    /// </summary>
    public DateTimeOffset LastModified { get; private set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Флаг удаления для мягкого удаления.
    /// </summary>
    public required bool IsDeleted { get; set; }

    /// <summary>
    /// Связи с событиями.
    /// </summary>
    public ICollection<EventHandlerRelationship> EventRelationships { get; } = [];

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
