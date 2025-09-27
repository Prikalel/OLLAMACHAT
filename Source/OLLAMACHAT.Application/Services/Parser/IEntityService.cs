namespace VelikiyPrikalel.OLLAMACHAT.Application.Services.Parser;

public interface IEntityService
{
    Task<IEnumerable<ParsedEntity>> ExtractEntitiesAsync(Document document);
}