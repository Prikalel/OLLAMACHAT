namespace VelikiyPrikalel.OLLAMACHAT.Application.Services;

/// <summary>
/// Сервис llm.
/// </summary>
public interface ILlmService
{
    /// <summary>
    /// Проверить что сервер жив.
    /// </summary>
    /// <returns>True если всё хорошо.</returns>
    Task<bool> IsServerAlive();

    /// <summary>
    /// Перечислить доступные пользователю модели.
    /// </summary>
    /// <returns>Список моделей (строковых названий).</returns>
    Task<IEnumerable<string>> ListLocalModelsAsync();

    /// <summary>
    /// .
    /// </summary>
    /// <param name="prompt">.</param>
    /// <param name="model">.</param>
    /// <param name="previousMessages">.</param>
    /// <returns></returns>
    Task<string> FullyGenerateNextTextResponse(string prompt, string model, ICollection<OllamaMessage> previousMessages);
}

/// <summary>
/// Сообщение.
/// </summary>
/// <param name="Role">Роль.</param>
/// <param name="Content">Контент.</param>
public record OllamaMessage(
    string Role, // Changed from OllamaSharp.Models.Chat.ChatRole
    string Content);
