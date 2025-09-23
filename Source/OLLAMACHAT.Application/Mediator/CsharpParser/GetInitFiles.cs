namespace VelikiyPrikalel.OLLAMACHAT.Application.Mediator.CsharpParser;

using VelikiyPrikalel.OLLAMACHAT.Application.Services;

/// <summary>
/// Получить доступные модели.
/// </summary>
public sealed class GetInitFiles
{
    /// <summary>
    /// Запрос.
    /// </summary>
    public sealed record Query(string RepoPath) : IRequest<List<string>>;

    /// <inheritdoc />
    public sealed class Handler(IRoslynParsingService roslynParsingService) : IRequestHandler<Query, List<string>>
    {
        /// <inheritdoc />
        public async ValueTask<List<string>> Handle(Query request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await roslynParsingService.GetInitFilesAsync(request.RepoPath);
                return result;
            }
            catch (Exception ex)
            {
                return new List<string>();
            }
        }
    }
}
