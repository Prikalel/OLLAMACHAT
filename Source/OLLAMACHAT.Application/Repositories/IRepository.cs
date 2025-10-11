namespace VelikiyPrikalel.OLLAMACHAT.Application.Repositories;

/// <summary>
/// Общий интерфейс репозитория для сущностей IEntity.
/// </summary>
/// <typeparam name="TEntity">Тип сущности, реализующей IEntity.</typeparam>
public interface IRepository<TEntity> where TEntity : IEntity
{
    /// <summary>
    /// Добавить, но не сохранять.
    /// </summary>
    /// <param name="entity">Сущность.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task AddAsync(TEntity entity);

    /// <summary>
    /// Обновить существующую сущность.
    /// </summary>
    Task SaveChanges();

    /// <summary>
    /// Получить сущности.
    /// </summary>
    /// <param name="predicate">Предикат.</param>
    /// <param name="asNoTracking">Выключить отслеживание.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task<IReadOnlyList<TEntity>> GetMany(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false);

    /// <summary>
    /// Получить сущность.
    /// Если по предикату их получилось больше 1, то будет исключение.
    /// </summary>
    /// <param name="predicate">Предикат.</param>
    /// <param name="asNoTracking">Выключить отслеживание.</param>
    /// <returns>Сущность или null если не найдена.</returns>
    Task<TEntity?> GetSingle(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false);
}
