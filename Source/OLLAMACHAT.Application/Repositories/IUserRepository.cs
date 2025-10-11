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

    /// <summary>
    /// Получить активный чат пользователя или создать его, если
    /// у пользователя не было чатов.
    /// </summary>
    /// <param name="user">Юзер.</param>
    /// <param name="defaultModel">Модель чата при создании.</param>
    /// <returns><see cref="UserChat"/>.</returns>
    Task<UserChat> GetOrCreateActiveChat(User user, string defaultModel);
}
