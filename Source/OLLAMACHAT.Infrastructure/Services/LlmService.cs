using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VelikiyPrikalel.OLLAMACHAT.Application.Services;
using VelikiyPrikalel.OLLAMACHAT.Infrastructure.Settings; // For OpenAISettings

namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Services;

/// <inheritdoc />
public class LlmService : ILlmService
{
    private readonly ILogger<LlmService> logger;
    private readonly ChatClient _chatClient;
    private readonly OpenAISettings _openAISettings;

    /// <summary>
    /// ctor.
    /// </summary>
    /// <param name="openAISettings">OpenAI settings.</param>
    /// <param name="logger">Logger.</param>
    public LlmService(IOptions<OpenAISettings> openAISettings, ILogger<LlmService> logger)
    {
        this.logger = logger;
        _openAISettings = openAISettings.Value;

        if (string.IsNullOrWhiteSpace(_openAISettings.ApiKey))
        {
            throw new ArgumentNullException(nameof(_openAISettings.ApiKey), "OpenAI API key is not configured.");
        }
        if (string.IsNullOrWhiteSpace(_openAISettings.Model))
        {
            throw new ArgumentNullException(nameof(_openAISettings.Model), "OpenAI model is not configured.");
        }
        _chatClient = new ChatClient(_openAISettings.Model, _openAISettings.ApiKey);
    }

    /// <inheritdoc />
    public async Task<bool> IsServerAlive()
    {
        try
        {
            var healthCheckMessages = new List<OpenAI.Chat.ChatMessage> // Fully qualified
            {
                new UserChatMessage("Hello") // This is OpenAI.Chat.UserChatMessage
            };
            var chatCompletionOptions = new ChatCompletionOptions() { MaxTokens = 5 };
            await _chatClient.CompleteChatAsync(healthCheckMessages, chatCompletionOptions); // healthCheckMessages is now List<OpenAI.Chat.ChatMessage>
            logger.LogInformation("OpenAI server is alive.");
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "OpenAI server is not responding or configuration is invalid.");
            return false;
        }
    }

    /// <inheritdoc />
    public Task<IEnumerable<string>> ListLocalModelsAsync()
    {
        // OpenAI doesn't have "local" models in the same way Ollama does.
        // This method will return the currently configured model.
        logger.LogInformation("Returning configured OpenAI model: {Model}", _openAISettings.Model);
        return Task.FromResult<IEnumerable<string>>(new List<string> { _openAISettings.Model });
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
            ChatClient specificModelClient = new ChatClient(model, _openAISettings.ApiKey);
            ChatCompletion completion = await specificModelClient.CompleteChatAsync(chatMessages); // chatMessages is now List<OpenAI.Chat.ChatMessage>

            if (completion.Content != null && completion.Content.Count > 0)
            {
                // Assuming we want to concatenate text if there are multiple content blocks.
                // Typically for chat, there's one primary text response.
                string fullResponse = string.Join(Environment.NewLine, completion.Content.Select(c => c.Text));
                logger.LogInformation("Received response from OpenAI: {Response}", fullResponse);
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
