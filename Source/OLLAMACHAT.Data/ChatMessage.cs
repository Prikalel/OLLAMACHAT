namespace VelikiyPrikalel.OLLAMACHAT.Data;

/// <summary>
/// Сообщение чата.
/// </summary>
public class ChatMessage
{
    /// <summary>
    /// Id сообщения.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Id чата.
    /// </summary>
    public required string ChatId { get; set; }

    /// <summary>
    /// Роль автора сообщения.
    /// </summary>
    public required ChatMessageRole Role { get; set; }

    /// <summary>
    /// Содержимое сообщения (markdown).
    /// </summary>
    public required string Content { get; set; }

    /// <summary>
    /// Время появления сообщения.
    /// </summary>
    public DateTimeOffset Time { get; private set; } = DateTimeOffset.Now;
}
