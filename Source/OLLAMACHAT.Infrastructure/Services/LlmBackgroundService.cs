namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Services;

/// <inheritdoc />
public class LlmBackgroundService(
    ILlmService llmService,
    ILogger<LlmBackgroundService> logger,
    IRepository<UserChat> chatRepository,
    IHubContext<ChatHub> hubContext) : ILlmBackgroundService
{
    /// <inheritdoc />
    public async Task GenerateTextResponse(string connectionId, string prompt, string model, string chatId, ICollection<ChatMessage> previousMessages)
    {
        if (!await llmService.IsServerAlive())
        {
            throw new InvalidOperationException("Сервер не отвечает");
        }

        if (!(await llmService.ListLocalModelsAsync()).Contains(model))
        {
            throw new InvalidOperationException($"Модель не существует на сервере '{model}'");
        }

        // Accumulate tokens while streaming response via SignalR
        string fullResponse = "";
        await foreach (string token in llmService.StreamTextResponse(
            prompt,
            model,
            previousMessages
                .Select(x => new OllamaMessage(
                    x.Role == ChatMessageRole.Assistant ? "assistant" : "user",
                    x.Content))
                .ToList()))
        {
            fullResponse += token;
            await hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessageChunk", token);
        }

        logger.LogInformation("Streamed response for prompt {Prompt} in chat {Id}", prompt, chatId);

        UserChat chat = await chatRepository.GetChatByIdAsync(chatId);
        if (chat == null)
        {
            throw new Exception($"Chat {chatId} not found");
        }
        ChatState state = chat.LlmReturnedResponse(prompt, fullResponse);
        await chatRepository.UpdateAsync(chat);
        logger.LogInformation("Updated state of chat {Id} to {State}", chatId, state);
    }
}
