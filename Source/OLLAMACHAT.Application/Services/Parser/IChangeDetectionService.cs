namespace VelikiyPrikalel.OLLAMACHAT.Application.Services.Parser;

/// <summary>
/// Сервис для обработки команд сравнения изменений между существующими и новыми данными.
/// Этот сервис предоставляет единый интерфейс для всех операций сравнения.
/// </summary>
public interface IChangeDetectionService
{
    /// <summary>
    /// Сравнивает UnityEvent между существующими и новыми данными.
    /// </summary>
    /// <param name="existingEvents">Существующие UnityEvent из базы данных.</param>
    /// <param name="newEvents">Новые UnityEvent из анализа кода.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Результат сравнения UnityEvent.</returns>
    public Task<ChangeDetectionResult<UnityEvent>> CompareUnityEventsAsync(
        IReadOnlyList<UnityEvent> existingEvents,
        IReadOnlyList<UnityEvent> newEvents,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Сравнивает EventHandler между существующими и новыми данными.
    /// </summary>
    /// <param name="existingHandlers">Существующие EventHandler из базы данных.</param>
    /// <param name="newHandlers">Новые EventHandler из анализа кода.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Результат сравнения EventHandler.</returns>
    public Task<ChangeDetectionResult<EventHandler>> CompareEventHandlersAsync(
        IReadOnlyList<EventHandler> existingHandlers,
        IReadOnlyList<EventHandler> newHandlers,
        CancellationToken cancellationToken = default);
}
