namespace VelikiyPrikalel.OLLAMACHAT.Application.Repositories;

/// <summary>
/// Общий интерфейс репозитория для сущностей IEntity.
/// </summary>
/// <typeparam name="TEntity">Тип сущности, реализующей IEntity.</typeparam>
public interface IRepository<TEntity> where TEntity : IEntity
{
    /// <summary>
    /// Обновить существующую сущность.
    /// </summary>
    /// <param name="entity">Обновленная сущность.</param>
    Task UpdateAsync(TEntity entity);
}
