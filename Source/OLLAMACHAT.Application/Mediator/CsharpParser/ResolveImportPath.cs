namespace VelikiyPrikalel.OLLAMACHAT.Application.Mediator.CsharpParser;

using ResolveImportResponse = (List<string> result, ErrorResponse? error);

/// <summary>
/// Команда резолвинга usings выражений.
/// </summary>
public sealed class ResolveImportPath
{
    /// <summary>
    /// Запрос.
    /// </summary>
    /// <param name="Request">Запрос.</param>
    public sealed record Query(ResolveImportRequest Request) : IRequest<ResolveImportResponse>;

    /// <inheritdoc />
    public sealed class Handler(IRoslynParsingService roslynParsingService) : IRequestHandler<Query, ResolveImportResponse>
    {
        /// <inheritdoc />
        public async ValueTask<ResolveImportResponse> Handle(Query request, CancellationToken cancellationToken)
        {
            try
            {
                List<string> result = await roslynParsingService.ResolveImportPathAsync(
                    request.Request.ImportPath,
                    request.Request.FilePath);

                return (result, null);
            }
            catch (Exception ex)
            {
                ErrorResponse error = new( ex.GetHashCode().ToString(), $"Failed to resolve import path: {ex.Message}", ex.ToString());

                return (new(), error);
            }
        }
    }
}
