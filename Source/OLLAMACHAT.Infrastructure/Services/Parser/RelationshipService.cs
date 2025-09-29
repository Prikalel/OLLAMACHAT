namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Services.Parser;

[UsedImplicitly]
public class RelationshipService(ILogger<RelationshipService> logger) : IRelationshipService
{
    //TODO: в некоторых местах устанавливается null
    public async Task<IEnumerable<Relationship>> AnalyzeRelationshipsAsync(IEnumerable<ParsedEntity> entities, Document document)
    {
        logger.LogInformation("Analyzing relationships for document: {DocumentPath}", document.FilePath);

        try
        {
            var relationships = new List<Relationship>();
            var semanticModel = await document.GetSemanticModelAsync();
            var syntaxTree = await document.GetSyntaxTreeAsync();
            var root = await syntaxTree.GetRootAsync();

            var entityDict = entities.ToDictionary(e => e.SimpleName, e => e);

            foreach (var entity in entities)
            {
                if (entity.Type == ParsedEntityType.Class || entity.Type == ParsedEntityType.Interface)
                {
                    AnalyzeInheritanceRelationships(entity, entityDict, relationships);
                    AnalyzeInterfaceImplementationRelationships(entity, entityDict, relationships);
                }

                if (entity.Type == ParsedEntityType.Method)
                {
                    AnalyzeMethodCallRelationships(entity, entityDict, semanticModel, root, relationships);
                }
            }

            logger.LogInformation("Successfully analyzed {RelationshipCount} relationships for document: {DocumentPath}", relationships.Count, document.FilePath);
            return relationships;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error analyzing relationships for document: {DocumentPath}", document.FilePath);
            throw;
        }
    }

    private void AnalyzeInheritanceRelationships(ParsedEntity entity, Dictionary<string, ParsedEntity> entityDict, List<Relationship> relationships)
    {
        if (entity.Inheritance?.DirectBaseClasses == null)
            return;

        foreach (var baseClass in entity.Inheritance.AllBaseClasses)
        {
            if (entityDict.TryGetValue(baseClass, out var baseEntity))
            {
                relationships.Add(new Relationship(
                    FullNameFrom: entity.FullName,
                    FullNameTo: baseEntity.FullName,
                    Type: RelationshipType.Inherits,
                    TargetDefinitionFilePath: null
                ));
            }
        }
    }

    private void AnalyzeInterfaceImplementationRelationships(ParsedEntity entity, Dictionary<string, ParsedEntity> entityDict, List<Relationship> relationships)
    {
        if (entity.Inheritance?.DirectInterfaces == null)
            return;

        foreach (var interfaceName in entity.Inheritance.AllInterfaces)
        {
            if (entityDict.TryGetValue(interfaceName, out var interfaceEntity))
            {
                relationships.Add(new Relationship(
                    FullNameFrom: entity.FullName,
                    FullNameTo: interfaceEntity.FullName,
                    Type: RelationshipType.Implements,
                    TargetDefinitionFilePath: null
                ));
            }
        }
    }

    private void AnalyzeMethodCallRelationships(ParsedEntity methodEntity, Dictionary<string, ParsedEntity> entityDict, SemanticModel semanticModel, SyntaxNode root, List<Relationship> relationships)
    {
        var methodDeclarations = root.DescendantNodes().OfType<MethodDeclarationSyntax>();
        var methodDeclaration = methodDeclarations.FirstOrDefault(m => m.Identifier.Text == methodEntity.FullName);

        if (methodDeclaration == null)
            return;

        var invocationExpressions = methodDeclaration.DescendantNodes().OfType<InvocationExpressionSyntax>();

        foreach (var invocation in invocationExpressions)
        {
            var symbolInfo = semanticModel.GetSymbolInfo(invocation.Expression);
            var methodSymbol = symbolInfo.Symbol as IMethodSymbol;

            if (methodSymbol != null)
            {
                var calledMethodName = methodSymbol.Name;
                var containingTypeName = methodSymbol.ContainingType?.Name;

                if (!string.IsNullOrEmpty(containingTypeName) && entityDict.TryGetValue(containingTypeName, out var containingTypeEntity))
                {
                    relationships.Add(new Relationship(
                        FullNameFrom: methodEntity.FullName,
                        FullNameTo: calledMethodName,
                        Type: RelationshipType.Calls,
                        TargetDefinitionFilePath: null
                    ));
                }
            }
        }

        var memberAccessExpressions = methodDeclaration.DescendantNodes().OfType<MemberAccessExpressionSyntax>();

        foreach (var memberAccess in memberAccessExpressions)
        {
            var symbolInfo = semanticModel.GetSymbolInfo(memberAccess.Name);
            var propertySymbol = symbolInfo.Symbol as IPropertySymbol;
            var fieldSymbol = symbolInfo.Symbol as IFieldSymbol;
            var eventSymbol = symbolInfo.Symbol as IEventSymbol;

            if (propertySymbol != null || fieldSymbol != null || eventSymbol != null)
            {
                var memberName = symbolInfo.Symbol?.Name;
                var containingTypeName = symbolInfo.Symbol?.ContainingType?.Name;

                if (!string.IsNullOrEmpty(memberName) && !string.IsNullOrEmpty(containingTypeName) &&
                    entityDict.TryGetValue(containingTypeName, out var containingTypeEntity))
                {
                    relationships.Add(new Relationship(
                        FullNameFrom: methodEntity.FullName,
                        FullNameTo: memberName,
                        Type: RelationshipType.Calls,
                        TargetDefinitionFilePath: null
                    ));
                }
            }
        }
    }
}