using Newtonsoft.Json.Linq;
using OpenAI.Chat;
using ChatMessage = OpenAI.Chat.ChatMessage;

namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Services;

/// <inheritdoc />
public class LlmService : ILlmService
{
    private readonly ILogger<LlmService> logger;
    private readonly OpenAISettings openAISettings;
    private readonly string uri;

    /// <summary>
    /// ctor.
    /// </summary>
    /// <param name="openAISettings">OpenAI settings.</param>
    /// <param name="logger">Logger.</param>
    public LlmService(IOptions<OpenAISettings> openAISettings, ILogger<LlmService> logger)
    {
        this.logger = logger;
        this.openAISettings = openAISettings.Value;
        this.uri = openAISettings.Value.ApiBase;

        if (string.IsNullOrWhiteSpace(this.uri))
        {
            throw new ArgumentNullException(nameof(LlmService.openAISettings.ApiBase), "OpenAI API net address is not configured.");
        }
        if (string.IsNullOrWhiteSpace(this.openAISettings.ApiKey))
        {
            throw new ArgumentNullException(nameof(LlmService.openAISettings.ApiKey), "OpenAI API key is not configured.");
        }
        if (string.IsNullOrWhiteSpace(this.openAISettings.Models.FirstOrDefault()))
        {
            throw new ArgumentNullException(nameof(LlmService.openAISettings.Models), "OpenAI model is not configured.");
        }
    }

    /// <inheritdoc />
    public Task<bool> IsServerAlive() => Task.FromResult(true);

    /// <inheritdoc />
    public Task<IEnumerable<string>> ListLocalModelsAsync()
    {
        // OpenAI doesn't have "local" models in the same way Ollama does.
        // This method will return the currently configured model.
        logger.LogInformation("Returning configured OpenAI model: {Model}", openAISettings.Models);
        return Task.FromResult<IEnumerable<string>>( openAISettings.Models );
    }

    /// <inheritdoc />
    public async Task<string> FullyGenerateNextTextResponse(
        string prompt,
        string model,
        ICollection<OllamaMessage> previousMessages)
    {
        logger.LogInformation("Sending chat request to OpenAI. Model: {Model}, Prompt: {Prompt}",
            model,
            prompt);

        // Создаем список сообщений, включая историю и текущий запрос
        List<ChatMessage> chatMessages = string.IsNullOrWhiteSpace(openAISettings.SystemChatMessage)
        ? []
        :
        [
            new SystemChatMessage(openAISettings.SystemChatMessage)
        ];

        // Map previous messages
        foreach (OllamaMessage prevMessage in previousMessages)
        {
            if (prevMessage.Role.Equals("user", StringComparison.OrdinalIgnoreCase))
            {
                chatMessages.Add(new UserChatMessage(prevMessage.Content));
            }
            else if (prevMessage.Role.Equals("assistant", StringComparison.OrdinalIgnoreCase))
            {
                chatMessages.Add(new AssistantChatMessage(prevMessage.Content));
            }
        }

        chatMessages.Add(new UserChatMessage(prompt));

        ChatClient chatClient = new(model,
            openAISettings.ApiKey,
            new()
            {
                Endpoint = new Uri(uri)
            });

        ChatCompletionOptions chatCompletionOptions = new()
        {
            ToolChoice = openAISettings.EnableTools
                ? ChatToolChoice.Auto
                : ChatToolChoice.None
        };
        if (openAISettings.EnableTools)
        {
            List<ChatTool> tools = GetTools();
            foreach (ChatTool tool in tools)
            {
                chatCompletionOptions.Tools.Add(tool);
            }
        }

        // Цикл для обработки потенциальных вызовов инструментов
        try
        {
            while (true)
            {
                ChatCompletion completion = await chatClient.CompleteChatAsync(
                    chatMessages,
                    chatCompletionOptions);

                if (completion is null)
                {
                    return "provider returned null";
                }

                switch (completion.FinishReason)
                {
                    case ChatFinishReason.Stop:
                    {
                        string fullResponse = string.Join(Environment.NewLine,
                            completion.Content.Select(c => c.Text));
                        logger.LogInformation(
                            "Received final text response from OpenAI: {Response}",
                            fullResponse);
                        return fullResponse;
                    }

                    case ChatFinishReason.ToolCalls:
                    {
                        chatMessages.Add(new AssistantChatMessage(completion));

                        foreach (ChatToolCall toolCall in completion.ToolCalls)
                        {
                            switch (toolCall.FunctionName)
                            {
                                case "search_wikipedia":
                                {
                                    logger.LogInformation(
                                        "Model requires tool call(s). Processing {Count} call(s).",
                                        completion.ToolCalls.Count);

                                    string toolResult = await SearchWikipedia(
                                        JObject.Parse(toolCall.FunctionArguments)["term"]
                                            .ToString());
                                    chatMessages.Add(new ToolChatMessage(toolCall.Id,
                                        toolResult));
                                    break;
                                }

                                default:
                                {
                                    // Handle other unexpected calls.
                                    throw new NotImplementedException();
                                }
                            }
                        }

                        break;
                    }

                    case ChatFinishReason.Length:
                        throw new NotImplementedException(
                            "Incomplete model output due to MaxTokens parameter or token limit exceeded.");

                    case ChatFinishReason.ContentFilter:
                        throw new NotImplementedException(
                            "Omitted content due to a content filter flag.");

                    case ChatFinishReason.FunctionCall:
                        throw new NotImplementedException("Deprecated in favor of tool calls.");

                    default:
                        throw new NotImplementedException(completion.FinishReason.ToString());
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Error during OpenAI chat completion request.");
            throw;
        }
    }

    /// <summary>
    /// Создает и возвращает список инструментов, доступных для использования моделью.
    /// </summary>
    /// <returns>Список инструментов.</returns>
    private List<ChatTool> GetTools()
    {
        List<ChatTool> tools = new List<ChatTool>();

        // Создаем инструмент для поиска по википедии
        ChatTool wikipediaTool = ChatTool.CreateFunctionTool(
            "search_wikipedia",
            "Search info in Wikipedia and returns as string.",
            BinaryData.FromObjectAsJson(new
            {
                type = "object",
                properties = new
                {
                    term = new
                    {
                        type = "string",
                        description = "Term to search to in Wikipedia."
                    }
                },
                required = new[]
                {
                    "term"
                }
            }));

        tools.Add(wikipediaTool);
        return tools;
    }

    private async Task<string> SearchWikipedia(string term)
    {
        try
        {

            // Используем API Wikipedia для получения краткого содержания страницы
            using HttpClient httpClient = new HttpClient();
            string requestUri = $"https://en.wikipedia.org/api/rest_v1/page/summary/{Uri.EscapeDataString(term)}";
            HttpResponseMessage response = await httpClient.GetAsync(requestUri);

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                JObject json = JObject.Parse(jsonResponse);
                string extract = json["extract"]?.ToString() ?? "Извините, не удалось извлечь содержание.";
                return extract;
            }
            else
            {
                return $"Ошибка при поиске в Wikipedia: {response.StatusCode}";
            }
        }
        catch (Exception ex)
        {
            return $"Исключение при поиске в Wikipedia: {ex.Message}";
        }
    }
}
