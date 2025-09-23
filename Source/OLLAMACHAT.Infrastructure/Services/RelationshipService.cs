using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VelikiyPrikalel.OLLAMACHAT.Application.Models;
using VelikiyPrikalel.OLLAMACHAT.Application.Services;

namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Services;

public class RelationshipService : IRelationshipService
{
    private readonly ILogger<RelationshipService> logger;

    public RelationshipService(ILogger<RelationshipService> logger)
    {
        this.logger = logger;
    }

    public async Task<IEnumerable<Relationship>> AnalyzeRelationshipsAsync(IEnumerable<ParsedEntity> entities, Document document)
    {
        logger.LogInformation("Analyzing relationships for document: {DocumentPath}", document.FilePath);

        try
        {
            var relationships = new List<Relationship>();
            var semanticModel = await document.GetSemanticModelAsync();
            var syntaxTree = await document.GetSyntaxTreeAsync();
            var root = await syntaxTree.GetRootAsync();

            var entityDict = entities.ToDictionary(e => e.Name, e => e);

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
        if (entity.Inheritance?.BaseClasses == null)
            return;

        foreach (var baseClass in entity.Inheritance.BaseClasses)
        {
            if (entityDict.TryGetValue(baseClass, out var baseEntity))
            {
                relationships.Add(new Relationship(
                    From: entity.Name,
                    To: baseEntity.Name,
                    Type: RelationshipType.Inherits,
                    TargetFile: null,
                    Location: entity.Location
                ));
            }
        }
    }

    private void AnalyzeInterfaceImplementationRelationships(ParsedEntity entity, Dictionary<string, ParsedEntity> entityDict, List<Relationship> relationships)
    {
        if (entity.Inheritance?.Interfaces == null)
            return;

        foreach (var interfaceName in entity.Inheritance.Interfaces)
        {
            if (entityDict.TryGetValue(interfaceName, out var interfaceEntity))
            {
                relationships.Add(new Relationship(
                    From: entity.Name,
                    To: interfaceEntity.Name,
                    Type: RelationshipType.Implements,
                    TargetFile: null,
                    Location: entity.Location
                ));
            }
        }
    }

    private void AnalyzeMethodCallRelationships(ParsedEntity methodEntity, Dictionary<string, ParsedEntity> entityDict, SemanticModel semanticModel, SyntaxNode root, List<Relationship> relationships)
    {
        var methodDeclarations = root.DescendantNodes().OfType<MethodDeclarationSyntax>();
        var methodDeclaration = methodDeclarations.FirstOrDefault(m => m.Identifier.Text == methodEntity.Name);

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
                        From: methodEntity.Name,
                        To: calledMethodName,
                        Type: RelationshipType.Calls,
                        TargetFile: null,
                        Location: methodEntity.Location
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
                        From: methodEntity.Name,
                        To: memberName,
                        Type: RelationshipType.References,
                        TargetFile: null,
                        Location: methodEntity.Location
                    ));
                }
            }
        }
    }
}