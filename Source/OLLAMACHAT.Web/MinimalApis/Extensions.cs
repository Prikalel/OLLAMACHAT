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
        app.MapGet("/get_history", () => { return TypedResults.Ok(Array.Empty<ChatMessage>()); })
            .WithSummary("Get chat history")
            .WithTags("MinimalApi")
            .WithDescription("Returns list of previous chat messages")
            .Produces<List<ChatMessage>>(StatusCodes.Status200OK)
            .WithOpenApi();

        app.MapPost("/send_message", (SendMessageRequest request) => { return TypedResults.Ok(new { request_id = Guid.NewGuid().ToString() }); })
            .WithSummary("Submit new message")
            .WithTags("MinimalApi")
            .WithDescription("Accepts user message and returns processing request ID")
            .Produces<ResponseStatus>(StatusCodes.Status200OK)
            .WithOpenApi();

        app.MapGet("/check_response/{id}",
                (string id) =>
                {
                    return TypedResults.Ok(new ResponseStatus(
                        "ready",
                        new ResponseContent("message", "Mock response content")
                    ));
                })
            .WithSummary("Check message processing status")
            .WithTags("MinimalApi")
            .WithDescription("Returns current status of message processing request")
            .Produces<ResponseStatus>(StatusCodes.Status200OK)
            .WithOpenApi();

        app.MapGet("/check_image/{id}",
                (string id) =>
                {
                    return TypedResults.Ok(new ResponseStatus(
                        "ready",
                        new ResponseContent("image", "mock_image.webp")
                    ));
                })
            .WithSummary("Check image generation status")
            .WithTags("MinimalApi")
            .WithDescription("Returns current status of image generation request")
            .Produces<ResponseStatus>(StatusCodes.Status200OK)
            .WithOpenApi();

        app.MapPost("/change_model", (ChangeModelRequest request) => { return TypedResults.Ok(new { success = true }); })
            .WithSummary("Change active AI model")
            .WithTags("MinimalApi")
            .WithDescription("Updates the currently selected AI model")
            .Produces(StatusCodes.Status200OK)
            .WithOpenApi();
    }
}
