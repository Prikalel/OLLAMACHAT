namespace VelikiyPrikalel.OLLAMACHAT.Application.Mediator;

/// <summary>
/// Отправка промта llm.
/// </summary>
public sealed class SendMessage
{
    /// <summary>
    /// Команда на выполнение llm генерации.
    /// </summary>
    /// <param name="Message">Строка от пользователя.</param>
    /// <returns>
    /// Возвращает id задачи HF.
    /// </returns>
    public sealed record Command(string ConnectionId, string Message) : IRequest<string>;

    /// <inheritdoc />
    public sealed class Handler(
        ILlmBackgroundService llmBackgroundService,
        IRepository<User> userRepository,
        ILogger<Handler> logger) : IRequestHandler<Command, string>
    {
        /// <inheritdoc />
        public async ValueTask<string> Handle(Command request, CancellationToken cancellationToken)
        {
            User user = await userRepository.GetOrCreateUser("alex");
            UserChat activeChat = user.GetOrCreateActiveChat(null, out bool _);

            if (request.Message.Trim().ToLower().Equals("/undo"))
            {
                logger.LogInformation("Will delete last message");
                activeChat.DeleteLastMessage();
                await userRepository.UpdateAsync(user);
            }
            else
            {
                logger.LogInformation("Will generate llm response from model {Model}", activeChat.Model);

                ChatState state = activeChat.UserEnteredPrompt(request.Message, null);
                await userRepository.UpdateAsync(user);

                await llmBackgroundService.GenerateTextResponse(
                    request.ConnectionId,
                    request.Message,
                    activeChat.Model,
                    activeChat.Id,
                    activeChat.Messages
                );
            }

            return null;
        }
    }
}
