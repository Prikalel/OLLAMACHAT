namespace VelikiyPrikalel.OLLAMACHAT.Application.Services.Parser;

public interface IRoslynParsingService
{
    Task<ParseResult> ParseFileAsync(string filePath, ParserOptions? options); 
    Task<List<string>> ResolveImportPathAsync(string importPath, string filePath);
}