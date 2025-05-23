namespace VelikiyPrikalel.OLLAMACHAT.Data;

/// <summary>
/// Роль автора сообщения.
/// </summary>
public enum ChatMessageRole
{
    /// <summary>
    /// Пользователь (человек)
    /// </summary>
    User = 0,

    /// <summary>
    /// Чат-бот (машина)
    /// </summary>
    Assistant = 1,
}
