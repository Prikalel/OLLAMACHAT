namespace VelikiyPrikalel.OLLAMACHAT.Application.Hubs;

/// <inheritdoc />
public class ChatHub(IMediator mediator, ILogger<ChatHub> logger, IUserRepository userRepository) : Hub
{
    /// <summary>
    /// Изменить модель взаимодействия с сервером.
    /// </summary>
    /// <param name="model"></param>
    public async Task ChangeModel(string model)
    {
        User u = await userRepository.GetOrCreateUser("alex");
        UserChat chat = await userRepository.GetOrCreateActiveChat(u, model);
        chat.UpdateModel(model);
        await userRepository.SaveChanges();
        logger.LogInformation("Model changed to {M}", model);
    }

    /// <summary>
    /// Отправить сообщение в LLm.
    /// </summary>
    /// <param name="prompt">Промпт.</param>
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
