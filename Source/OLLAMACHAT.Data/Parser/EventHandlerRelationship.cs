namespace VelikiyPrikalel.OLLAMACHAT.Data.Parser;

/// <summary>
/// Связующая сущность между событиями и обработчиками.
/// </summary>
public class EventHandlerRelationship : IEntity
{
    /// <summary>
    /// Уникальный идентификатор связи.
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public required string Id { get; set; }

    /// <summary>
    /// Идентификатор события.
    /// </summary>
    public required string EventId { get; set; }

    /// <summary>
    /// Идентификатор обработчика.
    /// </summary>
    public required string HandlerId { get; set; }

    /// <summary>
    /// Тип связи (Potential, Confirmed, Obsolete).
    /// </summary>
    public required EventHandlerRelationshipType EventHandlerRelationshipType { get; set; }

    /// <summary>
    /// Источник связи (Parser, Inspector, Code, Manual).
    /// </summary>
    public required Source Source { get; set; }

    /// <summary>
    /// Причина удаления (если связь устарела).
    /// </summary>
    public DeletionReason? DeletionReason { get; set; }

    /// <summary>
    /// Время создания связи.
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Время обновления связи.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; private set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Флаг удаления для мягкого удаления.
    /// </summary>
    public required bool IsDeleted { get; set; }

    /// <summary>
    /// Навигационное свойство для связанного события.
    /// </summary>
    public UnityEvent? Event { get; set; }

    /// <summary>
    /// Навигационное свойство для связанного обработчика.
    /// </summary>
    public EventHandler? Handler { get; set; }

    /// <summary>
    /// Обновить время последнего изменения.
    /// </summary>
    public void UpdateLastModified()
    {
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Пометить как удаленное с указанием причины (мягкое удаление).
    /// </summary>
    /// <param name="reason">Причина удаления.</param>
    public void MarkAsDeleted(DeletionReason reason)
    {
        IsDeleted = true;
        DeletionReason = reason;
        EventHandlerRelationshipType = EventHandlerRelationshipType.Obsolete;
        UpdateLastModified();
    }

    /// <summary>
    /// Изменить тип связи.
    /// </summary>
    /// <param name="newType">Новый тип связи.</param>
    public void ChangeRelationshipType(EventHandlerRelationshipType newType)
    {
        EventHandlerRelationshipType = newType;
        UpdateLastModified();
    }
}
