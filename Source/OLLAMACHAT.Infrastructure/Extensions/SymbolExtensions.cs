namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Extensions;

/// <summary>
/// Методы расширения для работы с символами Roslyn
/// </summary>
public static class SymbolExtensions
{
    /// <summary>
    /// Генерирует полное отображаемое имя для символа с учетом пространства имен.
    /// Корректно обрабатывает глобальное пространство имен, убирая префикс global::.
    /// </summary>
    /// <param name="symbol">Символ, для которого нужно получить полное имя</param>
    /// <returns>Полное отображаемое имя символа без префикса global::</returns>
    public static string GetFullName(this ISymbol symbol)
    {
        if (symbol == null)
            return string.Empty;

        var fullName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat
            .WithParameterOptions(SymbolDisplayParameterOptions.IncludeType
                | SymbolDisplayParameterOptions.IncludeParamsRefOut)
            .WithMemberOptions(SymbolDisplayMemberOptions.IncludeContainingType
                | SymbolDisplayMemberOptions.IncludeParameters)
            .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted));

        return fullName;
    }
}
