namespace VelikiyPrikalel.OLLAMACHAT.Application.Mediator;

/// <summary>
/// Получить доступные модели.
/// </summary>
public sealed class GetAvailableModels
{
    /// <summary>
    /// Запрос.
    /// </summary>
    public sealed record Query() : IRequest<IEnumerable<string>>;

    /// <inheritdoc />
    public sealed class Handler(ILlmService llmService) : IRequestHandler<Query, IEnumerable<string>>
    {
        /// <inheritdoc />
        public async ValueTask<IEnumerable<string>> Handle(Query request, CancellationToken cancellationToken) =>
            await llmService.ListLocalModelsAsync();
    }
}
