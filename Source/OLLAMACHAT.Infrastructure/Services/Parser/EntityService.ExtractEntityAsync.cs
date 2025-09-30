using Attribute = VelikiyPrikalel.OLLAMACHAT.Application.Models.Attribute;

namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Services.Parser;

/// <summary>
/// Часть класса EntityService, содержащая метод ExtractEntityAsync
/// </summary>
public partial class EntityService
{
    private async Task<ParsedEntity> ExtractEntityAsync(ISymbol symbol, SemanticModel semanticModel, int currentDepth = 0, ParserOptions? options = null)
    {
        // Проверка на null символ
        if (symbol == null)
        {
            logger.LogWarning("Symbol is null, cannot extract entity");
            throw new ArgumentNullException(nameof(symbol));
        }

        // Проверка на null семантическую модель
        if (semanticModel == null)
        {
            logger.LogWarning("Semantic model is null for symbol: {SymbolName}", symbol.Name);
            throw new ArgumentNullException(nameof(semanticModel));
        }

        // Проверка на пустое имя символа
        if (string.IsNullOrEmpty(symbol.Name))
        {
            logger.LogWarning("Symbol name is null or empty for symbol: {SymbolDisplay}", symbol.ToDisplayString());
        }

        // Проверка на отсутствие локаций
        if (!symbol.Locations.Any())
        {
            logger.LogWarning("Symbol has no locations: {SymbolName}", symbol.Name);
        }

        var extractionStartTime = DateTime.UtcNow;
        logger.LogDebug("Starting entity extraction for symbol: {SymbolName} of kind: {SymbolKind} at {StartTime}",
            symbol.Name, symbol.Kind, extractionStartTime);

        try
        {
            logger.LogDebug("Determining entity type for symbol: {SymbolName}", symbol.Name);
            var entityType = DetermineEntityType(symbol);

            logger.LogDebug("Mapping basic properties for symbol: {SymbolName}", symbol.Name);
            var location = mapperService.MapLocation(symbol.Locations.FirstOrDefault());
            var modifiers = mapperService.MapModifiers(symbol)?.ToList() ?? new List<string>();
            var attributes = mapperService.MapAttributes(symbol) ?? new List<Attribute>();
            var inheritance = symbol is INamedTypeSymbol namedTypeSymbol ? mapperService.MapInheritance(namedTypeSymbol, options) : null;
            var returnType = mapperService.MapReturnType(symbol);
            var parameters = symbol is IMethodSymbol methodSymbol ? mapperService.MapParameters(methodSymbol) : null;

            logger.LogDebug("Checking for UnityEvent fields for symbol: {SymbolName}", symbol.Name);
            var isUnityEvent = IsUnityEventField(symbol);

            logger.LogDebug("Extracting child entities for symbol: {SymbolName}", symbol.Name);
            var childEntitiesStartTime = DateTime.UtcNow;
            var childEntities = await ExtractChildEntitiesAsync(symbol, semanticModel, currentDepth, options);
            var childEntitiesTime = DateTime.UtcNow - childEntitiesStartTime;
            logger.LogDebug("Extracted {ChildCount} child entities for symbol: {SymbolName} in {ElapsedMs}ms",
                childEntities.Count, symbol.Name, childEntitiesTime.TotalMilliseconds);

            // Extract inheritance entities if ExtractFullExtractInheritance is true
            List<ParsedEntity> inheritanceEntities = new();
            if (symbol is INamedTypeSymbol typeSymbol &&
                (options?.ExtractFullExtractInheritance ?? true))
            {
                logger.LogDebug("Extracting inheritance entities for symbol: {SymbolName}", symbol.Name);
                var inheritanceEntitiesStartTime = DateTime.UtcNow;
                var processedEntities = new HashSet<string>();
                inheritanceEntities = await ExtractInheritanceEntitiesAsync(typeSymbol, semanticModel, currentDepth, options, processedEntities);
                var inheritanceEntitiesTime = DateTime.UtcNow - inheritanceEntitiesStartTime;
                logger.LogDebug("Extracted {InheritanceCount} inheritance entities for symbol: {SymbolName} in {ElapsedMs}ms",
                    inheritanceEntities.Count, symbol.Name, inheritanceEntitiesTime.TotalMilliseconds);
            }

            logger.LogDebug("Extracting attributes for symbol: {SymbolName}", symbol.Name);

            // Use only real C# attributes from the symbol
            var allAttributes = attributes ?? new List<Attribute>();

            // Add special attribute for UnityEvent fields
            if (isUnityEvent)
            {
                allAttributes.Add(new Attribute("UnityEvent", ["Unity Event Field"]));
            }

            logger.LogDebug("Creating ParsedEntity for symbol: {SymbolName} with {AttributeCount} attributes",
                symbol.Name, allAttributes.Count);

            var entity = new ParsedEntity(
                SimpleName: symbol.Name ?? string.Empty,
                FullName: symbol.ToDisplayString(),
                Type: entityType,
                Location: location,
                Children: childEntities.Any() ? childEntities : null,
                Modifiers: modifiers.Any() ? modifiers : null,
                Attributes: allAttributes.Any() ? allAttributes : null,
                Inheritance: inheritance,
                ReturnType: returnType,
                Parameters: parameters,
                UsingStatementData: null
            );

            var extractionEndTime = DateTime.UtcNow;
            var extractionTime = extractionEndTime - extractionStartTime;

            logger.LogInformation("Successfully extracted entity: {SymbolName} of type: {EntityType} in {ElapsedMs}ms",
                symbol.Name, entityType, extractionTime.TotalMilliseconds);

            // Детальная информация о созданной сущности
            logger.LogDebug("Entity details for {SymbolName}: Type={EntityType}, Children={ChildCount}, " +
                           "Attributes={AttributeCount}, Modifiers={ModifierCount}",
                symbol.Name, entityType, childEntities.Count, allAttributes.Count, modifiers.Count);

            return entity;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error extracting entity for symbol: {SymbolName}", symbol.Name);
            throw;
        }
    }
}