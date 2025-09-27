namespace VelikiyPrikalel.OLLAMACHAT.Application.Services;

public interface IDocumentService
{
    Task<Document?> GetDocumentAsync(string filePath, string repoPath);
    Task<string> GetContentHashAsync(Document document);
}