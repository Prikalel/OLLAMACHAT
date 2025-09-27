namespace VelikiyPrikalel.OLLAMACHAT.Application.Services.Parser;

public interface IImportService
{
    Task<List<string>> ResolveImportPathAsync(string importPath, Document document);
    Task<List<string>> FindCommonInitFilesAsync(string repoPath);
}