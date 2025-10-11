namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Services.Parser;

/// <inheritdoc />
public class RoslynParsingService(
     IDocumentService documentService,
     IEntityService entityService,
     IRelationshipService relationshipService,
     IErrorService errorService,
     IImportService importService,
     ILogger<RoslynParsingService> logger) : IRoslynParsingService
{
/// <inheritdoc />
    public async Task<ParseResult> ParseFileAsync(string filePath, ParserOptions? options)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        logger.LogInformation("Starting to parse file: {FilePath}", filePath);

        Document? document = await documentService.GetDocumentAsync(filePath);
        if (document == null)
        {
            logger.LogWarning("Document not found: {FilePath}", filePath);
            throw new Exception($"Document not found: {filePath}");
        }

        string contentHash = await documentService.GetContentHashAsync(document);

        IEnumerable<ParsedEntity> entities = await entityService.ExtractEntitiesAsync(document, options);
        logger.LogInformation("Extracted {EntityCount} entities from file: {FilePath}", entities.Count(), filePath);

        IEnumerable<SimpleRelationship> relationships = await relationshipService.AnalyzeRelationshipsAsync(entities, document);
        logger.LogInformation("Analyzed {RelationshipCount} relationships in file: {FilePath}", relationships.Count(), filePath);

        IEnumerable<ParseError> errors = await errorService.GetDiagnosticsAsync(document);
        if (errors.Any())
        {
            logger.LogWarning("Found {ErrorCount} errors in file: {FilePath}", errors.Count(), filePath);
        }

        ParseResult result = new ParseResult(
            FilePath: document.FilePath!,
            Language: ParseResultLanguage.Csharp,
            Entities: entities.ToList(),
            Relationships: relationships
                .Select(x => new Relationship(x))
                .ToList(),
            ContentHash: contentHash,
            ParseTimeMs: (int)stopwatch.ElapsedMilliseconds,
            Errors: errors.ToList()
        );

        logger.LogInformation("Successfully parsed file: {FilePath} in {ElapsedMs}ms", filePath, stopwatch.ElapsedMilliseconds);
        return result;
    }

    /// <inheritdoc />
    public async Task<List<string>> ResolveImportPathAsync(string importPath, string filePath)
    {
        logger.LogInformation("Resolving import path: {ImportPath} for file: {FilePath}", importPath, filePath);
        Document? document = await documentService.GetDocumentAsync(filePath);
        if (document == null)
        {
            logger.LogWarning("Document not found: {FilePath}", filePath);
            return new List<string>();
        }

        return await importService.ResolveImportPathAsync(importPath, document);
    }
}
