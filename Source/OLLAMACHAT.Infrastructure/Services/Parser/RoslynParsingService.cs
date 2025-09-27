namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Services.Parser;

public class RoslynParsingService(
     IDocumentService documentService,
     IEntityService entityService,
     IRelationshipService relationshipService,
     IErrorService errorService,
     IMetadataService metadataService,
     IImportService importService,
     ILogger<RoslynParsingService> logger) : IRoslynParsingService
{
    public async Task<ParseResult> ParseFileAsync(string filePath, ParserOptions? options)
    {
        var stopwatch = Stopwatch.StartNew();
        logger.LogInformation("Starting to parse file: {FilePath}", filePath);

        var document = await documentService.GetDocumentAsync(filePath);
        if (document == null)
        {
            logger.LogWarning("Document not found: {FilePath}", filePath);
            throw new Exception($"Document not found: {filePath}");
        }

        var contentHash = await documentService.GetContentHashAsync(document);

        var entities = await entityService.ExtractEntitiesAsync(document);
        logger.LogInformation("Extracted {EntityCount} entities from file: {FilePath}", entities.Count(), filePath);

        var relationships = await relationshipService.AnalyzeRelationshipsAsync(entities, document);
        logger.LogInformation("Analyzed {RelationshipCount} relationships in file: {FilePath}", relationships.Count(), filePath);

        var errors = await errorService.GetDiagnosticsAsync(document);
        if (errors.Any())
        {
            logger.LogWarning("Found {ErrorCount} errors in file: {FilePath}", errors.Count(), filePath);
        }

        var metadata = metadataService.CalculateFileMetadata(document, entities);

        var result = new ParseResult(
            FilePath: filePath,
            Language: ParseResultLanguage.Csharp,
            Entities: entities.ToList(),
            Relationships: relationships.ToList(),
            ContentHash: contentHash,
            ParseTimeMs: (int)stopwatch.ElapsedMilliseconds,
            Errors: errors.ToList(),
            Metadata: metadata
        );

        logger.LogInformation("Successfully parsed file: {FilePath} in {ElapsedMs}ms", filePath, stopwatch.ElapsedMilliseconds);
        return result;
    }

    public async Task<List<string>> ResolveImportPathAsync(string importPath, string filePath)
    {
        logger.LogInformation("Resolving import path: {ImportPath} for file: {FilePath}", importPath, filePath);
        var document = await documentService.GetDocumentAsync(filePath);
        if (document == null)
        {
            logger.LogWarning("Document not found: {FilePath}", filePath);
            return new List<string>();
        }

        return await importService.ResolveImportPathAsync(importPath, document);
    }
}