namespace VelikiyPrikalel.OLLAMACHAT.Application.Services;

public interface IRelationshipService
{
    Task<IEnumerable<Relationship>> AnalyzeRelationshipsAsync(IEnumerable<ParsedEntity> entities, Document document);
}