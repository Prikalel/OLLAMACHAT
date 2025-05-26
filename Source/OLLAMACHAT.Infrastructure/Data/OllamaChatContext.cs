namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Data;

/// <summary>
/// Контекст БД.
/// </summary>
public class OllamaChatContext : DbContext
{
    /// <summary>
    /// ctor.
    /// </summary>
    /// <param name="options"><see cref="DbContextOptions"/>.</param>
    public OllamaChatContext(DbContextOptions<OllamaChatContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Пользователи.
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// Чаты пользователей.
    /// </summary>
    public DbSet<UserChat> UserChats { get; set; }

    /// <summary>
    /// Сообщения чатов.
    /// </summary>
    public DbSet<ChatMessage> Messages { get; set; }
}