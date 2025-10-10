namespace VelikiyPrikalel.OLLAMACHAT.Application.Services.Parser;

public interface IRelationshipService
{
    Task<IEnumerable<SimpleRelationship>> AnalyzeRelationshipsAsync(IEnumerable<ParsedEntity> entities, Document document);
}
