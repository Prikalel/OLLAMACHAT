using Microsoft.CodeAnalysis;
using VelikiyPrikalel.OLLAMACHAT.Application.Models;

namespace VelikiyPrikalel.OLLAMACHAT.Application.Services;

public interface IErrorService
{
    Task<IEnumerable<ParseError>> GetDiagnosticsAsync(Document document);
}