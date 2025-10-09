namespace VelikiyPrikalel.OLLAMACHAT.Application.Repositories;

/// <summary>
/// Репозиторий <see cref="UserChat"/>.
/// </summary>
public interface IUserChatRepository : IRepository<UserChat>
{
    /// <summary>
    /// Получить чат по id.
    /// </summary>
    /// <param name="id">ID чата.</param>
    /// <returns>Сущность или null, если не найдено.</returns>
    Task<UserChat?> GetChatByIdAsync(string id);
}
