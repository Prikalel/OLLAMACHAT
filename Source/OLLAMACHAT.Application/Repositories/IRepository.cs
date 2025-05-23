namespace VelikiyPrikalel.OLLAMACHAT.Application.Repositories;

/// <summary>
/// Общий интерфейс репозитория для сущностей IEntity.
/// </summary>
/// <typeparam name="TEntity">Тип сущности, реализующей IEntity.</typeparam>
public interface IRepository<TEntity> where TEntity : IEntity
{
    /// <summary>
    /// Получить пользователя.
    /// </summary>
    /// <param name="name">Имя.</param>
    /// <returns>id пользователя.</returns>
    Task<User> GetOrCreateUser(string name);

    /// <summary>
    /// Получить все сущности.
    /// </summary>
    /// <returns>Коллекция сущностей.</returns>
    Task<IEnumerable<TEntity>> GetAllAsync();

    /// <summary>
    /// Получить сущность по id.
    /// </summary>
    /// <param name="id">ID сущности.</param>
    /// <returns>Сущность или null, если не найдено.</returns>
    Task<TEntity> GetByIdAsync(string id);

    /// <summary>
    /// Добавить новую сущность.
    /// </summary>
    /// <param name="entity">Сущность для добавления.</param>
    Task AddAsync(TEntity entity);

    /// <summary>
    /// Обновить существующую сущность.
    /// </summary>
    /// <param name="entity">Обновленная сущность.</param>
    Task UpdateAsync(TEntity entity);

    /// <summary>
    /// Удалить сущность по id.
    /// </summary>
    /// <param name="id">ID сущности для удаления.</param>
    Task DeleteAsync(string id);
}