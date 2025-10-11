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
        IRepository<ChatMessage> messageRepository,
        ILogger<Handler> logger) : IRequestHandler<Command>
    {
        /// <inheritdoc />
        public async ValueTask<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            (User user, UserChat activeChat) = await GetUserActiveChat();

            if (request.Message.Trim().ToLower().Equals("/undo"))
            {
                logger.LogInformation("Will delete last message");
                activeChat.DeleteLastMessage();
                await userRepository.SaveChanges();
                return new();
            }

            logger.LogInformation("Will generate llm response from model {Model}", activeChat.Model);

            activeChat.UserEnteredPrompt(request.Message);
            await userRepository.SaveChanges();

            try
            {
                await ExecuteLlmRequest(request, activeChat);
                foreach (ChatMessage message in activeChat.Messages.TakeLast(2))
                {
                    await messageRepository.AddAsync(message);
                }

                await userRepository.SaveChanges();
            }
            catch (Exception)
            {
                if ((await GetUserActiveChat()).Chat is { State: ChatState.WaitingMessageGeneration } chat)
                {
                    chat.GenerationFailed();
                    await userRepository.SaveChanges();
                }

                throw;
            }

            return new();
        }

        private async ValueTask ExecuteLlmRequest(Command request, UserChat activeChat)
        {
            string fullResponse = await llmBackgroundService.GenerateTextResponse(
                request.ConnectionId,
                request.Message,
                activeChat
            );
            activeChat.LlmReturnedResponse(request.Message, fullResponse);
        }

        private async ValueTask<(User User, UserChat Chat)> GetUserActiveChat()
        {
            User user = await userRepository.GetOrCreateUser("alex");
            UserChat activeChat = await userRepository.GetOrCreateActiveChat(user, string.Empty);
            return (user, activeChat);
        }
    }
}
