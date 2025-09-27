namespace VelikiyPrikalel.OLLAMACHAT.Application.Services;

public interface IImportService
{
    Task<List<string>> ResolveImportPathAsync(string importPath, Document document);
    Task<List<string>> FindCommonInitFilesAsync(string repoPath);
    Task<List<string>> GetSupportedExtensionsAsync();
}