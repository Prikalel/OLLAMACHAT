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
    public async Task UpdateAsync(TEntity entity)
    {
        dbSet.Update(entity);
        await context.SaveChangesAsync();
    }
}
