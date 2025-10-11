namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Services.Parser;

/// <summary>
/// Сервис для обработки команд сравнения изменений между существующими и новыми данными.
/// Этот сервис предоставляет единый интерфейс для всех операций сравнения.
/// </summary>
public sealed class ChangeDetectionService : IChangeDetectionService
{
    private readonly IMediator mediator;
    private readonly ILogger<ChangeDetectionService> logger;

    /// <summary>
    /// ctor.
    /// </summary>
    /// <param name="mediator">Медиатор.</param>
    /// <param name="logger">Логгер.</param>
    public ChangeDetectionService(
        IMediator mediator,
        ILogger<ChangeDetectionService> logger)
    {
        this.mediator = mediator;
        this.logger = logger;
    }

    /// <summary>
    /// Сравнивает UnityEvent между существующими и новыми данными.
    /// </summary>
    /// <param name="existingEvents">Существующие UnityEvent из базы данных.</param>
    /// <param name="newEvents">Новые UnityEvent из анализа кода.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Результат сравнения UnityEvent.</returns>
    public async Task<ChangeDetectionResult<UnityEvent>> CompareUnityEventsAsync(
        IReadOnlyList<UnityEvent> existingEvents,
        IReadOnlyList<UnityEvent> newEvents,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Запуск сравнения UnityEvent: {ExistingCount} существующих, {NewCount} новых",
            existingEvents?.Count ?? 0, newEvents?.Count ?? 0);

        DetectUnityEventChanges.Command command = new(
            existingEvents ?? [],
            newEvents ?? []);

        DetectUnityEventChanges.Response response = await mediator.Send(command, cancellationToken);
        return response.Result;
    }

    /// <summary>
    /// Сравнивает EventHandler между существующими и новыми данными.
    /// </summary>
    /// <param name="existingHandlers">Существующие EventHandler из базы данных.</param>
    /// <param name="newHandlers">Новые EventHandler из анализа кода.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Результат сравнения EventHandler.</returns>
    public async Task<ChangeDetectionResult<EventHandler>> CompareEventHandlersAsync(
        IReadOnlyList<EventHandler> existingHandlers,
        IReadOnlyList<EventHandler> newHandlers,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Запуск сравнения EventHandler: {ExistingCount} существующих, {NewCount} новых",
            existingHandlers?.Count ?? 0, newHandlers?.Count ?? 0);

        DetectEventHandlerChanges.Command command = new(
            existingHandlers ?? [],
            newHandlers ?? []);

        DetectEventHandlerChanges.Response response = await mediator.Send(command, cancellationToken);
        return response.Result;
    }
}
