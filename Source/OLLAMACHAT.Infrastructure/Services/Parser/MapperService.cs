using Attribute = VelikiyPrikalel.OLLAMACHAT.Application.Models.Attribute;
using Location = Microsoft.CodeAnalysis.Location;
using LocationApplication = VelikiyPrikalel.OLLAMACHAT.Application.Models.Location;

namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Services.Parser;

public class MapperService(ILogger<MapperService> logger) : IMapperService
{
    public LocationApplication MapLocation(Location location)
    {
        try
        {
            if (location == null)
            {
                logger.LogError("Location null!");
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
        catch (Exception ex)
        {
            logger.LogError(ex, "Error mapping location");
            return new LocationApplication(new Position(null, null, null), new Position(null, null, null));
        }
    }

    public IEnumerable<string> MapModifiers(ISymbol symbol)
    {
        try
        {
            var modifiers = new List<string>();

            if (symbol == null)
                return modifiers;

            // Accessibility modifiers
            switch (symbol.DeclaredAccessibility)
            {
                case Accessibility.Public:
                    modifiers.Add("public");
                    break;
                case Accessibility.Private:
                    modifiers.Add("private");
                    break;
                case Accessibility.Protected:
                    modifiers.Add("protected");
                    break;
                case Accessibility.Internal:
                    modifiers.Add("internal");
                    break;
            }

            // Type-specific modifiers
            if (symbol is ITypeSymbol typeSymbol)
            {
                if (typeSymbol.IsStatic)
                    modifiers.Add("static");
                if (typeSymbol is { IsAbstract: true, TypeKind: not TypeKind.Interface }) // интерфейс всегда абстрактный
                    modifiers.Add("abstract");
                if (typeSymbol.IsSealed)
                    modifiers.Add("sealed");
                if (typeSymbol.IsReadOnly)
                    modifiers.Add("readonly");
                if (typeSymbol.IsValueType)
                    modifiers.Add("struct");
                if (typeSymbol.TypeKind is TypeKind.Class)
                    modifiers.Add("class");
                if (typeSymbol.TypeKind is TypeKind.Interface)
                    modifiers.Add("interface");
                if (typeSymbol.IsAnonymousType)
                    modifiers.Add("anonymous");
                if (typeSymbol.IsTupleType)
                    modifiers.Add("tuple");
            }

            // Method-specific modifiers
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
                if (methodSymbol.IsExtensionMethod)
                    modifiers.Add("extension");
                if (methodSymbol.IsExtern)
                    modifiers.Add("extern");
                if (methodSymbol.IsGenericMethod)
                    modifiers.Add("generic");
            }

            // Property-specific modifiers
            if (symbol is IPropertySymbol propertySymbol)
            {
                if (propertySymbol.IsStatic)
                    modifiers.Add("static");
                if (propertySymbol.IsVirtual)
                    modifiers.Add("virtual");
                if (propertySymbol.IsOverride)
                    modifiers.Add("override");
                if (propertySymbol.IsAbstract)
                    modifiers.Add("abstract");
                if (propertySymbol.IsSealed)
                    modifiers.Add("sealed");
                if (propertySymbol.IsReadOnly)
                    modifiers.Add("readonly");
                if (propertySymbol.IsRequired)
                    modifiers.Add("required");
            }

            // Field-specific modifiers
            if (symbol is IFieldSymbol fieldSymbol)
            {
                if (fieldSymbol.IsStatic)
                    modifiers.Add("static");
                if (fieldSymbol.IsReadOnly)
                    modifiers.Add("readonly");
                if (fieldSymbol.IsVolatile)
                    modifiers.Add("volatile");
                if (fieldSymbol.IsConst)
                    modifiers.Add("const");
                if (fieldSymbol.IsRequired)
                    modifiers.Add("required");
            }

            // Event-specific modifiers
            if (symbol is IEventSymbol eventSymbol)
            {
                if (eventSymbol.IsStatic)
                    modifiers.Add("static");
                if (eventSymbol.IsVirtual)
                    modifiers.Add("virtual");
                if (eventSymbol.IsOverride)
                    modifiers.Add("override");
                if (eventSymbol.IsAbstract)
                    modifiers.Add("abstract");
                if (eventSymbol.IsSealed)
                    modifiers.Add("sealed");
            }

            return modifiers;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error mapping modifiers for symbol: {SymbolName}", symbol?.Name);
            return Enumerable.Empty<string>();
        }
    }

    public ParsedEntityInheritance MapInheritance(INamedTypeSymbol typeSymbol, ParserOptions? options = null)
    {
        try
        {
            var baseClasses = new List<string>();
            var interfaces = new List<string>();
            var allBaseClasses = new List<string>();
            var allInterfaces = new List<string>();

            var extractFullInheritance = options?.ExtractFullExtractInheritance ?? true;

            if (typeSymbol == null)
            {
                logger.LogError("TypeSymbol is null!");
                return new ParsedEntityInheritance(null, null, [], []);
            }

            if (typeSymbol.BaseType != null && typeSymbol.BaseType.SpecialType != SpecialType.System_Object)
            {
                var baseClassName = typeSymbol.BaseType.GetFullName();
                baseClasses.Add(baseClassName);
                if (extractFullInheritance)
                {
                    allBaseClasses.Add(baseClassName);
                }
            }

            if (extractFullInheritance)
            {
                var currentBase = typeSymbol.BaseType;
                while (currentBase != null && currentBase.SpecialType != SpecialType.System_Object)
                {
                    var baseClassName = currentBase.GetFullName();
                    if (!allBaseClasses.Contains(baseClassName))
                    {
                        allBaseClasses.Add(baseClassName);
                    }
                    currentBase = currentBase.BaseType;
                }
            }

            foreach (var interfaceSymbol in typeSymbol.Interfaces)
            {
                var interfaceName = interfaceSymbol.GetFullName();
                interfaces.Add(interfaceName);
                if (extractFullInheritance && !allInterfaces.Contains(interfaceName))
                {
                    allInterfaces.Add(interfaceName);
                }
            }

            if (extractFullInheritance)
            {
                foreach (var interfaceSymbol in typeSymbol.AllInterfaces)
                {
                    var interfaceName = interfaceSymbol.GetFullName();
                    if (!allInterfaces.Contains(interfaceName))
                    {
                        allInterfaces.Add(interfaceName);
                    }
                }
            }

            return new ParsedEntityInheritance(
                baseClasses.Any() ? baseClasses : null,
                interfaces.Any() ? interfaces : null,
                extractFullInheritance ? allBaseClasses : [],
                extractFullInheritance ? allInterfaces : []
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error mapping inheritance for type: {TypeName}", typeSymbol?.Name);
            return new ParsedEntityInheritance(null, null, [], []);
        }
    }

    public string? MapReturnType(ISymbol symbol)
    {
        try
        {
            if (symbol == null)
                return null;

            if (symbol is IMethodSymbol methodSymbol)
            {
                return methodSymbol.ReturnType?.GetFullName();
            }

            if (symbol is IPropertySymbol propertySymbol)
            {
                return propertySymbol.Type?.GetFullName();
            }

            if (symbol is IFieldSymbol fieldSymbol)
            {
                return fieldSymbol.Type?.GetFullName();
            }

            if (symbol is IEventSymbol eventSymbol)
            {
                return eventSymbol.Type?.GetFullName();
            }

            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error mapping return type for symbol: {SymbolName}", symbol?.Name);
            return null;
        }
    }

    public List<ModelParameter> MapParameters(IMethodSymbol methodSymbol)
    {
        try
        {
            var parameters = new List<ModelParameter>();

            if (methodSymbol == null)
                return parameters;

            foreach (var parameter in methodSymbol.Parameters)
            {
                var parameterType = parameter.Type?.GetFullName() ?? "unknown";
                var isOptional = parameter.HasExplicitDefaultValue;
                var defaultValue = isOptional ? parameter.ExplicitDefaultValue?.ToString() : null;

                // Parameter modifiers
                var refKind = parameter.RefKind;
                var refKindString = refKind switch
                {
                    RefKind.Ref => "ref",
                    RefKind.Out => "out",
                    RefKind.In => "in",
                    _ => ""
                };

                // This, params, and other special parameters
                var isThis = parameter.IsThis;
                var isParams = parameter.IsParams;

                // Enhanced parameter type information
                var enhancedParameterType = parameterType;
                if (!string.IsNullOrEmpty(refKindString))
                {
                    enhancedParameterType = $"{refKindString} {parameterType}";
                }

                if (isParams)
                {
                    enhancedParameterType = $"params {enhancedParameterType}";
                }

                if (isThis)
                {
                    enhancedParameterType = $"this {enhancedParameterType}";
                }

                // Check for nullable reference types
                if (parameter.Type?.NullableAnnotation == NullableAnnotation.Annotated)
                {
                    enhancedParameterType += "?";
                }

                // Check for generic type parameters
                if (parameter.Type is ITypeParameterSymbol typeParam)
                {
                    var constraints = new List<string>();
                    if (typeParam.HasReferenceTypeConstraint)
                        constraints.Add("class");
                    if (typeParam.HasValueTypeConstraint)
                        constraints.Add("struct");
                    if (typeParam.HasNotNullConstraint)
                        constraints.Add("notnull");
                    if (typeParam.HasUnmanagedTypeConstraint)
                        constraints.Add("unmanaged");

                    var constraintTypes = typeParam.ConstraintTypes.Select(t => t.Name);
                    constraints.AddRange(constraintTypes);

                    if (constraints.Any())
                    {
                        enhancedParameterType += $" where {typeParam.Name} : {string.Join(", ", constraints)}";
                    }
                }

                // Check for tuple types
                if (parameter.Type.IsTupleType && parameter.Type is INamedTypeSymbol tupleType)
                {
                    var tupleElements = tupleType.TupleElements;
                    var elementNames = tupleElements.Select(e => e.Name);
                    var elementTypes = tupleElements.Select(e => e.Type.Name);

                    enhancedParameterType += $" ({string.Join(", ", elementNames.Select((n, i) => $"{n}:{elementTypes.ElementAt(i)}"))})";
                }

                parameters.Add(new ModelParameter(
                    parameter.Name,
                    enhancedParameterType,
                    isOptional,
                    defaultValue
                ));
            }

            return parameters;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error mapping parameters for method: {MethodName}", methodSymbol?.Name);
            return new List<ModelParameter>();
        }
    }

    public List<Attribute> MapAttributes(ISymbol symbol)
    {
        try
        {
            var attributes = new List<Attribute>();

            if (symbol == null)
                return attributes;

            foreach (var attribute in symbol.GetAttributes())
            {
                var attributeName = attribute.AttributeClass?.GetFullName() ?? "unknown";
                var constructorArguments = new List<string>();
                var namedArguments = new Dictionary<string, string>();

                // Constructor arguments
                foreach (var argument in attribute.ConstructorArguments)
                {
                    constructorArguments.Add(FormatAttributeArgument(argument));
                }

                // Named arguments
                foreach (var namedArgument in attribute.NamedArguments)
                {
                    namedArguments[namedArgument.Key] = FormatAttributeArgument(namedArgument.Value);
                }

                // Combine constructor arguments and named arguments
                var allArguments = new List<string>(constructorArguments);

                // Add named arguments in a readable format
                foreach (var namedArg in namedArguments)
                {
                    allArguments.Add($"{namedArg.Key} = {namedArg.Value}");
                }

                attributes.Add(new Attribute(attributeName, allArguments));
            }

            return attributes;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error mapping attributes for symbol: {SymbolName}", symbol?.Name);
            return new List<Attribute>();
        }
    }

    public ParseErrorSeverity MapSeverity(DiagnosticSeverity diagnosticSeverity)
    {
        try
        {
            return diagnosticSeverity switch
            {
                DiagnosticSeverity.Error => ParseErrorSeverity.Error,
                DiagnosticSeverity.Warning => ParseErrorSeverity.Warning,
                DiagnosticSeverity.Info => ParseErrorSeverity.Info,
                DiagnosticSeverity.Hidden => ParseErrorSeverity.Info,
                _ => ParseErrorSeverity.Error
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error mapping diagnostic severity: {Severity}", diagnosticSeverity);
            return ParseErrorSeverity.Error;
        }
    }

    public LocationApplication MapErrorLocation(Location location)
    {
        return MapLocation(location);
    }

    private string FormatAttributeArgument(TypedConstant argument)
    {
        try
        {
            if (argument.IsNull)
                return "null";

            if (argument.Kind == TypedConstantKind.Array)
            {
                var arrayElements = argument.Values.Select(FormatAttributeArgument);
                return $"[{string.Join(", ", arrayElements)}]";
            }

            if (argument.Kind == TypedConstantKind.Enum)
            {
                return $"{argument.Type?.GetFullName()}.{argument.Value}";
            }

            if (argument.Type?.SpecialType == SpecialType.System_String)
            {
                return $"\"{argument.Value}\"";
            }

            if (argument.Type?.SpecialType == SpecialType.System_Char)
            {
                return $"'{argument.Value}'";
            }

            if (argument.Type?.SpecialType == SpecialType.System_Boolean)
            {
                return argument.Value?.ToString().ToLower() ?? "false";
            }

            return argument.Value?.ToString() ?? "null";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error formatting attribute argument");
            return "error";
        }
    }
}
