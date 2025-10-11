namespace VelikiyPrikalel.OLLAMACHAT.Application.Mediator;

/// <summary>
/// Отправка промта llm.
/// </summary>
public sealed class SendMessage
{
    /// <summary>
    /// Команда на выполнение llm генерации.
    /// </summary>
    /// <param name="ConnectionId">Идентификатор соединения signalR.</param>
    /// <param name="Message">Строка от пользователя.</param>
    public sealed record Command(string ConnectionId, string Message) : IRequest;

    /// <inheritdoc />
    public sealed class Handler(
        ILlmBackgroundService llmBackgroundService,
        IUserRepository userRepository,
        ILogger<Handler> logger) : IRequestHandler<Command>
    {
        /// <inheritdoc />
        public async ValueTask<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            User user = await userRepository.GetOrCreateUser("alex");
            UserChat activeChat = user.GetOrCreateActiveChat(string.Empty, out bool _);

            if (request.Message.Trim().ToLower().Equals("/undo"))
            {
                logger.LogInformation("Will delete last message");
                activeChat.DeleteLastMessage();
                await userRepository.UpdateAsync(user);
            }
            else
            {
                logger.LogInformation("Will generate llm response from model {Model}", activeChat.Model);

                activeChat.UserEnteredPrompt(request.Message);
                await userRepository.UpdateAsync(user);

                try
                {
                    await llmBackgroundService.GenerateTextResponse(
                        request.ConnectionId,
                        request.Message,
                        activeChat.Model,
                        activeChat.Id,
                        activeChat.Messages
                    );
                }
                catch (Exception)
                {
                    if (await userRepository.GetOrCreateUser("alex") is { } user2
                        && user.GetOrCreateActiveChat(string.Empty, out bool _) is { State: ChatState.WaitingMessageGeneration } chat)
                    {
                        chat.GenerationFailed();
                        await userRepository.UpdateAsync(user2);
                    }

                    throw;
                }
            }

            return new();
        }
    }
}
