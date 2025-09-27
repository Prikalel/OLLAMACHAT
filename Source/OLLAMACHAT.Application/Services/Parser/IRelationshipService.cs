namespace VelikiyPrikalel.OLLAMACHAT.Application.Services.Parser;

public interface IRelationshipService
{
    Task<IEnumerable<Relationship>> AnalyzeRelationshipsAsync(IEnumerable<ParsedEntity> entities, Document document);
}