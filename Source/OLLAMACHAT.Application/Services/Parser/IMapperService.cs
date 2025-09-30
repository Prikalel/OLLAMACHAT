using Attribute = VelikiyPrikalel.OLLAMACHAT.Application.Models.Attribute;
using Location = Microsoft.CodeAnalysis.Location;
using LocationApplication = VelikiyPrikalel.OLLAMACHAT.Application.Models.Location;

namespace VelikiyPrikalel.OLLAMACHAT.Application.Services.Parser;

public interface IMapperService
{
    LocationApplication MapLocation(Location location);
    IEnumerable<string> MapModifiers(ISymbol symbol);
    ParsedEntityInheritance MapInheritance(INamedTypeSymbol typeSymbol, ParserOptions? options = null);
    string? MapReturnType(ISymbol symbol);
    List<ModelParameter> MapParameters(IMethodSymbol methodSymbol);
    List<Attribute> MapAttributes(ISymbol symbol);
    ParseErrorSeverity MapSeverity(DiagnosticSeverity diagnosticSeverity);
    LocationApplication MapErrorLocation(Location location);
}