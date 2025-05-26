using Newtonsoft.Json.Linq;
using OpenAI.Chat;

namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Services;

/// <inheritdoc />
public class LlmService : ILlmService
{
    private const string uri = "https://api.llm7.io/v1";
    private readonly ILogger<LlmService> logger;
    private readonly OpenAISettings openAISettings;

    /// <summary>
    /// ctor.
    /// </summary>
    /// <param name="openAISettings">OpenAI settings.</param>
    /// <param name="logger">Logger.</param>
    public LlmService(IOptions<OpenAISettings> openAISettings, ILogger<LlmService> logger)
    {
        this.logger = logger;
        this.openAISettings = openAISettings.Value;

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
    public async Task<string> FullyGenerateNextTextResponse(string prompt, string model, ICollection<OllamaMessage> previousMessages)
    {
        logger.LogInformation("Sending chat request to OpenAI. Model: {Model}, Prompt: {Prompt}", model, prompt);

        var chatMessages = new List<OpenAI.Chat.ChatMessage>(); // Fully qualified

        // Map previous messages
        foreach (var prevMessage in previousMessages)
        {
            if (prevMessage.Role.Equals("user", StringComparison.OrdinalIgnoreCase))
            {
                chatMessages.Add(new UserChatMessage(prevMessage.Content)); // This is OpenAI.Chat.UserChatMessage
            }
            else if (prevMessage.Role.Equals("assistant", StringComparison.OrdinalIgnoreCase))
            {
                chatMessages.Add(new AssistantChatMessage(prevMessage.Content)); // This is OpenAI.Chat.AssistantChatMessage
            }
            // Add other roles if necessary, e.g., "system"
            // Consider adding a SystemChatMessage if you have a system prompt concept
        }

        // Add the current user prompt
        chatMessages.Add(new UserChatMessage(prompt)); // This is OpenAI.Chat.UserChatMessage

        try
        {
            // Use the 'model' parameter passed to the method, which might be different from the default in settings
            ChatClient specificModelClient = new ChatClient(model, openAISettings.ApiKey, new OpenAI.OpenAIClientOptions()
            {
                Endpoint = new Uri(uri)
            });
            ChatCompletion completion = await specificModelClient.CompleteChatAsync(chatMessages); // chatMessages is now List<OpenAI.Chat.ChatMessage>

            if (completion.Content != null && completion.Content.Count > 0)
            {
                // Assuming we want to concatenate text if there are multiple content blocks.
                // Typically for chat, there's one primary text response.
                string fullResponse = string.Join(Environment.NewLine, completion.Content.Select(c => c.Text));
                logger.LogInformation("Received response from OpenAI: {Response}", fullResponse);
                if (model == "deepseek-v3") // особенность этой модели
                {
                    var responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject(fullResponse);
                    return (responseObject as JObject)["reasoning_content"].ToString();
                }
                return fullResponse;
            }
            else
            {
                logger.LogWarning("OpenAI returned no content.");
                return string.Empty;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during OpenAI chat completion request.");
            throw; // Re-throw to allow higher levels to handle
        }
    }
}
