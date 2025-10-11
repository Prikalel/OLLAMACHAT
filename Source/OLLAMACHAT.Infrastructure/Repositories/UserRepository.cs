namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Repositories;

/// <summary>
/// Репозиторий <see cref="User"/>.
/// </summary>
public class UserRepository(OllamaChatContext context) : Repository<User>(context), IUserRepository
{
    /// <inheritdoc />
    public async Task<User> GetOrCreateUser(string name)
    {
        if (await Context.Users.AnyAsync())
        {
            return Context.Users
                .Include(x => x.Chats)
                .ThenInclude(x => x.Messages)
                .First(x => x.Name == name);
        }

        EntityEntry<User> entity = await Context.Users.AddAsync(new User { Id = Guid.NewGuid().ToString(), Name = name });
        await Context.SaveChangesAsync();
        return entity.Entity;
    }
}
