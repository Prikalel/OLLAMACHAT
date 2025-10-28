namespace TestLlm;

/// <summary>
/// Integration tests for SendMessage mediator command
/// </summary>
public class SendMessageIntegrationTests : IDisposable
{
    private readonly ServiceProvider serviceProvider;
    private readonly OllamaChatContext dbContext;
    private readonly Mock<ILlmBackgroundService> mockLlmBackgroundService = new();

    public SendMessageIntegrationTests()
    {
        ServiceCollection services = new();

        services.AddDbContext<OllamaChatContext>(options =>
            options.UseSqlite("Data Source=test.db"));

        Mock<ILlmService> mockLlmService1 = new();

        services.AddSingleton(mockLlmBackgroundService.Object);
        services.AddSingleton(mockLlmService1.Object);
        services.AddSingleton<ILogger<SendMessage.Handler>>(NullLogger<SendMessage.Handler>.Instance);
        services.AddSingleton<ILogger<GetUserChatHistory.Handler>>(NullLogger<GetUserChatHistory.Handler>.Instance);

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserChatRepository, UserChatRepository>();
        services.AddScoped<IRepository<ChatMessage>, Repository<ChatMessage>>();

        services.AddScoped<IRequestHandler<SendMessage.Command>, SendMessage.Handler>();
        services.AddScoped<IRequestHandler<GetUserChatHistory.Query, IEnumerable<ChatMessage>>, GetUserChatHistory.Handler>();

        serviceProvider = services.BuildServiceProvider();

        dbContext = serviceProvider.GetRequiredService<OllamaChatContext>();
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
        dbContext.RemoveRange(dbContext.Set<User>());
        dbContext.RemoveRange(dbContext.Set<UserChat>());
        dbContext.RemoveRange(dbContext.Set<ChatMessage>());
        dbContext.SaveChanges();

        mockLlmBackgroundService
            .Setup(x => x.GenerateTextResponse(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserChat>()))
            .ReturnsAsync("Test response from LLM");
        mockLlmService1
            .Setup(x => x.ListLocalModelsAsync())
            .ReturnsAsync(["glm-4.6"]);
    }

    [Fact]
    public async Task SendMessage_Command_ShouldCreateUserAndChat_AndSaveMessage()
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        IRequestHandler<SendMessage.Command> handler = scope.ServiceProvider.GetRequiredService<IRequestHandler<SendMessage.Command>>();
        IUserRepository userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        SendMessage.Command command = new("test-connection-id", "Hello, how are you?");
        await TriggerInitialUserGeneration(scope);

        await handler.Handle(command, CancellationToken.None);

        User user = await userRepository.GetOrCreateUser("alex");
        Assert.NotNull(user);
        Assert.Equal("alex", user.Name);
        Assert.Single(user.Chats);
        UserChat chat = user.Chats.First();
        Assert.True(chat.Active);
        Assert.Equal(ChatState.PendingInput, chat.State);
        Assert.Equal(2, chat.Messages.Count);
        ChatMessage? userMessage = chat.Messages.FirstOrDefault(m => m.Role == ChatMessageRole.User);
        ChatMessage? assistantMessage = chat.Messages.FirstOrDefault(m => m.Role == ChatMessageRole.Assistant);
        Assert.NotNull(userMessage);
        Assert.Equal("Hello, how are you?", userMessage.Content);
        Assert.NotNull(assistantMessage);
        Assert.Equal("Test response from LLM", assistantMessage.Content);
        mockLlmBackgroundService.Verify(
            x => x.GenerateTextResponse("test-connection-id", "Hello, how are you?", It.IsAny<UserChat>()),
            Times.Once);
    }

    [Fact]
    public async Task SendMessage_Command_Undo_ShouldDeleteLastMessage()
    {
        int initialMessageCount;
        IRequestHandler<SendMessage.Command> handler;
        User user;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            await TriggerInitialUserGeneration(scope);
            handler = scope.ServiceProvider.GetRequiredService<IRequestHandler<SendMessage.Command>>();
            IUserRepository userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            SendMessage.Command normalCommand = new("test-connection-id", "First message");
            await handler.Handle(normalCommand, CancellationToken.None);
            user = await userRepository.GetOrCreateUser("alex");
            initialMessageCount = user.Chats.First().Messages.Count;
            Assert.Equal(2, initialMessageCount);
        }

        using (IServiceScope scope2 = serviceProvider.CreateScope())
        {
            OllamaChatContext context = scope2.ServiceProvider.GetRequiredService<OllamaChatContext>();
            foreach (EntityEntry VARIABLE in context.ChangeTracker.Entries())
            {
                await VARIABLE.ReloadAsync();
            }
            await scope2.ServiceProvider.GetRequiredService<IRequestHandler<SendMessage.Command>>()
                .Handle(new SendMessage.Command("test-connection-id", "/undo"), CancellationToken.None);
        }

        user = await dbContext.Set<User>()
            .Include(u => u.Chats)
            .ThenInclude(userChat => userChat.Messages)
            .FirstAsync(x => x.Name == "alex");
        await dbContext.Entry(user).ReloadAsync();
        UserChat chat = user.Chats.First();
        Assert.Equal(initialMessageCount - 1, chat.Messages.Count);
        Assert.All(chat.Messages, m => Assert.Equal(ChatMessageRole.User, m.Role));
    }

    public void Dispose()
    {
        dbContext.Database.EnsureDeleted();
        dbContext.Database.CloseConnection();
        dbContext?.Dispose();
        serviceProvider?.Dispose();

        if (File.Exists("test.db"))
        {
            File.Delete("test.db");
        }
    }

    private static async Task TriggerInitialUserGeneration(IServiceScope scope)
    {
        await scope.ServiceProvider.GetRequiredService<IRequestHandler<GetUserChatHistory.Query, IEnumerable<ChatMessage>>>()
            .Handle(new GetUserChatHistory.Query(), CancellationToken.None);
    }
}
