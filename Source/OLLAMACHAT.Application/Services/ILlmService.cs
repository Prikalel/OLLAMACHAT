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
}
