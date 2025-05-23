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

        app.MapPost("/send_message", async ([FromServices] IMediator mediator, SendMessageRequestDto request) =>
            TypedResults.Ok(new
            {
                request_id = await mediator.Send(new SendMessage.Command(request.Message))
            }))
            .WithSummary("Submit new message")
            .WithTags("MinimalApi")
            .WithDescription("Accepts user message and returns processing request ID")
            .Produces<ResponseStatusDto>(StatusCodes.Status200OK)
            .WithOpenApi();

        app.MapGet("/check_response/{id}",
                (
                    [FromServices] IBackgroundJobClientV2 backgroundJobClient,
                    [FromServices] IRepository<UserChat> chatRepository,
                    string id
                ) =>
                {
                    try
                    {
                        IMonitoringApi monitoringApi = JobStorage.Current.GetMonitoringApi();
                        JobDetailsDto jobDetails = monitoringApi.JobDetails(id);
                        string currentState = jobDetails.History[0].StateName;
                        if (currentState == EnqueuedState.StateName || currentState == ProcessingState.StateName || currentState == AwaitingState.StateName)
                        {
                            return TypedResults.Ok(new ResponseStatusDto(
                                "processing",
                                null));
                        }
                        else if (currentState == FailedState.StateName)
                        {
                            var chatId = "123";
                            var chat = chatRepository.GetChatByIdAsync(chatId).GetAwaiter().GetResult();
                            chat.GenerationFailed();
                            chatRepository.UpdateAsync(chat);
                        }

                        string jobValue = jobDetails.History
                            .First(x => x.StateName == SucceededState.StateName)
                            .Data["Result"];
                        string deserializeString = JsonConvert.DeserializeObject<string>(jobValue)!;
                        string responseHtmlContent = Markdig.Markdown.ToHtml(deserializeString);
                        return TypedResults.Ok(new ResponseStatusDto(
                            "ready",
                            new ResponseContentDto("message", responseHtmlContent)
                        ));
                    }
                    catch (Exception)
                    {
                        return TypedResults.Ok(new ResponseStatusDto(
                            "processing",
                            null));
                    }
                })
            .WithSummary("Check message processing status")
            .WithTags("MinimalApi")
            .WithDescription("Returns current status of message processing request")
            .Produces<ResponseStatusDto>(StatusCodes.Status200OK)
            .WithOpenApi();

        app.MapGet("/check_image/{id}",
                (string id) =>
                {
                    return TypedResults.Ok(new ResponseStatusDto(
                        "ready",
                        new ResponseContentDto("image", "mock_image.webp")
                    ));
                })
            .WithSummary("Check image generation status")
            .WithTags("MinimalApi")
            .WithDescription("Returns current status of image generation request")
            .Produces<ResponseStatusDto>(StatusCodes.Status200OK)
            .WithOpenApi();

        app.MapPost("/change_model",
                (ChangeModelRequestDto requestDto) =>
                {
                    throw new NotImplementedException();
                    return TypedResults.Ok(new { success = true });
                })
            .WithSummary("Change active AI model")
            .WithTags("MinimalApi")
            .WithDescription("Updates the currently selected AI model")
            .Produces(StatusCodes.Status200OK)
            .WithOpenApi();
    }
}
