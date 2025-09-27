namespace VelikiyPrikalel.OLLAMACHAT.Application.Services.Parser;

public interface IMetadataService
{
    FileMetadata CalculateFileMetadata(Document document, IEnumerable<ParsedEntity> entities);
}