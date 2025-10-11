namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Repositories;

/// <summary>
/// Репозиторий <see cref="UserChat"/>.
/// </summary>
public class UserChatRepository(OllamaChatContext context) : Repository<UserChat>(context), IUserChatRepository
{
    /// <inheritdoc />
    public async Task<UserChat?> GetChatByIdAsync(string id) =>
        await Context.UserChats
            .Include(x => x.Messages)
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();
}
