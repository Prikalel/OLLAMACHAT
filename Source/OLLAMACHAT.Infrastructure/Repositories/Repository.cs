namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Repositories;

/// <inheritdoc />
public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
{
    /// <summary>
    /// Контекст.
    /// </summary>
    protected readonly OllamaChatContext Context;

    /// <summary>
    /// Сет.
    /// </summary>
    protected readonly DbSet<TEntity> DbSet;

    /// <inheritdoc />
    public Repository(OllamaChatContext context)
    {
        this.Context = context;
        DbSet = this.Context.Set<TEntity>();
    }

    /// <inheritdoc />
    public async Task AddAsync(TEntity entity) => await DbSet.AddAsync(entity);

    /// <inheritdoc />
    public async Task SaveChanges() => await Context.SaveChangesAsync();

    /// <inheritdoc />
    public async Task<IReadOnlyList<TEntity>> GetMany(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false) =>
        asNoTracking
            ? await DbSet
                .AsNoTracking()
                .Where(predicate)
                .ToListAsync()
            : await DbSet
                .Where(predicate)
                .ToListAsync();

    /// <inheritdoc />
    public async Task<TEntity?> GetSingle(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false) =>
        asNoTracking
            ? await DbSet
                .AsNoTracking()
                .Where(predicate)
                .SingleOrDefaultAsync()
            : await DbSet
                .Where(predicate)
                .SingleOrDefaultAsync();
}
