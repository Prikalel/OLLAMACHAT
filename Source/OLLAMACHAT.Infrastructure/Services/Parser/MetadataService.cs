namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Services.Parser;

public class MetadataService(ILogger<MetadataService> logger) : IMetadataService
{
    public FileMetadata CalculateFileMetadata(Document document, IEnumerable<ParsedEntity> entities)
    {
        logger.LogInformation("Calculating file metadata for document: {DocumentPath}", document.FilePath);

        try
        {
            var syntaxTree = document.GetSyntaxTreeAsync().Result;
            var root = syntaxTree.GetRootAsync().Result;

            var linesOfCode = CountLinesOfCode(root);
            var complexityScore = CalculateComplexityScore(entities);
            var primaryNamespace = GetPrimaryNamespace(root);

            var metadata = new FileMetadata(
                Loc: linesOfCode,
                ComplexityScore: complexityScore,
                Namespace: primaryNamespace
            );

            logger.LogInformation("Successfully calculated file metadata for document: {DocumentPath}", document.FilePath);
            return metadata;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error calculating file metadata for document: {DocumentPath}", document.FilePath);
            throw;
        }
    }

    private int CountLinesOfCode(SyntaxNode root) =>
        root.DescendantNodesAndTokens()
            .Where(t => t.IsKind(SyntaxKind.EndOfFileToken) == false)
            .Select(t => t.GetLocation().GetLineSpan().StartLinePosition.Line)
            .Distinct()
            .Count();

    private int CalculateComplexityScore(IEnumerable<ParsedEntity> entities)
    {
        var complexity = 0;

        // TODO: proper algorithm to get complexity score
        // foreach (var entity in entities)
        // {
        //     if (entity.Type == ParsedEntityType.Method)
        //     {
        //         complexity += 1;
        //
        //         if (entity.Decorators != null)
        //         {
        //             complexity += entity.Decorators.Count;
        //         }
        //     }
        //     else if (entity.Type == ParsedEntityType.Class || entity.Type == ParsedEntityType.Interface)
        //     {
        //         complexity += 2;
        //
        //         if (entity.Children != null)
        //         {
        //             complexity += entity.Children.Count;
        //         }
        //
        //         if (entity.Inheritance?.BaseClasses != null)
        //         {
        //             complexity += entity.Inheritance.BaseClasses.Count;
        //         }
        //
        //         if (entity.Inheritance?.Interfaces != null)
        //         {
        //             complexity += entity.Inheritance.Interfaces.Count;
        //         }
        //     }
        // }

        return complexity;
    }

    private string? GetPrimaryNamespace(SyntaxNode root)
    {
        var namespaceDeclarations = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>().ToList();

        if (!namespaceDeclarations.Any())
        {
            return null;
        }

        var namespaceCounts = namespaceDeclarations
            .GroupBy(n => n.Name.ToString())
            .Select(g => new { Namespace = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .FirstOrDefault();

        return namespaceCounts?.Namespace;
    }
}