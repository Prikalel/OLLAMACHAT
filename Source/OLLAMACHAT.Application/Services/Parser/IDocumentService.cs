namespace VelikiyPrikalel.OLLAMACHAT.Application.Services.Parser;

public interface IDocumentService
{
    Task<Document?> GetDocumentAsync(string filePath);
    Task<string> GetContentHashAsync(Document document);
}