namespace VelikiyPrikalel.OLLAMACHAT.Application.Services.Parser;

public interface IMetadataService
{
    Task<FileMetadata> CalculateFileMetadata(Document document, IEnumerable<ParsedEntity> entities);
}