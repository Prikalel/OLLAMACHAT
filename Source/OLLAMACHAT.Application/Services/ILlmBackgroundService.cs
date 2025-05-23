namespace VelikiyPrikalel.OLLAMACHAT.Application.Services;

/// <summary>
/// Сервис фонового выполнения задач.
/// </summary>
public interface ILlmBackgroundService
{
    /// <summary>
    /// Создать следующую генерацию.
    /// </summary>
    /// <param name="model">Модель выполняющая запрос.</param>
    /// <param name="userId">Идентификатор инициатора.</param>
    /// <param name="previousMessages">Предыдущая переписка.</param>
    /// <returns>Ответ llm.</returns>
    Task<string> GenerateTextResponse(
        string model,
        string userId,
        ICollection<ChatMessage> previousMessages);
}
