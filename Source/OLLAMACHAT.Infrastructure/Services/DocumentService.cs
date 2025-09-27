namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Services;

/// <inheritdoc />
public class DocumentService : IDocumentService
{
    private readonly ISolutionLoaderService solutionLoaderService;
    private readonly ILogger<DocumentService> logger;

    public DocumentService(
        ISolutionLoaderService solutionLoaderService,
        ILogger<DocumentService> logger)
    {
        this.solutionLoaderService = solutionLoaderService;
        this.logger = logger;
    }

    /// <inheritdoc />
    public async Task<Document?> GetDocumentAsync(string filePath, string repoPath)
    {
        logger.LogInformation("Getting document for file: {FilePath}", filePath);

        if (!solutionLoaderService.IsSolutionLoaded)
        {
            logger.LogWarning("Solution is not loaded");
            return null;
        }

        try
        {
            var solution = solutionLoaderService.CurrentSolution;
            if (solution == null)
            {
                logger.LogWarning("No solution available");
                return null;
            }

            foreach (var project in solution.Projects)
            {
                var document = project.Documents.FirstOrDefault(d =>
                    d.FilePath?.Equals(filePath, StringComparison.OrdinalIgnoreCase) is true
                    || d.FilePath?.Equals(Path.Join(repoPath, filePath), StringComparison.OrdinalIgnoreCase) is true);

                if (document != null)
                {
                    logger.LogInformation("Found document in project: {ProjectName}", project.Name);
                    return document;
                }
            }

            logger.LogWarning("Document not found in any project: {FilePath}", filePath);
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting document: {FilePath}", filePath);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<string> GetContentHashAsync(Document document)
    {
        logger.LogInformation("Calculating content hash for document: {DocumentPath}", document.FilePath);

        try
        {
            var sourceText = await document.GetTextAsync();
            var content = sourceText.ToString();

            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(content));
            var hash = Convert.ToHexString(hashBytes).ToLowerInvariant();

            logger.LogInformation("Successfully calculated content hash for document: {DocumentPath}", document.FilePath);
            return hash;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error calculating content hash for document: {DocumentPath}", document.FilePath);
            throw;
        }
    }
}