using Location = Microsoft.CodeAnalysis.Location;
using LocationApplication = VelikiyPrikalel.OLLAMACHAT.Application.Models.Location;

namespace VelikiyPrikalel.OLLAMACHAT.Application.Services;

public interface IMapperService
{
    LocationApplication MapLocation(Location location);
    IEnumerable<string> MapModifiers(ISymbol symbol);
    ParsedEntityInheritance MapInheritance(INamedTypeSymbol typeSymbol);
    string? MapReturnType(ISymbol symbol);
    List<ModelParameter> MapParameters(IMethodSymbol methodSymbol);
    ParsedEntityImportData MapImportData(ISymbol symbol);
    List<Decorator> MapAttributes(ISymbol symbol);
    ParseErrorSeverity MapSeverity(DiagnosticSeverity diagnosticSeverity);
    LocationApplication MapErrorLocation(Location location);
}