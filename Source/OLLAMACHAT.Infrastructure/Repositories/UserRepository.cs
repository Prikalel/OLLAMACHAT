namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Repositories;

/// <summary>
/// Репозиторий <see cref="User"/>.
/// </summary>
public class UserRepository(OllamaChatContext context) : Repository<User>(context), IUserRepository
{
    /// <inheritdoc />
    public async Task<User> GetOrCreateUser(string name)
    {
        if (await context.Users.AnyAsync())
        {
            return context.Users
                .Include(x => x.Chats)
                .ThenInclude(x => x.Messages)
                .FirstOrDefault(x => x.Name == name);
        }

        EntityEntry<User> entity = await context.Users.AddAsync(new User { Id = null, Name = name });
        await context.SaveChangesAsync();
        return entity.Entity;
    }
}
