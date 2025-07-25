namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Repositories;

/// <inheritdoc />
public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
{
    private readonly OllamaChatContext context;
    private readonly DbSet<TEntity> dbSet;

    /// <inheritdoc />
    public Repository(OllamaChatContext context)
    {
        this.context = context;
        dbSet = this.context.Set<TEntity>();
    }

    /// <inheritdoc />
    public async Task<User> GetOrCreateUser(string name)
    {
        if (await context.Users.AnyAsync())
        {
            return context.Users
                .Include(x => x.Chats)
                .ThenInclude(x => x.Messages)
                .FirstOrDefault(x => x.Name == name);
        }

        EntityEntry<User> entity = await context.Users.AddAsync(new User { Id = null, Name = name });
        await context.SaveChangesAsync();
        return entity.Entity;
    }

    /// <inheritdoc />
    public async Task<UserChat?> GetChatByIdAsync(string id) =>
        await context.UserChats
            .Include(x => x.Messages)
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();

    /// <inheritdoc />
    public async Task<UserChat?> GetChatWithJobId(string JobId) =>
        await context.UserChats
            .Include(x => x.Messages)
            .Where(x => x.EnqueuedCompletionJobId == JobId)
            .FirstOrDefaultAsync();

    /// <inheritdoc />
    public async Task UpdateAsync(TEntity entity)
    {
        dbSet.Update(entity);
        await context.SaveChangesAsync();
    }
}