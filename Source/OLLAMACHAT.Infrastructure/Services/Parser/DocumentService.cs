namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Services.Parser;

/// <inheritdoc />
public class DocumentService(
    ISolutionLoaderService solutionLoaderService,
    IOptions<SolutionSettings> slnSettings,
    ILogger<DocumentService> logger) : IDocumentService
{
    /// <inheritdoc />
    public async Task<Document?> GetDocumentAsync(string filePath)
    {
        logger.LogInformation("Getting document for file: {FilePath}", filePath);
        await Task.CompletedTask;

        if (!solutionLoaderService.IsSolutionLoaded)
        {
            logger.LogWarning("Solution is not loaded");
            return null;
        }

        if (!filePath.EndsWith(".cs"))
        {
            int lastIndexOfDot = filePath.LastIndexOf('.');
            if (lastIndexOfDot == -1)
            {
                logger.LogWarning("Current parser supports only .cs files but the passed file path do not contain extension");
                return null;
            }

            string substring = filePath.Substring(lastIndexOfDot, filePath.Length - lastIndexOfDot);
            logger.LogWarning("Current parser supports only .cs files not the {Extension}",
                substring);
            return null;
        }

        Solution? solution = solutionLoaderService.CurrentSolution;
        if (solution == null)
        {
            logger.LogWarning("No solution available");
            return null;
        }

        string repoPath = Path.GetDirectoryName(slnSettings.Value.SolutionFilePath)!;
        string pathToTestAgainst = Path.IsPathRooted(filePath)
            ? Path.GetFullPath(filePath)
            : Path.Join(repoPath, filePath);
        logger.LogTrace("Will check against {Path}", pathToTestAgainst);

        foreach (Project project in solution.Projects)
        {
            Document? document = project.Documents
                .FirstOrDefault(d => d.FilePath?.Equals(pathToTestAgainst, StringComparison.OrdinalIgnoreCase) is true);

            if (document != null)
            {
                logger.LogInformation("Found document in project: {ProjectName}", project.Name);
                return document;
            }
        }

        logger.LogWarning("Document not found in any project: {FilePath}", filePath);
        return null;
    }

    /// <inheritdoc />
    public async Task<string> GetContentHashAsync(Document document)
    {
        logger.LogInformation("Calculating content hash for document: {DocumentPath}", document.FilePath);

        try
        {
            SourceText sourceText = await document.GetTextAsync();
            string content = sourceText.ToString();

            using SHA256 sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(content));
            string hash = Convert.ToHexString(hashBytes).ToLowerInvariant();

            logger.LogTrace("Successfully calculated content hash for document: {DocumentPath}", document.FilePath);
            return hash;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error calculating content hash for document: {DocumentPath}", document.FilePath);
            throw;
        }
    }
}
