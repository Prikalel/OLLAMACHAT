namespace VelikiyPrikalel.OLLAMACHAT.Data;

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

    /// <summary>
    /// Получить активный чат пользователя или создать его, если
    /// у пользователя не было чатов.
    /// </summary>
    /// <param name="defaultModel">Модель чата при создании.</param>
    /// <param name="created">TRUE если чат был создан.</param>
    /// <returns><see cref="UserChat"/>.</returns>
    public UserChat GetOrCreateActiveChat(string defaultModel, out bool created)
    {
        if (!Chats.Any())
        {
            Chats.Add(new(null, this.Id, "Chat1", defaultModel, true, ChatState.PendingInput));
            created = true;
        }
        else
        {
            created = false;
        }

        return Chats.SingleOrDefault(x => x.Active)
            ?? throw new InvalidOperationException("У пользователя должен существовать ровно 1 активный чат");
    }
}
