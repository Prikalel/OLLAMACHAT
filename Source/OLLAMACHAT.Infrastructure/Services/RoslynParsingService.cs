namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Services;

public class RoslynParsingService(
     IDocumentService documentService,
     IEntityService entityService,
     IRelationshipService relationshipService,
     IErrorService errorService,
     IMetadataService metadataService,
     IImportService importService,
     ILogger<RoslynParsingService> logger) : IRoslynParsingService
{
    public async Task<ParseResult> ParseFileAsync(string filePath, string repoPath, ParserOptions? options)
    {
        var stopwatch = Stopwatch.StartNew();
        logger.LogInformation("Starting to parse file: {FilePath}", filePath);

        try
        {
            var document = await documentService.GetDocumentAsync(filePath, repoPath);
            if (document == null)
            {
                logger.LogWarning("Document not found: {FilePath}", filePath);
                return CreateErrorResult($"Document not found: {filePath}");
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
        catch (Exception ex)
        {
            logger.LogError(ex, "Error parsing file: {FilePath}", filePath);
            return CreateErrorResult($"Parsing failed: {ex.Message}");
        }
    }

    public async Task<List<string>> GetInitFilesAsync(string repoPath)
    {
        logger.LogInformation("Getting init files for repo: {RepoPath}", repoPath);
        return await importService.FindCommonInitFilesAsync(repoPath);
    }

    public async Task<List<string>> ResolveImportPathAsync(string importPath, string filePath, string repoPath)
    {
        logger.LogInformation("Resolving import path: {ImportPath} for file: {FilePath}", importPath, filePath);
        var document = await documentService.GetDocumentAsync(filePath, repoPath);
        if (document == null)
        {
            logger.LogWarning("Document not found: {FilePath}", filePath);
            return new List<string>();
        }

        return await importService.ResolveImportPathAsync(importPath, document);
    }

    private ParseResult CreateErrorResult(string errorMessage)
    {
        return new ParseResult(
            FilePath: "",
            Language: ParseResultLanguage.Csharp,
            Entities: new List<ParsedEntity>(),
            Relationships: new List<Relationship>(),
            ContentHash: "",
            ParseTimeMs: 0,
            Errors: new List<ParseError>
            {
                new ParseError(
                    Message: errorMessage,
                    Severity: ParseErrorSeverity.Error,
                    Location: null
                )
            },
            Metadata: null
        );
    }
}