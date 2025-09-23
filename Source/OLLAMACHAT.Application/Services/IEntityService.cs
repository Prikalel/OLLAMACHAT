using Microsoft.CodeAnalysis;
using VelikiyPrikalel.OLLAMACHAT.Application.Models;

namespace VelikiyPrikalel.OLLAMACHAT.Application.Services;

public interface IEntityService
{
    Task<ParsedEntity> ExtractEntityAsync(ISymbol symbol, SemanticModel semanticModel);
    Task<IEnumerable<ParsedEntity>> ExtractEntitiesAsync(Document document);
    Task<List<ParsedEntity>> ExtractChildEntitiesAsync(ISymbol symbol, SemanticModel semanticModel);
}