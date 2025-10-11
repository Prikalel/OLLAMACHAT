namespace VelikiyPrikalel.OLLAMACHAT.Application.Mediator.CsharpParser;

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
    public sealed class Handler(IImportService importService, ILogger<Handler> logger,
        ISolutionLoaderService loaderService) : IRequestHandler<Query, List<string>>
    {
        /// <inheritdoc />
        public async ValueTask<List<string>> Handle(Query request, CancellationToken cancellationToken)
        {
            if (!loaderService.IsSolutionLoaded)
            {
                logger.LogError("Solution not loaded");
                return new();
            }

            try
            {
                List<string> findCommonInitFilesAsync = await importService.FindCommonInitFilesAsync(request.RepoPath);
                HashSet<string> existingProjects = loaderService.GetProjects().Select(x => x.FilePath!).ToHashSet();
                findCommonInitFilesAsync = findCommonInitFilesAsync
                    .Where(x => x.EndsWith("GlobalUsing.cs"))
                    .Union(findCommonInitFilesAsync
                        .Where(x => x.EndsWith(".csproj"))
                        .Intersect(existingProjects)
                    )
                    .ToList();
                    
                foreach (string file in findCommonInitFilesAsync)
                {
                    logger.LogTrace("[init file] - {InitFile}", file);
                }

                return findCommonInitFilesAsync;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "error getting init files");
                return new();
            }
        }
    }
}
