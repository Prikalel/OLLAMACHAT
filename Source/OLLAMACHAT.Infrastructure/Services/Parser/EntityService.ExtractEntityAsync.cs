using Attribute = VelikiyPrikalel.OLLAMACHAT.Application.Models.Attribute;
using Location = VelikiyPrikalel.OLLAMACHAT.Application.Models.Location;

namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Services.Parser;

/// <summary>
/// Часть класса EntityService, содержащая метод ExtractEntityAsync
/// </summary>
public partial class EntityService
{
    private async Task<ParsedEntity> ExtractEntityAsync(ISymbol symbol, SemanticModel semanticModel, int currentDepth = 0, ParserOptions? options = null)
    {
        if (symbol == null)
        {
            logger.LogError("Symbol is null, cannot extract entity");
            throw new ArgumentNullException(nameof(symbol));
        }

        if (semanticModel == null)
        {
            logger.LogError("Semantic model is null for symbol: {SymbolName}", symbol.Name);
            throw new ArgumentNullException(nameof(semanticModel));
        }

        if (string.IsNullOrEmpty(symbol.Name))
        {
            logger.LogError("Symbol name is null or empty for symbol: {SymbolDisplay}", symbol.GetFullName());
        }

        if (!symbol.Locations.Any())
        {
            logger.LogWarning("Symbol has no locations: {SymbolName}", symbol.Name);
        }

        DateTime extractionStartTime = DateTime.UtcNow;
        logger.LogInformation("Starting entity extraction for symbol: {SymbolName} of kind: {SymbolKind} containing namespace: {ContainingNamespace} at {StartTime}",
            symbol.Name, symbol.Kind, symbol.ContainingNamespace?.Name ?? "Global", extractionStartTime);

        try
        {
            logger.LogDebug("Determining entity type for symbol: {SymbolName}", symbol.Name);
            ParsedEntityType entityType = DetermineEntityType(symbol);

            logger.LogDebug("Mapping basic properties for symbol: {SymbolName}", symbol.Name);
            Location location = mapperService.MapLocation(symbol.Locations.First());
            List<string> modifiers = mapperService.MapModifiers(symbol)?.ToList() ?? new List<string>();
            List<Attribute> attributes = mapperService.MapAttributes(symbol) ?? new List<Attribute>();
            ParsedEntityInheritance? inheritance = symbol is INamedTypeSymbol namedTypeSymbol ? mapperService.MapInheritance(namedTypeSymbol, options) : null;
            string? returnType = mapperService.MapReturnType(symbol);
            List<ModelParameter>? parameters = symbol is IMethodSymbol methodSymbol ? mapperService.MapParameters(methodSymbol) : null;

            logger.LogDebug("Checking for UnityEvent fields for symbol: {SymbolName}", symbol.Name);
            bool isUnityEvent = IsUnityEventField(symbol);

            logger.LogDebug("Extracting child entities for symbol: {SymbolName}", symbol.Name);
            DateTime childEntitiesStartTime = DateTime.UtcNow;
            List<ParsedEntity> childEntities = await ExtractChildEntitiesAsync(symbol, semanticModel, currentDepth, options);
            TimeSpan childEntitiesTime = DateTime.UtcNow - childEntitiesStartTime;
            logger.LogDebug("Extracted {ChildCount} child entities for symbol: {SymbolName} in {ElapsedMs}ms",
                childEntities.Count, symbol.Name, childEntitiesTime.TotalMilliseconds);

            logger.LogDebug("Extracting attributes for symbol: {SymbolName}", symbol.Name);

            if (isUnityEvent)
            {
                entityType = ParsedEntityType.UnityEvent;
            }

            logger.LogDebug("Creating ParsedEntity for symbol: {SymbolName} with {AttributeCount} attributes",
                symbol.Name, attributes.Count);

            ParsedEntity entity = new ParsedEntity(
                SimpleName: symbol.Name ?? string.Empty,
                FullName: symbol.GetFullName(),
                Type: entityType,
                Location: location,
                Children: childEntities.Any() ? childEntities : null,
                Modifiers: modifiers.Any() ? modifiers : null,
                Attributes: attributes.Any() ? attributes : null,
                Inheritance: inheritance,
                ReturnType: returnType,
                Parameters: parameters,
                UsingStatementData: null
            );

            DateTime extractionEndTime = DateTime.UtcNow;
            TimeSpan extractionTime = extractionEndTime - extractionStartTime;

            logger.LogInformation("Successfully extracted entity: {SymbolName} of type: {EntityType} in {ElapsedMs}ms",
                symbol.Name, entityType, extractionTime.TotalMilliseconds);

            // Детальная информация о созданной сущности
            logger.LogDebug("Entity details for {SymbolName}: Type={EntityType}, Children={ChildCount}, " +
                           "Attributes={AttributeCount}, Modifiers={ModifierCount}",
                symbol.Name, entityType, childEntities.Count, attributes.Count, modifiers.Count);

            return entity;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error extracting entity for symbol: {SymbolName}", symbol.Name);
            throw;
        }
    }

    private async Task<List<ParsedEntity>> ExtractChildEntitiesAsync(ISymbol symbol, SemanticModel semanticModel, int currentDepth, ParserOptions? options = null)
    {
        if (symbol == null)
        {
            logger.LogError("Symbol is null in ExtractChildEntitiesAsync");
            return new List<ParsedEntity>();
        }

        if (semanticModel == null)
        {
            logger.LogError("Semantic model is null in ExtractChildEntitiesAsync for symbol: {SymbolName}", symbol.Name);
            return new List<ParsedEntity>();
        }

        if (symbol.Name != null && ContainsSpecialCharacters(symbol.Name))
        {
            logger.LogWarning("Symbol contains special characters: {SymbolName}", symbol.Name);
        }

        logger.LogInformation("Extracting child entities for symbol: {SymbolName} of kind: {SymbolKind}", symbol.Name, symbol.Kind);

        if (options?.MaxDepth != null && currentDepth >= options.MaxDepth.Value)
        {
            logger.LogTrace("Maximum depth ({MaxDepth}) reached for symbol: {SymbolName} at depth {CurrentDepth}",
                options.MaxDepth.Value, symbol.Name, currentDepth);
            return new List<ParsedEntity>();
        }

        try
        {
            List<ParsedEntity> childEntities = new();
            HashSet<string> processedSymbols = new(); // Защита от рекурсивных зависимостей
            List<Task<ParsedEntity>> childEntityTasks = new();

            // Special handling for properties - extract get/set methods as children
            if (symbol is IPropertySymbol propertySymbol)
            {
                logger.LogDebug("Processing property symbol: {PropertyName}, extracting get/set methods", propertySymbol.Name);

                if (propertySymbol.GetMethod != null)
                {
                    logger.LogDebug("Found get method for property: {PropertyName}", propertySymbol.Name);
                    ParsedEntity getMethodEntity = await ExtractEntityAsync(propertySymbol.GetMethod, semanticModel, currentDepth + 1, options);
                    childEntities.Add(getMethodEntity);
                }

                if (propertySymbol.SetMethod != null)
                {
                    logger.LogDebug("Found set method for property: {PropertyName}", propertySymbol.Name);
                    ParsedEntity setMethodEntity = await ExtractEntityAsync(propertySymbol.SetMethod, semanticModel, currentDepth + 1, options);
                    childEntities.Add(setMethodEntity);
                }
            }
            else if (symbol is INamedTypeSymbol || symbol is INamespaceSymbol)
            {
                List<ISymbol> allMembers = symbol is INamedTypeSymbol typeSymbol
                    ? typeSymbol!.GetMembers().ToList()
                    : symbol is INamespaceSymbol namespaceSymbol
                        ? namespaceSymbol.GetMembers().OfType<ISymbol>().ToList()
                        : throw new NotImplementedException();

                foreach (ISymbol member in allMembers)
                {
                    string memberKey = $"{member.Name}_{member.Kind}";
                    if (!processedSymbols.Add(memberKey))
                    {
                        logger.LogTrace("Detected potential recursive dependency for member: {MemberName}", member.Name);
                        continue;
                    }

                    if (member.IsOverride && !ShouldIncludeOverrideMember(member))
                    {
                        continue;
                    }

                    // Skip compiler-generated members
                    if (member.IsImplicitlyDeclared && !ShouldIncludeImplicitMember(member))
                    {
                        continue;
                    }

                    // Skip property accessors (get/set methods) as they are handled separately
                    if (member is IMethodSymbol methodSymbol &&
                        (methodSymbol.MethodKind == MethodKind.PropertyGet || methodSymbol.MethodKind == MethodKind.PropertySet))
                    {
                        logger.LogTrace("Skipping property accessor method: {MethodName} as it's handled separately", methodSymbol.Name);
                        continue;
                    }

                    // Extract methods (including constructors, destructors, operators)
                    if (member is IMethodSymbol)
                    {
                        childEntityTasks.Add(ExtractEntityAsync(member, semanticModel, currentDepth + 1, options));
                    }
                    // Extract properties
                    else if (member is IPropertySymbol)
                    {
                        childEntityTasks.Add(ExtractEntityAsync(member, semanticModel, currentDepth + 1, options));
                    }
                    // Extract fields
                    else if (member is IFieldSymbol)
                    {
                        childEntityTasks.Add(ExtractEntityAsync(member, semanticModel, currentDepth + 1, options));
                    }
                    // Extract events
                    else if (member is IEventSymbol)
                    {
                        childEntityTasks.Add(ExtractEntityAsync(member, semanticModel, currentDepth + 1, options));
                    }
                    // Extract nested types
                    else if (member is INamedTypeSymbol)
                    {
                        childEntityTasks.Add(ExtractEntityAsync(member, semanticModel, currentDepth + 1, options));
                    }
                }

                if (childEntityTasks.Count > 0)
                {
                    ParsedEntity[] childResults = await Task.WhenAll(childEntityTasks);
                    childEntities.AddRange(childResults);
                }
            }

            logger.LogInformation("Successfully extracted {ChildCount} child entities for symbol: {SymbolName}", childEntities.Count, symbol.Name);
            return childEntities;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error extracting child entities for symbol: {SymbolName}", symbol.Name);
            throw;
        }
    }

    /// <summary>
    /// Определяет, следует ли включать переопределенный член
    /// </summary>
    private static bool ShouldIncludeOverrideMember(ISymbol member)
    {
        return IsImportantOverrideMethod(member);
    }

    /// <summary>
    /// Определяет, следует ли включать неявно объявленный член
    /// </summary>
    private static bool ShouldIncludeImplicitMember(ISymbol member)
    {
        if (member is IMethodSymbol methodSymbol)
        {
            if (methodSymbol.MethodKind == MethodKind.Constructor &&
                methodSymbol.ContainingType.IsRecord)
            {
                return true;
            }
        }

        return false;
    }

    private ParsedEntityType DetermineEntityType(ISymbol symbol) =>
        symbol switch
        {
            INamespaceSymbol => ParsedEntityType.Namespace,
            INamedTypeSymbol namedTypeSymbol => namedTypeSymbol.TypeKind switch
            {
                TypeKind.Class => ParsedEntityType.Class,
                TypeKind.Interface => ParsedEntityType.Interface,
                TypeKind.Enum => ParsedEntityType.Enum,
                TypeKind.Struct => ParsedEntityType.Struct,
                _ => ParsedEntityType.Class
            },
            IMethodSymbol methodSymbol => methodSymbol.MethodKind switch
            {
                _ => ParsedEntityType.Method
            },
            IPropertySymbol => ParsedEntityType.Property,
            IFieldSymbol fieldSymbol => fieldSymbol.ContainingType is INamedTypeSymbol { TypeKind: TypeKind.Enum }
                ? ParsedEntityType.Enum
                : ParsedEntityType.Property,
            IEventSymbol => ParsedEntityType.Property, // Using existing enum value
            _ => ParsedEntityType.Class
        };

    private bool IsUnityEventField(ISymbol symbol)
    {
        if (symbol is not IFieldSymbol fieldSymbol)
        {
            return false;
        }

        string fieldType = fieldSymbol.Type.GetFullName();

        if (unityEventNames.Contains(fieldType))
        {
            return true;
        }

        if (unityEventRegex.IsMatch(fieldType))
        {
            return true;
        }

        if (fieldSymbol.Type.AllInterfaces.Any(i => i.Name.Contains("UnityEvent")))
        {
            return true;
        }

        return false;
    }
}
