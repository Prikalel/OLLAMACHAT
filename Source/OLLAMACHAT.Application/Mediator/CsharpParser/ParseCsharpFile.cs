namespace VelikiyPrikalel.OLLAMACHAT.Application.Mediator.CsharpParser;

using ParseCsharpFileResponse = (ParseResult result, ErrorResponse? error);

public sealed class ParseCsharpFile
{
    /// <summary>
    /// Запрос.
    /// </summary>
    public sealed record Query(ParserRequest request) : IRequest<ParseCsharpFileResponse>;

    /// <inheritdoc />
    public sealed class Handler(IRoslynParsingService roslynParsingService) : IRequestHandler<Query, ParseCsharpFileResponse>
    {
        /// <inheritdoc />
        public async ValueTask<ParseCsharpFileResponse> Handle(Query request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await roslynParsingService.ParseFileAsync(
                    request.request.FilePath,
                    request.request.RepoPath,
                    request.request.Options);

                return (result, null);
            }
            catch (Exception ex)
            {
                var error = new ErrorResponse(ex.GetHashCode().ToString(), $"Failed to parse C# file: {ex.Message}", ex.ToString());

                return (new ParseResult(
                    FilePath: request.request.FilePath,
                    Language: ParseResultLanguage.Csharp,
                    Entities: new List<ParsedEntity>(),
                    Relationships: new List<Relationship>(),
                    ContentHash: "",
                    ParseTimeMs: 0,
                    Errors: new List<ParseError>
                    {
                        new ParseError(
                            Message: ex.Message,
                            Severity: ParseErrorSeverity.Error,
                            Location: null
                        )
                    },
                    Metadata: null
                ), error);
            }
        }
    }
}
