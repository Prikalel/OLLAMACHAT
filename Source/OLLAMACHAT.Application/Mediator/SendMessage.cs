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
    public sealed record Command(string Message) : IRequest<string>;

    /// <inheritdoc />
    public sealed class Handler(
        IRepository<User> userRepository,
        IBackgroundJobClientV2 backgroundJobClient,
        ILogger<Handler> logger) : IRequestHandler<Command, string>
    {
        /// <inheritdoc />
        public async ValueTask<string> Handle(Command request, CancellationToken cancellationToken)
        {
            User user = await userRepository.GetOrCreateUser("alex");
            UserChat activeChat = user.GetOrCreateActiveChat(null, out bool _);

            ChatState state = activeChat.UserEnteredPrompt(request.Message);
            await userRepository.UpdateAsync(user); // сохранили состояние чата

            logger.LogInformation("Will generate llm response from model {Model}", activeChat.Model);
            string jobId = backgroundJobClient.Enqueue<ILlmBackgroundService>(llmService => llmService.GenerateTextResponse(
                request.Message,
                activeChat.Model,
                activeChat.Id,
                activeChat.Messages));

            logger.LogInformation("Returning job id {Id}, chat {Id} current state {State}",
                jobId,
                activeChat.Id,
                state);

            return jobId;
        }
    }
}
