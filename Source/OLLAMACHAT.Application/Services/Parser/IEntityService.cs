namespace VelikiyPrikalel.OLLAMACHAT.Application.Services.Parser;

public interface IEntityService
{
    Task<ParsedEntity> ExtractEntityAsync(ISymbol symbol, SemanticModel semanticModel);
    Task<IEnumerable<ParsedEntity>> ExtractEntitiesAsync(Document document);
    Task<List<ParsedEntity>> ExtractChildEntitiesAsync(ISymbol symbol, SemanticModel semanticModel);
}