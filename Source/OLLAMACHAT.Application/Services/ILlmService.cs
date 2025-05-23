namespace VelikiyPrikalel.OLLAMACHAT.Application.Services;

/// <summary>
/// Сервис llm.
/// </summary>
public interface ILlmService
{
    /// <summary>
    /// Перечислить доступные пользователю модели.
    /// </summary>
    /// <returns>Список моделей (строковых названий).</returns>
    Task<IEnumerable<string>> ListLocalModelsAsync();
}
