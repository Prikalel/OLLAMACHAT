namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Services.Parser;

/// <inheritdoc />
public class ImportService(
    ISolutionLoaderService solutionLoaderService,
    ILogger<ImportService> logger) : IImportService
{
    /// <inheritdoc />
    public async Task<List<string>> ResolveImportPathAsync(string importPath, Document document)
    {
        logger.LogInformation("Resolving import path: {ImportPath} for document: {DocumentPath}", importPath, document.FilePath);

        try
        {
            List<string> resolvedPaths = new();

            if (!solutionLoaderService.IsSolutionLoaded)
            {
                logger.LogWarning("Solution is not loaded");
                return resolvedPaths;
            }

            Solution? solution = solutionLoaderService.CurrentSolution;
            if (solution == null)
            {
                logger.LogWarning("No solution available");
                return resolvedPaths;
            }

            SyntaxTree? syntaxTree = await document.GetSyntaxTreeAsync();
            SyntaxNode root = await syntaxTree!.GetRootAsync();
            SemanticModel? semanticModel = await document.GetSemanticModelAsync();

            IEnumerable<UsingDirectiveSyntax> usingDirectives = root.DescendantNodes().OfType<UsingDirectiveSyntax>();
            string? documentPath = document.FilePath;
            string? documentDirectory = Path.GetDirectoryName(documentPath);

            foreach (UsingDirectiveSyntax usingDirective in usingDirectives)
            {
                string namespaceName = usingDirective.Name.ToString();
                if (namespaceName.Equals(importPath, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (Project project in solution.Projects)
                    {
                        foreach (Document projectDocument in project.Documents)
                        {
                            if (projectDocument.FilePath != documentPath)
                            {
                                SyntaxTree? projectSyntaxTree = await projectDocument.GetSyntaxTreeAsync();
                                SyntaxNode projectRoot = await projectSyntaxTree!.GetRootAsync();
                                IEnumerable<NamespaceDeclarationSyntax> namespaceDeclarations = projectRoot.DescendantNodes().OfType<NamespaceDeclarationSyntax>();

                                foreach (NamespaceDeclarationSyntax namespaceDeclaration in namespaceDeclarations)
                                {
                                    if (namespaceDeclaration.Name.ToString().Equals(namespaceName, StringComparison.OrdinalIgnoreCase))
                                    {
                                        string relativePath = GetRelativePath(documentDirectory!, projectDocument.FilePath!);
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

    /// <inheritdoc />
    public async Task<List<string>> FindCommonInitFilesAsync(string repoPath)
    {
        await Task.CompletedTask;
        logger.LogInformation("Finding common init files for repo: {RepoPath}", repoPath);

        try
        {
            if (!Directory.Exists(repoPath))
            {
                logger.LogWarning("Repository path does not exist: {RepoPath}", repoPath);
                return new();
            }

            Regex regex = new(@"^.*(GlobalUsings\.cs|\.csproj)", RegexOptions.Compiled);
            List<FileInfo> files = new DirectoryInfo(repoPath)
                .EnumerateFiles("*.*", SearchOption.AllDirectories)
                .Where(fi => regex.IsMatch(fi.Name))
                .ToList();

            logger.LogInformation("Found {InitFileCount} common init files for repo: {RepoPath}", files.Count, repoPath);
            return files.Select(x => x.FullName!).ToList();
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
        {
            return path;
        }

        Uri fromUri = new(relativeTo);
        Uri toUri = new(path);

        if (fromUri.Scheme != toUri.Scheme)
        {
            return path;
        }

        Uri relativeUri = fromUri.MakeRelativeUri(toUri);
        string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

        if (toUri.Scheme.Equals("file", StringComparison.OrdinalIgnoreCase))
        {
            relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        }

        return relativePath;
    }
}
