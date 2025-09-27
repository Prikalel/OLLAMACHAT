namespace VelikiyPrikalel.OLLAMACHAT.Application.Services;

public interface IErrorService
{
    Task<IEnumerable<ParseError>> GetDiagnosticsAsync(Document document);
}