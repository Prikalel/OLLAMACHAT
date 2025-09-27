namespace VelikiyPrikalel.OLLAMACHAT.Application.Services.Parser;

public interface IErrorService
{
    Task<IEnumerable<ParseError>> GetDiagnosticsAsync(Document document);
}