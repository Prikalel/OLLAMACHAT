namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Services;

public class ImportService : IImportService
{
    private readonly ISolutionLoaderService solutionLoaderService;
    private readonly ILogger<ImportService> logger;

    public ImportService(
        ISolutionLoaderService solutionLoaderService,
        ILogger<ImportService> logger)
    {
        this.solutionLoaderService = solutionLoaderService;
        this.logger = logger;
    }

    public async Task<List<string>> ResolveImportPathAsync(string importPath, Document document)
    {
        logger.LogInformation("Resolving import path: {ImportPath} for document: {DocumentPath}", importPath, document.FilePath);

        try
        {
            var resolvedPaths = new List<string>();

            if (!solutionLoaderService.IsSolutionLoaded)
            {
                logger.LogWarning("Solution is not loaded");
                return resolvedPaths;
            }

            var solution = solutionLoaderService.CurrentSolution;
            if (solution == null)
            {
                logger.LogWarning("No solution available");
                return resolvedPaths;
            }

            var syntaxTree = await document.GetSyntaxTreeAsync();
            var root = await syntaxTree.GetRootAsync();
            var semanticModel = await document.GetSemanticModelAsync();

            var usingDirectives = root.DescendantNodes().OfType<UsingDirectiveSyntax>();
            var documentPath = document.FilePath;
            var documentDirectory = Path.GetDirectoryName(documentPath);

            foreach (var usingDirective in usingDirectives)
            {
                var namespaceName = usingDirective.Name.ToString();
                if (namespaceName.Equals(importPath, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var project in solution.Projects)
                    {
                        foreach (var projectDocument in project.Documents)
                        {
                            if (projectDocument.FilePath != documentPath)
                            {
                                var projectSyntaxTree = await projectDocument.GetSyntaxTreeAsync();
                                var projectRoot = await projectSyntaxTree.GetRootAsync();
                                var namespaceDeclarations = projectRoot.DescendantNodes().OfType<NamespaceDeclarationSyntax>();

                                foreach (var namespaceDeclaration in namespaceDeclarations)
                                {
                                    if (namespaceDeclaration.Name.ToString().Equals(namespaceName, StringComparison.OrdinalIgnoreCase))
                                    {
                                        var relativePath = GetRelativePath(documentDirectory, projectDocument.FilePath);
                                        if (!resolvedPaths.Contains(relativePath))
                                        {
                                            resolvedPaths.Add(relativePath);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            logger.LogInformation("Resolved {PathCount} paths for import: {ImportPath}", resolvedPaths.Count, importPath);
            return resolvedPaths;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error resolving import path: {ImportPath} for document: {DocumentPath}", importPath, document.FilePath);
            throw;
        }
    }

    public async Task<List<string>> FindCommonInitFilesAsync(string repoPath)
    {
        logger.LogInformation("Finding common init files for repo: {RepoPath}", repoPath);

        try
        {
            var initFiles = new List<string>();

            if (!Directory.Exists(repoPath))
            {
                logger.LogWarning("Repository path does not exist: {RepoPath}", repoPath);
                return initFiles;
            }

            var commonInitFiles = new[] { "Program.cs", "Startup.cs", "Global.asax.cs" };

            foreach (var file in commonInitFiles)
            {
                var filePath = Path.Combine(repoPath, file);
                if (File.Exists(filePath))
                {
                    var relativePath = Path.GetRelativePath(repoPath, filePath);
                    initFiles.Add(relativePath);
                }
            }

            logger.LogInformation("Found {InitFileCount} common init files for repo: {RepoPath}", initFiles.Count, repoPath);
            return initFiles;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error finding common init files for repo: {RepoPath}", repoPath);
            throw;
        }
    }

    private string GetRelativePath(string relativeTo, string path)
    {
        if (string.IsNullOrEmpty(relativeTo))
            return path;

        var fromUri = new Uri(relativeTo);
        var toUri = new Uri(path);

        if (fromUri.Scheme != toUri.Scheme)
            return path;

        var relativeUri = fromUri.MakeRelativeUri(toUri);
        var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

        if (toUri.Scheme.Equals("file", StringComparison.OrdinalIgnoreCase))
        {
            relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        }

        return relativePath;
    }
}