namespace VelikiyPrikalel.OLLAMACHAT.Application.Services.Llm;

/// <summary>
/// Сервис фонового выполнения задач.
/// </summary>
public interface ILlmBackgroundService
{
    /// <summary>
    /// Создать следующую генерацию.
    /// </summary>
    /// <param name="connectionId">Идентификатор соединения.</param>
    /// <param name="prompt"></param>
    /// <param name="chat">Идентификатор чата в котором происходит запрос.</param>
    /// <returns>Ответ llm.</returns>
    Task<string> GenerateTextResponse(string connectionId,
        string prompt,
        UserChat chat);
}
