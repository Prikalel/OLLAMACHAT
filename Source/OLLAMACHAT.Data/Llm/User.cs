namespace VelikiyPrikalel.OLLAMACHAT.Data.Llm;

/// <summary>
/// Пользователь сервиса.
/// </summary>
public class User : IEntity
{
    /// <summary>
    /// Id.
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public required string Id { get; set; }

    /// <summary>
    /// Имя.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Чаты пользователя.
    /// </summary>
    public ICollection<UserChat> Chats { get; } = [];
}
