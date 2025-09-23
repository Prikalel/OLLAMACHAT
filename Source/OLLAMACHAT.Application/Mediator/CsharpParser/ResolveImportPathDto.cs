namespace VelikiyPrikalel.OLLAMACHAT.Application.Mediator.CsharpParser;

using ResolveImportResponse = (List<string> result, ErrorResponse? error);

public sealed class ResolveImportPathDto
{
    /// <summary>
    /// Запрос.
    /// </summary>
    public sealed record Query(ResolveImportRequest request) : IRequest<ResolveImportResponse>;

    /// <inheritdoc />
    public sealed class Handler() : IRequestHandler<Query, ResolveImportResponse>
    {
        /// <inheritdoc />
        public async ValueTask<ResolveImportResponse> Handle(Query request, CancellationToken cancellationToken)
        {
            string exampleJson = null;
            exampleJson = "[ \"src/System/Collections/Generic/List.cs\" ]";

            var example = exampleJson != null
                ? JsonConvert.DeserializeObject<List<string>>(exampleJson)
                : default(List<string>);            //TODO: Change the data returned
            return (example!, null);
        }
    }
}
