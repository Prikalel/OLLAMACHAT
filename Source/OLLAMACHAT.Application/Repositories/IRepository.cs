namespace VelikiyPrikalel.OLLAMACHAT.Application.Repositories;

/// <summary>
/// Общий интерфейс репозитория для сущностей IEntity.
/// </summary>
/// <typeparam name="TEntity">Тип сущности, реализующей IEntity.</typeparam>
public interface IRepository<TEntity> where TEntity : IEntity
{
    /// <summary>
    /// Добавить.
    /// </summary>
    /// <param name="entity">Сущность.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task AddAsync(TEntity entity);

    /// <summary>
    /// Обновить существующую сущность.
    /// </summary>
    /// <param name="entity">Обновленная сущность.</param>
    Task UpdateAsync(TEntity entity);

    /// <summary>
    /// Получить сущности.
    /// </summary>
    /// <param name="predicate">Предикат.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task<IReadOnlyList<TEntity>> GetMany(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Получить сущность.
    /// Если по предикату их получилось больше 1, то будет исключение.
    /// </summary>
    /// <param name="predicate">Предикат.</param>
    /// <returns>Сущность или null если не найдена.</returns>
    Task<TEntity?> GetSingle(Expression<Func<TEntity, bool>> predicate);
}
