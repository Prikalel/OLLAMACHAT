namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Services.Parser;

public class ErrorService(
     IMapperService mapperService,
     ILogger<ErrorService> logger) : IErrorService
{
    public async Task<IEnumerable<ParseError>> GetDiagnosticsAsync(Document document)
    {
        logger.LogInformation("Getting diagnostics for document: {DocumentPath}", document.FilePath);

        try
        {
            var semanticModel = await document.GetSemanticModelAsync();
            var syntaxTree = await document.GetSyntaxTreeAsync();

            var diagnostics = semanticModel.GetDiagnostics();
            var syntaxDiagnostics = syntaxTree.GetDiagnostics();

            var allDiagnostics = diagnostics.Concat(syntaxDiagnostics);

            var errors = allDiagnostics
                .Where(d => d.Severity == DiagnosticSeverity.Error || d.Severity == DiagnosticSeverity.Warning)
                .Select(d => new ParseError(
                    Message: d.GetMessage(),
                    Severity: mapperService.MapSeverity(d.Severity),
                    Location: mapperService.MapErrorLocation(d.Location)
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