using ChatMessage = VelikiyPrikalel.OLLAMACHAT.Data.Llm.ChatMessage;

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

    /// <summary>
    /// События Unity.
    /// </summary>
    public DbSet<UnityEvent> UnityEvents { get; set; }

    /// <summary>
    /// Обработчики событий.
    /// </summary>
    public DbSet<EventHandler> EventHandlers { get; set; }

    /// <summary>
    /// Связи между событиями и обработчиками.
    /// </summary>
    public DbSet<EventHandlerRelationship> EventHandlerRelationships { get; set; }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Применение конфигураций для существующих сущностей
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new UserChatConfiguration());
        modelBuilder.ApplyConfiguration(new ChatMessageConfiguration());

        // Применение конфигураций для новых сущностей Unity
        modelBuilder.ApplyConfiguration(new UnityEventConfiguration());
        modelBuilder.ApplyConfiguration(new EventHandlerConfiguration());
        modelBuilder.ApplyConfiguration(new EventHandlerRelationshipConfiguration());
    }
}
