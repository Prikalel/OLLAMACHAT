namespace VelikiyPrikalel.OLLAMACHAT.Application.Services;

public interface IRoslynParsingService
{
    Task<ParseResult> ParseFileAsync(string filePath, string repoPath, ParserOptions? options);
    Task<List<string>> GetSupportedExtensionsAsync();
    Task<List<string>> GetInitFilesAsync(string repoPath);
    Task<List<string>> ResolveImportPathAsync(string importPath, string filePath, string repoPath);
}