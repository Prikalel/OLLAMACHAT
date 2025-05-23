using Microsoft.EntityFrameworkCore.ChangeTracking;

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
    public async Task<IEnumerable<TEntity>> GetAllAsync() => await dbSet.ToListAsync();

    /// <inheritdoc />
    public async Task<TEntity> GetByIdAsync(string id) => await dbSet.FindAsync(id);

    /// <inheritdoc />
    public async Task AddAsync(TEntity entity)
    {
        await dbSet.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task UpdateAsync(TEntity entity)
    {
        dbSet.Update(entity);
        await context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task DeleteAsync(string id)
    {
        TEntity entity = await GetByIdAsync(id);
        if (entity != null)
        {
            dbSet.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
}