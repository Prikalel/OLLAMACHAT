using Attribute = VelikiyPrikalel.OLLAMACHAT.Application.Models.Attribute;
using Location = Microsoft.CodeAnalysis.Location;
using LocationApplication = VelikiyPrikalel.OLLAMACHAT.Application.Models.Location;

namespace VelikiyPrikalel.OLLAMACHAT.Application.Services.Parser;

/// <summary>
/// Маппер.
/// </summary>
public interface IMapperService
{
    /// <summary>
    /// Получить информацию о местоположении символа в файле.
    /// </summary>
    /// <param name="location">Местоположение символа в файле.</param>
    /// <returns><see cref="LocationApplication"/>.</returns>
    LocationApplication MapLocation(Location location);

    /// <summary>
    /// Получить модификаторы.
    /// </summary>
    /// <param name="symbol">Символ.</param>
    /// <returns>Модификаторы.</returns>
    IEnumerable<string> MapModifiers(ISymbol symbol);

    /// <summary>
    /// Получить цепочку наследования.
    /// </summary>
    /// <param name="typeSymbol">Символ.</param>
    /// <param name="options">Опции выгрузки.</param>
    /// <returns>Наследование.</returns>
    ParsedEntityInheritance MapInheritance(INamedTypeSymbol typeSymbol, ParserOptions? options = null);

    /// <summary>
    /// Получить return тип для методов.
    /// </summary>
    /// <param name="symbol">Символ.</param>
    /// <returns>Return тип.</returns>
    string? MapReturnType(ISymbol symbol);

    /// <summary>
    /// Получить параметры.
    /// </summary>
    /// <param name="methodSymbol">Символ.</param>
    /// <returns>Параметры.</returns>
    List<ModelParameter> MapParameters(IMethodSymbol methodSymbol);

    /// <summary>
    /// Получить аттрибуты.
    /// </summary>
    /// <param name="symbol">Символ.</param>
    /// <returns>Аттрибуты.</returns>
    List<Attribute> MapAttributes(ISymbol symbol);

    /// <summary>
    /// Маппинг ошибки.
    /// </summary>
    /// <param name="diagnosticSeverity"><see cref="DiagnosticSeverity"/>.</param>
    /// <returns><see cref="ParseErrorSeverity"/>.</returns>
    ParseErrorSeverity MapSeverity(DiagnosticSeverity diagnosticSeverity);
}
