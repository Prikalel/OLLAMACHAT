using ChatMessage = VelikiyPrikalel.OLLAMACHAT.Data.Llm.ChatMessage;

namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Services.Llm;

/// <inheritdoc />
public class LlmBackgroundService(
    ILlmService llmService,
    ILogger<LlmBackgroundService> logger,
    IHubContext<ChatHub> hubContext) : ILlmBackgroundService
{
    /// <inheritdoc />
    public async Task<string> GenerateTextResponse(string connectionId, string prompt, UserChat chat)
    {
        string model = chat.Model;
        ICollection<ChatMessage> previousMessages = chat.Messages;
        if (!await llmService.IsServerAlive())
        {
            throw new InvalidOperationException("Сервер не отвечает");
        }

        if (!(await llmService.ListLocalModelsAsync()).Contains(model))
        {
            throw new InvalidOperationException($"Модель не существует на сервере '{model}'");
        }

        // Accumulate tokens while streaming response via SignalR
        string fullResponse = await llmService.FullyGenerateNextTextResponse(
            prompt,
            model,
            previousMessages
                .Select(x => new OllamaMessage(
                    x.Role == ChatMessageRole.Assistant ? "assistant" : "user",
                    x.Content))
                .ToList());
        await hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessageChunk", Markdig.Markdown.ToHtml(fullResponse));

        logger.LogInformation("Streamed response for prompt {Prompt} in chat {Id}", prompt, chat.Id);

        return fullResponse;
    }
}
