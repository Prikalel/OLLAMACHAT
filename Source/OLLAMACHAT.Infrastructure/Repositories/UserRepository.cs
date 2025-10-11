namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Repositories;

/// <summary>
/// Репозиторий <see cref="User"/>.
/// </summary>
public class UserRepository : Repository<User>, IUserRepository
{
    /// <summary>
    /// Репозиторий <see cref="User"/>.
    /// </summary>
    public UserRepository(OllamaChatContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<UserChat> GetOrCreateActiveChat(User user, string defaultModel)
    {
        await Task.CompletedTask;
        if (!user.Chats.Any())
        {
            UserChat userChat = new(Guid.NewGuid().ToString(), user.Id, "Chat1", defaultModel, true, ChatState.PendingInput);
            user.Chats.Add(userChat);
            return userChat;
        }

        return user.Chats.SingleOrDefault(x => x.Active)
            ?? throw new InvalidOperationException("У пользователя должен существовать ровно 1 активный чат");
    }

    /// <inheritdoc />
    public async Task<User> GetOrCreateUser(string name)
    {
        if (await DbSet.AnyAsync())
        {
            return DbSet
                .Include(x => x.Chats)
                .ThenInclude(x => x.Messages)
                .First(x => x.Name == name);
        }

        User entity = new() { Id = Guid.NewGuid().ToString(), Name = name };
        await this.AddAsync(entity);
        return entity;
    }
}
