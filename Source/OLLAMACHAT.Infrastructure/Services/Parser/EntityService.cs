namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Services.Parser;

/// <inheritdoc />
public class EntityService(
      IMapperService mapperService,
      ILogger<EntityService> logger) : IEntityService
{
    /// <inheritdoc />
    public async Task<IEnumerable<ParsedEntity>> ExtractEntitiesAsync(Document document)
    {
        logger.LogInformation("Extracting entities from document: {DocumentPath}", document.FilePath);

        try
        {
            var semanticModel = await document.GetSemanticModelAsync();
            var syntaxTree = await document.GetSyntaxTreeAsync();
            var root = await syntaxTree.GetRootAsync();

            var entities = new List<ParsedEntity>();

            var namespaceDeclarations = root.DescendantNodes().OfType<BaseNamespaceDeclarationSyntax>();
            foreach (var namespaceDeclaration in namespaceDeclarations)
            {
                var namespaceSymbol = semanticModel.GetDeclaredSymbol(namespaceDeclaration);
                if (namespaceSymbol != null)
                {
                    var namespaceEntity = await ExtractEntityAsync(namespaceSymbol, semanticModel);
                    entities.Add(namespaceEntity);
                }
            }

            var typeDeclarations = root.DescendantNodes().OfType<TypeDeclarationSyntax>();
            foreach (var typeDeclaration in typeDeclarations)
            {
                var typeSymbol = semanticModel.GetDeclaredSymbol(typeDeclaration);
                if (typeSymbol != null)
                {
                    var typeEntity = await ExtractEntityAsync(typeSymbol, semanticModel);
                    entities.Add(typeEntity);
                }
            }

            var methodDeclarations = root.DescendantNodes().OfType<MethodDeclarationSyntax>();
            foreach (var methodDeclaration in methodDeclarations)
            {
                var methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration);
                if (methodSymbol != null && methodSymbol.ContainingType == null)
                {
                    var methodEntity = await ExtractEntityAsync(methodSymbol, semanticModel);
                    entities.Add(methodEntity);
                }
            }

            var propertyDeclarations = root.DescendantNodes().OfType<PropertyDeclarationSyntax>();
            foreach (var propertyDeclaration in propertyDeclarations)
            {
                var propertySymbol = semanticModel.GetDeclaredSymbol(propertyDeclaration);
                if (propertySymbol != null && propertySymbol.ContainingType == null)
                {
                    var propertyEntity = await ExtractEntityAsync(propertySymbol, semanticModel);
                    entities.Add(propertyEntity);
                }
            }

            logger.LogInformation("Successfully extracted {EntityCount} entities from document: {DocumentPath}", entities.Count, document.FilePath);
            return entities;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error extracting entities from document: {DocumentPath}", document.FilePath);
            throw;
        }
    }

    private async Task<ParsedEntity> ExtractEntityAsync(ISymbol symbol, SemanticModel semanticModel)
    {
        logger.LogInformation("Extracting entity for symbol: {SymbolName}", symbol.Name);

        try
        {
            var entityType = DetermineEntityType(symbol);
            var location = mapperService.MapLocation(symbol.Locations.FirstOrDefault());
            var modifiers = mapperService.MapModifiers(symbol).ToList();
            var decorators = mapperService.MapAttributes(symbol);
            var inheritance = symbol is INamedTypeSymbol namedTypeSymbol ? mapperService.MapInheritance(namedTypeSymbol) : null;
            var returnType = mapperService.MapReturnType(symbol);
            var parameters = symbol is IMethodSymbol methodSymbol ? mapperService.MapParameters(methodSymbol) : null;
            var importData = mapperService.MapImportData(symbol);

            var childEntities = await ExtractChildEntitiesAsync(symbol, semanticModel);

            var entity = new ParsedEntity(
                Name: symbol.Name ?? string.Empty,
                Type: entityType,
                Location: location,
                Children: childEntities.Any() ? childEntities : null,
                Modifiers: modifiers.Any() ? modifiers : null,
                Decorators: decorators.Any() ? decorators : null,
                Inheritance: inheritance,
                ReturnType: returnType,
                Parameters: parameters,
                ImportData: importData
            );

            logger.LogInformation("Successfully extracted entity: {SymbolName} of type: {EntityType}", symbol.Name, entityType);
            return entity;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error extracting entity for symbol: {SymbolName}", symbol.Name);
            throw;
        }
    }

    private async Task<List<ParsedEntity>> ExtractChildEntitiesAsync(ISymbol symbol, SemanticModel semanticModel)
    {
        logger.LogInformation("Extracting child entities for symbol: {SymbolName}", symbol.Name);

        try
        {
            var childEntities = new List<ParsedEntity>();

            if (symbol is INamedTypeSymbol namedTypeSymbol)
            {
                foreach (var member in namedTypeSymbol.GetMembers())
                {
                    if (member is IMethodSymbol methodSymbol && methodSymbol.MethodKind == MethodKind.Ordinary)
                    {
                        var methodEntity = await ExtractEntityAsync(methodSymbol, semanticModel);
                        childEntities.Add(methodEntity);
                    }
                    else if (member is IPropertySymbol propertySymbol)
                    {
                        var propertyEntity = await ExtractEntityAsync(propertySymbol, semanticModel);
                        childEntities.Add(propertyEntity);
                    }
                    else if (member is INamedTypeSymbol nestedTypeSymbol)
                    {
                        var nestedTypeEntity = await ExtractEntityAsync(nestedTypeSymbol, semanticModel);
                        childEntities.Add(nestedTypeEntity);
                    }
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

    private ParsedEntityType DetermineEntityType(ISymbol symbol)
    {
        return symbol switch
        {
            INamespaceSymbol => ParsedEntityType.Namespace,
            INamedTypeSymbol namedTypeSymbol when namedTypeSymbol.TypeKind == TypeKind.Class => ParsedEntityType.Class,
            INamedTypeSymbol namedTypeSymbol when namedTypeSymbol.TypeKind == TypeKind.Interface => ParsedEntityType.Interface,
            INamedTypeSymbol namedTypeSymbol when namedTypeSymbol.TypeKind == TypeKind.Enum => ParsedEntityType.Enum,
            INamedTypeSymbol namedTypeSymbol when namedTypeSymbol.TypeKind == TypeKind.Struct => ParsedEntityType.Struct,
            IMethodSymbol => ParsedEntityType.Method,
            IPropertySymbol => ParsedEntityType.Property,
            _ => ParsedEntityType.Class
        };
    }
}