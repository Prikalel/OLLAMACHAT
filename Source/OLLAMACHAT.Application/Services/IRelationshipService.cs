using Microsoft.CodeAnalysis;
using VelikiyPrikalel.OLLAMACHAT.Application.Models;

namespace VelikiyPrikalel.OLLAMACHAT.Application.Services;

public interface IRelationshipService
{
    Task<IEnumerable<Relationship>> AnalyzeRelationshipsAsync(IEnumerable<ParsedEntity> entities, Document document);
}