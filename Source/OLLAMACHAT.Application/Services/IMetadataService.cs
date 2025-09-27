namespace VelikiyPrikalel.OLLAMACHAT.Application.Services;

public interface IMetadataService
{
    FileMetadata CalculateFileMetadata(Document document, IEnumerable<ParsedEntity> entities);
}