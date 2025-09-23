namespace VelikiyPrikalel.OLLAMACHAT.Application.Mediator.CsharpParser;

/// <summary>
/// Получить доступные модели.
/// </summary>
public sealed class GetInitFiles
{
    /// <summary>
    /// Запрос.
    /// </summary>
    public sealed record Query() : IRequest<List<string>>;

    /// <inheritdoc />
    public sealed class Handler() : IRequestHandler<Query, List<string>>
    {
        /// <inheritdoc />
        public async ValueTask<List<string>> Handle(Query request, CancellationToken cancellationToken)
        {
            string exampleJson = null;
            exampleJson = "[ \"AssemblyInfo.cs\", \"GlobalUsings.cs\" ]";

            var example = exampleJson != null
                ? JsonConvert.DeserializeObject<List<string>>(exampleJson)
                : default(List<string>);            //TODO: Change the data returned
            return example;
        }
    }
}
