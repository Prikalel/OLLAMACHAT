namespace VelikiyPrikalel.OLLAMACHAT.Web.MinimalApis;

/// <summary>
/// Роуты minimal api.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Добавить api.
    /// </summary>
    /// <param name="app">.</param>
    public static void AddMinimalApis(this IEndpointRouteBuilder app)
    {
        app.MapGet("/hi", (ILogger<Program> logger) =>
            {
                logger.LogInformation("HELLO WORLD FROM MINIMAL API CALLED !");
                return TypedResults.Ok("hello world");
            })
            .WithSummary("summary text summary text summary text")
            .WithTags("Api")
            .WithDescription("Some Method Summary Description")
            .WithOpenApi();
    }
}
