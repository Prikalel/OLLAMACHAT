namespace VelikiyPrikalel.OLLAMACHAT.Application.Repositories;

/// <summary>
/// Репозиторий <see cref="User"/>.
/// </summary>
public interface IUserRepository : IRepository<User>
{
    /// <summary>
    /// Получить пользователя.
    /// </summary>
    /// <param name="name">Имя.</param>
    /// <returns>id пользователя.</returns>
    Task<User> GetOrCreateUser(string name);
}
