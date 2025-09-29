using Attribute = VelikiyPrikalel.OLLAMACHAT.Application.Models.Attribute;
using Location = Microsoft.CodeAnalysis.Location;
using LocationApplication = VelikiyPrikalel.OLLAMACHAT.Application.Models.Location;

namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Services.Parser;

public class MapperService(ILogger<MapperService> logger) : IMapperService
{
    public LocationApplication MapLocation(Location location)
    {
        if (location == null)
        {
            return new LocationApplication(new Position(null, null, null), new Position(null, null, null));
        }

        var lineSpan = location.GetLineSpan();
        var startLinePosition = lineSpan.StartLinePosition;
        var endLinePosition = lineSpan.EndLinePosition;

        return new LocationApplication(
            new Position(
                startLinePosition.Line + 1,
                startLinePosition.Character,
                location.SourceSpan.Start
            ),
            new Position(
                endLinePosition.Line + 1,
                endLinePosition.Character,
                location.SourceSpan.End
            )
        );
    }

    public IEnumerable<string> MapModifiers(ISymbol symbol)
    {
        var modifiers = new List<string>();

        if (symbol is IMethodSymbol methodSymbol)
        {
            if (methodSymbol.IsStatic)
                modifiers.Add("static");
            if (methodSymbol.IsVirtual)
                modifiers.Add("virtual");
            if (methodSymbol.IsOverride)
                modifiers.Add("override");
            if (methodSymbol.IsAbstract)
                modifiers.Add("abstract");
            if (methodSymbol.IsSealed)
                modifiers.Add("sealed");
            if (methodSymbol.IsAsync)
                modifiers.Add("async");
        }

        if (symbol is ITypeSymbol typeSymbol)
        {
            if (typeSymbol.IsStatic)
                modifiers.Add("static");
            if (typeSymbol.IsAbstract)
                modifiers.Add("abstract");
            if (typeSymbol.IsSealed)
                modifiers.Add("sealed");
        }

        if (symbol.DeclaredAccessibility == Accessibility.Public)
            modifiers.Add("public");
        else if (symbol.DeclaredAccessibility == Accessibility.Private)
            modifiers.Add("private");
        else if (symbol.DeclaredAccessibility == Accessibility.Protected)
            modifiers.Add("protected");
        else if (symbol.DeclaredAccessibility == Accessibility.Internal)
            modifiers.Add("internal");

        return modifiers;
    }

    public ParsedEntityInheritance MapInheritance(INamedTypeSymbol typeSymbol)
    {
        var baseClasses = new List<string>();
        var interfaces = new List<string>();

        if (typeSymbol.BaseType != null && typeSymbol.BaseType.SpecialType != SpecialType.System_Object)
        {
            baseClasses.Add(typeSymbol.BaseType.Name);
        }

        foreach (var interfaceSymbol in typeSymbol.AllInterfaces)
        {
            interfaces.Add(interfaceSymbol.Name);
        }

        return new ParsedEntityInheritance(
            baseClasses.Any() ? baseClasses : null,
            interfaces.Any() ? interfaces : null,
            [],
            []
        );
    }

    public string? MapReturnType(ISymbol symbol)
    {
        if (symbol is IMethodSymbol methodSymbol)
        {
            return methodSymbol.ReturnType?.Name;
        }

        if (symbol is IPropertySymbol propertySymbol)
        {
            return propertySymbol.Type?.Name;
        }

        return null;
    }

    public List<ModelParameter> MapParameters(IMethodSymbol methodSymbol)
    {
        var parameters = new List<ModelParameter>();

        foreach (var parameter in methodSymbol.Parameters)
        {
            parameters.Add(new ModelParameter(
                parameter.Name,
                parameter.Type?.Name,
                parameter.HasExplicitDefaultValue,
                parameter.HasExplicitDefaultValue ? parameter.ExplicitDefaultValue?.ToString() : null
            ));
        }

        return parameters;
    }

    public UsingStatementData MapImportData(ISymbol symbol)
    {
        var namespaceSymbol = symbol.ContainingNamespace;
        return new UsingStatementData(namespaceSymbol?.Name);
    }

    public List<Attribute> MapAttributes(ISymbol symbol)
    {
        var attributes = new List<Attribute>();

        foreach (var attribute in symbol.GetAttributes())
        {
            var arguments = new List<string>();
            foreach (var argument in attribute.ConstructorArguments)
            {
                arguments.Add(argument.Value?.ToString() ?? "null");
            }

            attributes.Add(new Attribute(attribute.AttributeClass?.Name, arguments));
        }

        return attributes;
    }

    public ParseErrorSeverity MapSeverity(DiagnosticSeverity diagnosticSeverity)
    {
        return diagnosticSeverity switch
        {
            DiagnosticSeverity.Error => ParseErrorSeverity.Error,
            DiagnosticSeverity.Warning => ParseErrorSeverity.Warning,
            DiagnosticSeverity.Info => ParseErrorSeverity.Info,
            _ => ParseErrorSeverity.Error
        };
    }

    public LocationApplication MapErrorLocation(Location location)
    {
        return MapLocation(location);
    }
}