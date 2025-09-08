namespace VelikiyPrikalel.OLLAMACHAT.Web.MinimalApis;

/// <summary>
/// Роуты minimal api.
/// </summary>
public static class Extensions // TODO: имплементировать endpoint-ы
{
    // TODO: static files.
    // The Flask app references images at /static/images/, but in ASP.NET Core, static files are typically in wwwroot.
    // We need to ensure that the static files are properly set up

    /// <summary>
    /// Добавить api.
    /// </summary>
    /// <param name="app">.</param>
    public static void AddMinimalApis(this IEndpointRouteBuilder app)
    {
        app.MapGet("/get_history", async (IMediator mediator) => TypedResults.Ok(
                (await mediator.Send(new GetUserChatHistory.Query()))
                .Select(x => new ChatMessageDto(
                    x.Role.ToString().ToLower(),
                    x.Content))
            ))
            .WithSummary("Get chat history")
            .WithTags("MinimalApi")
            .WithDescription("Returns list of previous chat messages")
            .Produces<List<ChatMessageDto>>(StatusCodes.Status200OK)
            .WithOpenApi();
    }
}
