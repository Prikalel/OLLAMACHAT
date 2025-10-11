namespace VelikiyPrikalel.OLLAMACHAT.Application.Mediator.CsharpParser;

using ParseCsharpFileResponse = (ParseResult result, ErrorResponse? error);

/// <summary>
/// Команда парсинга файла.
/// </summary>
public sealed class ParseCsharpFile
{
    /// <summary>
    /// Запрос.
    /// </summary>
    /// <param name="Request">Запрос.</param>
    public sealed record Query(ParserRequest Request) : IRequest<ParseCsharpFileResponse>;

    /// <inheritdoc />
    public sealed class Handler(IRoslynParsingService roslynParsingService) : IRequestHandler<Query, ParseCsharpFileResponse>
    {
        /// <inheritdoc />
        public async ValueTask<ParseCsharpFileResponse> Handle(Query request, CancellationToken cancellationToken)
        {
            try
            {
                ParseResult result = await roslynParsingService.ParseFileAsync(
                    request.Request.FilePath,
                    request.Request.Options);

                return (result, null);
            }
            catch (Exception ex)
            {
                ErrorResponse error = new(ex.GetHashCode().ToString(), $"Failed to parse C# file: {ex.Message}", ex.ToString());

                return (new(
                    FilePath: request.Request.FilePath,
                    Language: ParseResultLanguage.Csharp,
                    Entities: new(),
                    Relationships: new(),
                    ContentHash: "",
                    ParseTimeMs: 0,
                    Errors: new()
                    {
                        new(
                            Message: ex.Message,
                            Severity: ParseErrorSeverity.Error,
                            Location: null
                        )
                    }
                ), error);
            }
        }
    }
}
