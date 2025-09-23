using Microsoft.CodeAnalysis;
using VelikiyPrikalel.OLLAMACHAT.Application.Models;

namespace VelikiyPrikalel.OLLAMACHAT.Application.Services;

public interface IMetadataService
{
    FileMetadata CalculateFileMetadata(Document document, IEnumerable<ParsedEntity> entities);
}