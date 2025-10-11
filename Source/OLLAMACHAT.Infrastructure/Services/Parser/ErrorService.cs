namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Services.Parser;

/// <inheritdoc />
public class ErrorService(
     IMapperService mapperService,
     ILogger<ErrorService> logger) : IErrorService
{
    /// <inheritdoc />
    public async Task<IEnumerable<ParseError>> GetDiagnosticsAsync(Document document)
    {
        logger.LogInformation("Getting diagnostics for document: {DocumentPath}", document.FilePath);

        try
        {
            SemanticModel? semanticModel = await document.GetSemanticModelAsync();
            SyntaxTree? syntaxTree = await document.GetSyntaxTreeAsync();

            ImmutableArray<Diagnostic> diagnostics = semanticModel!.GetDiagnostics();
            IEnumerable<Diagnostic> syntaxDiagnostics = syntaxTree!.GetDiagnostics();

            IEnumerable<Diagnostic> allDiagnostics = diagnostics.Concat(syntaxDiagnostics);

            List<ParseError> errors = allDiagnostics
                .Where(d => d.Severity == DiagnosticSeverity.Error || d.Severity == DiagnosticSeverity.Warning)
                .Select(d => new ParseError(
                    Message: d.GetMessage(),
                    Severity: mapperService.MapSeverity(d.Severity),
                    Location: mapperService.MapLocation(d.Location)
                ))
                .ToList();

            logger.LogInformation("Found {ErrorCount} diagnostics for document: {DocumentPath}", errors.Count, document.FilePath);
            return errors;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting diagnostics for document: {DocumentPath}", document.FilePath);
            throw;
        }
    }
}
