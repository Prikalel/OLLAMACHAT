namespace VelikiyPrikalel.OLLAMACHAT.Application.Hubs;

public class ChatHub(IMediator mediator, ILogger<ChatHub> logger, IUserRepository userRepository, IUserChatRepository chatRepository) : Hub
{
    public async Task ChangeModel(string model)
    {
        User u = await userRepository.GetOrCreateUser("alex");
        UserChat chat = u.GetOrCreateActiveChat(null, out bool _);
        chat.UpdateModel(model);
        await chatRepository.UpdateAsync(chat);
        logger.LogInformation("Model changed to {M}", model);
    }

    public async Task SendMessage(string prompt)
    {
        try
        {
            await mediator.Send(new SendMessage.Command(
                Context.ConnectionId,
                prompt));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in SendMessage");
            throw;
        }
    }
}
