namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Services;

/// <inheritdoc />
public class LlmBackgroundService(
    ILlmService llmService,
    ILogger<LlmBackgroundService> logger,
    IRepository<UserChat> chatRepository) : ILlmBackgroundService
{
    /// <inheritdoc />
    public async Task<string> GenerateTextResponse(string prompt, string model, string chatId, ICollection<ChatMessage> previousMessages)
    {
        if (!await llmService.IsServerAlive())
        {
            throw new InvalidOperationException("Сервер не отвечает");
        }

        if (!(await llmService.ListLocalModelsAsync()).Contains(model))
        {
            throw new InvalidOperationException($"Модель не существует на сервере '{model}'");
        }

        string response = await llmService.FullyGenerateNextTextResponse(
            prompt,
            model,
            previousMessages
                .Select(x => new OllamaMessage(
                    x.Role == ChatMessageRole.Assistant ? ChatRole.Assistant : ChatRole.User,
                    x.Content))
                .ToList()
            );
        logger.LogInformation("Fully got response for prompt {Prompt} in chat {Id}", prompt, chatId);

        UserChat chat = await chatRepository.GetChatByIdAsync(chatId);
        ChatState state = chat.LlmReturnedResponse(prompt, response);
        await chatRepository.UpdateAsync(chat);
        logger.LogInformation("Updated state of chat {Id} to {State}", chatId, state);

        return response;
    }
}
