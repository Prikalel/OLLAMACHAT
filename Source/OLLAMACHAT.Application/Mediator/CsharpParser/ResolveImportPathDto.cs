namespace VelikiyPrikalel.OLLAMACHAT.Application.Mediator.CsharpParser;

using VelikiyPrikalel.OLLAMACHAT.Application.Models;
using VelikiyPrikalel.OLLAMACHAT.Application.Services;

using ResolveImportResponse = (List<string> result, ErrorResponse? error);

public sealed class ResolveImportPathDto
{
    /// <summary>
    /// Запрос.
    /// </summary>
    public sealed record Query(ResolveImportRequest request) : IRequest<ResolveImportResponse>;

    /// <inheritdoc />
    public sealed class Handler(IRoslynParsingService roslynParsingService) : IRequestHandler<Query, ResolveImportResponse>
    {
        /// <inheritdoc />
        public async ValueTask<ResolveImportResponse> Handle(Query request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await roslynParsingService.ResolveImportPathAsync(
                    request.request.ImportPath,
                    request.request.FilePath,
                    request.request.RepoPath);

                return (result, null);
            }
            catch (Exception ex)
            {
                var error = new ErrorResponse( ex.GetHashCode().ToString(), $"Failed to resolve import path: {ex.Message}", ex.ToString());

                return (new List<string>(), error);
            }
        }
    }
}
