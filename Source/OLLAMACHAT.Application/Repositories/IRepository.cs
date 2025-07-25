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
    /// Получить чат по id.
    /// </summary>
    /// <param name="id">ID чата.</param>
    /// <returns>Сущность или null, если не найдено.</returns>
    Task<UserChat?> GetChatByIdAsync(string id);

    /// <summary>
    /// .
    /// </summary>
    /// <param name="JobId">.</param>
    /// <returns>.</returns>
    Task<UserChat?> GetChatWithJobId(string JobId);

    /// <summary>
    /// Обновить существующую сущность.
    /// </summary>
    /// <param name="entity">Обновленная сущность.</param>
    Task UpdateAsync(TEntity entity);
}