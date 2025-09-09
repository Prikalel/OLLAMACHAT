using System.Text.Json.Nodes;
using ModelContextProtocol.Protocol;
using OpenAI.Chat;
using ChatMessage = OpenAI.Chat.ChatMessage;

namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Services;

/// <inheritdoc />
public class LlmService : ILlmService
{
    private readonly ILogger<LlmService> logger;
    private readonly OpenAISettings openAISettings;
    private readonly IMcpConfigurationService mcpConfigurationService;
    private readonly string uri;

    /// <summary>
    /// ctor.
    /// </summary>
    /// <param name="openAISettings">OpenAI settings.</param>
    /// <param name="logger">Logger.</param>
    public LlmService(IOptions<OpenAISettings> openAISettings, ILogger<LlmService> logger, IMcpConfigurationService mcpConfigurationService)
    {
        this.logger = logger;
        this.openAISettings = openAISettings.Value;
        this.mcpConfigurationService = mcpConfigurationService;
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
            List<ChatTool> tools = await GetTools();
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
                            if (toolCall.FunctionName == "search_wikipedia")
                            {
                                logger.LogInformation(
                                    "Model requires tool call(s). Processing {Count} call(s).",
                                    completion.ToolCalls.Count);

                                string toolResult = await SearchWikipedia(
                                    JObject.Parse(toolCall.FunctionArguments)["term"]
                                        .ToString());
                                chatMessages.Add(new ToolChatMessage(toolCall.Id,
                                    toolResult));
                            }
                            else if (toolCall.FunctionName.IndexOf('.') is int separatorIndex
                                     && separatorIndex > 0
                                     && toolCall.FunctionName.Substring(0, separatorIndex) is string serverName
                                     && serverName.Length > 0
                                     && mcpConfigurationService.GetServerByName(serverName) is McpServerInfo mcpServerInfo
                                     && mcpServerInfo.Type.Equals("SSE", StringComparison.OrdinalIgnoreCase))
                            {
                                await using IMcpClient client = await ConnectToSseMcpClient(mcpServerInfo);

                                JsonNode argumentsNode = JsonNode.Parse(toolCall.FunctionArguments);
                                IReadOnlyDictionary<string, object?> arguments = ConvertJsonNodeToDictionary(argumentsNode);

                                string toolName = toolCall.FunctionName.Substring(separatorIndex + 1);
                                logger.LogInformation(
                                    "Request to call tool: {Tool}",
                                    toolName);
                                CallToolResult result = await client.CallToolAsync(
                                    toolName,
                                    arguments);

                                string? responseContent = (result.Content.FirstOrDefault() as TextContentBlock)?.Text;
                                chatMessages.Add(new ToolChatMessage(
                                    toolCall.Id,
                                    responseContent));
                                logger.LogInformation(
                                    "Successfully called tool: {Tool}",
                                    toolName);
                            }
                            else
                            {
                                // Handle other unexpected calls.
                                throw new NotImplementedException();
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
    private async Task<List<ChatTool>> GetTools()
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

        foreach (McpServerInfo serverInfo in mcpConfigurationService.GetAllServers())
        {
            if (serverInfo.Type.Equals("SSE", StringComparison.OrdinalIgnoreCase))
            {
                await using IMcpClient client = await ConnectToSseMcpClient(serverInfo);

                foreach (McpClientTool tool in await client.ListToolsAsync())
                {
                    ChatTool customTool = ChatTool.CreateFunctionTool(
                        serverInfo.Name.Replace(' ', '_') + "." + tool.Name,
                        tool.Description,
                        BinaryData.FromString(tool.JsonSchema.GetRawText()));

                    tools.Add(customTool);
                    Console.WriteLine($"registered {customTool.FunctionName} ({tool.Description})");
                }
            }
        }

        return tools;
    }

    private static async Task<IMcpClient> ConnectToSseMcpClient(McpServerInfo serverInfo)
    {
        SseClientTransport clientTransport = new(new()
        {
            Name = serverInfo.Name,
            Endpoint = new Uri(serverInfo.Url),
            AdditionalHeaders = serverInfo.AuthToken is null
                ? null
                : new Dictionary<string, string>()
                {
                    { "Authorization", serverInfo.AuthToken }
                }
        });

        IMcpClient client = await McpClientFactory.CreateAsync(clientTransport);
        return client;
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

    private IReadOnlyDictionary<string, object?> ConvertJsonNodeToDictionary(JsonNode jsonNode)
    {
        if (jsonNode is not JsonObject jsonObject)
        {
            return new Dictionary<string, object?>();
        }

        Dictionary<string, object?> dictionary = new();

        foreach (KeyValuePair<string, JsonNode?> property in jsonObject)
        {
            dictionary[property.Key] = ConvertJsonValue(property.Value);
        }

        return dictionary;
    }

    private object? ConvertJsonValue(JsonNode? node)
    {
        if (node == null)
            return null;

        return node switch
        {
            JsonObject jsonObject => ConvertJsonNodeToDictionary(jsonObject),
            JsonArray jsonArray => jsonArray.Select(ConvertJsonValue).ToList(),
            JsonValue jsonValue => GetJsonValueContent(jsonValue),
            _ => node.ToString()
        };
    }

    private object? GetJsonValueContent(JsonValue jsonValue)
    {
        if (jsonValue.TryGetValue<string>(out string? stringValue))
            return stringValue;
        if (jsonValue.TryGetValue<int>(out int intValue))
            return intValue;
        if (jsonValue.TryGetValue<double>(out double doubleValue))
            return doubleValue;
        if (jsonValue.TryGetValue<bool>(out bool boolValue))
            return boolValue;

        return jsonValue.ToString();
    }
}
