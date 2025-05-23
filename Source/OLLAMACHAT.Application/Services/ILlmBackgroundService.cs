namespace VelikiyPrikalel.OLLAMACHAT.Application.Services;

/// <summary>
/// Сервис фонового выполнения задач.
/// </summary>
public interface ILlmBackgroundService
{
    /// <summary>
    /// Создать следующую генерацию.
    /// </summary>
    /// <param name="prompt"></param>
    /// <param name="model">Модель выполняющая запрос.</param>
    /// <param name="chatId">Идентификатор чата в котором происходит запрос.</param>
    /// <param name="previousMessages">Предыдущая переписка.</param>
    /// <returns>Ответ llm.</returns>
    Task<string> GenerateTextResponse(string prompt,
        string model,
        string chatId,
        ICollection<ChatMessage> previousMessages);
}
