namespace VelikiyPrikalel.OLLAMACHAT.Application.Mediator;

/// <summary>
/// Получить историю текущего чата пользователя.
/// </summary>
public sealed class GetUserChatHistory
{
    /// <summary>
    /// Запрос.
    /// </summary>
    public sealed record Query() : IRequest<IEnumerable<ChatMessage>>;

    /// <inheritdoc />
    public sealed class Handler(
        IUserRepository userRepository,
        ILlmService llmService,
        ILogger<Handler> logger) : IRequestHandler<Query, IEnumerable<ChatMessage>>
    {
        /// <inheritdoc />
        public async ValueTask<IEnumerable<ChatMessage>> Handle(Query request, CancellationToken cancellationToken)
        {
            User user = await userRepository.GetOrCreateUser("alex");
            IEnumerable<string> models = await llmService.ListLocalModelsAsync();
            if (!models.Any())
            {
                throw new InvalidOperationException("Нету моделей на сервере");
            }

            string defaultModel = models.First();
            logger.LogInformation("For new chats default model {Model} will be used", defaultModel);
            UserChat activeChat = await userRepository.GetOrCreateActiveChat(user, defaultModel);

            logger.LogInformation("Returning {Count} messages in chat history",
                activeChat
                    .Messages
                    .Count);

            await userRepository.SaveChanges(); // чтобы сохранить если было добавлено.

            return activeChat
                .Messages
                .OrderBy(x => x.Time);
        }
    }
}
