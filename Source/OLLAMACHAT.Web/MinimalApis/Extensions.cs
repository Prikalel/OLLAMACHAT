namespace VelikiyPrikalel.OLLAMACHAT.Web.MinimalApis;

/// <summary>
/// Роуты minimal api.
/// </summary>
public static class Extensions // TODO: переместить в выход Home контроллера, а этот файл удалить
{
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
                    Markdig.Markdown.ToHtml(x.Content)))
            ))
            .WithSummary("Get chat history")
            .WithTags("MinimalApi")
            .WithDescription("Returns list of previous chat messages")
            .Produces<List<ChatMessageDto>>(StatusCodes.Status200OK)
            .WithOpenApi();
    }
}
