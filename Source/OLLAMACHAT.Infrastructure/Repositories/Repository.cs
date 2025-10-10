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
    public async Task<IReadOnlyList<TEntity>> GetMany(Expression<Func<TEntity, bool>> predicate) =>
        await dbSet
            .Where(predicate)
            .ToListAsync();

    /// <inheritdoc />
    public async Task<TEntity?> GetSingle(Expression<Func<TEntity, bool>> predicate) =>
        await dbSet
            .Where(predicate)
            .SingleOrDefaultAsync();
}
