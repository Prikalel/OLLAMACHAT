# Обзор изменений

Переход на Event Sourcing фундаментально меняет подход к хранению данных. Вместо того чтобы сохранять в базе данных только последнее состояние ваших сущностей (`User`, `UserChat`), вы будете хранить полную последовательность событий, которые привели к этому состоянию. Состояние объекта (агрегата) будет восстанавливаться "на лету" путем проигрывания всех его событий.

В текущей реализации используется SQLite с Mediator и простым интерфейсом репозитория для CRUD операций. Приложение использует Minimal API вместо традиционных контроллеров. Также стоит отметить, что `ChatMessage` может быть превращён не в агрегат, а в ValueObject (в терминах C# - record).

## Шаг 1: Установка необходимых пакетов

Для начала добавьте в ваш проект следующие NuGet-пакеты:

```xml
<PackageReference Include="CQRSlite" Version="0.25.0" />
<PackageReference Include="SqlStreamStore" Version="1.2.0" />
<PackageReference Include="SqlStreamStore.Sqlite" Version="1.2.0" />
<PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
```

Будьте умными и не добавляйте неправильные лишние зависимости в Application-слой, а меняйте зависимости только Infrastructure или Web-слоя.

## Шаг 2: Создание доменных событий

События — это неизменяемые факты о том, что произошло в системе. Создайте классы для каждого значимого изменения в ваших агрегатах. Все события должны реализовывать интерфейс `CQRSlite.Events.IEvent`.

```csharp
// Для единообразия, базовый интерфейс для событий
public interface IMyEvent : IEvent 
{
    Guid Id { get; set; }
    DateTimeOffset TimeStamp { get; set; }
}

// --- События для User ---
public class UserCreatedEvent : IMyEvent
{
    public Guid Id { get; set; }
    public int Version { get; set; }
    public DateTimeOffset TimeStamp { get; set; }
    
    public string UserId { get; set; }
    public string Name { get; set; }
}

public class UserNameChangedEvent : IMyEvent
{
    public Guid Id { get; set; }
    public int Version { get; set; }
    public DateTimeOffset TimeStamp { get; set; }
    
    public string UserId { get; set; }
    public string NewName { get; set; }
}

// --- События для UserChat ---
public class ChatCreatedEvent : IMyEvent
{
    public Guid Id { get; set; }
    public int Version { get; set; }
    public DateTimeOffset TimeStamp { get; set; }
    
    public string ChatId { get; set; }
    public string UserId { get; set; }
    public string Name { get; set; }
    public string Model { get; set; }
}

public class MessageAddedEvent : IMyEvent
{
    public Guid Id { get; set; }
    public int Version { get; set; }
    public DateTimeOffset TimeStamp { get; set; }
    
    public string ChatId { get; set; }
    public string MessageId { get; set; }
    public ChatMessageRole Role { get; set; }
    public string Content { get; set; }
    public DateTimeOffset Time { get; set; }
}

public class ChatStateChangedEvent : IMyEvent
{
    public Guid Id { get; set; }
    public int Version { get; set; }
    public DateTimeOffset TimeStamp { get; set; }
    
    public string ChatId { get; set; }
    public ChatState NewState { get; set; }
    public string JobId { get; set; }
}

public class ChatModelChangedEvent : IMyEvent
{
    public Guid Id { get; set; }
    public int Version { get; set; }
    public DateTimeOffset TimeStamp { get; set; }
    
    public string ChatId { get; set; }
    public string NewModel { get; set; }
}
```

## Шаг 3: Преобразование сущностей в агрегаты

Ваши доменные модели (`User`, `UserChat`) теперь станут агрегатами. Агрегат — это граница транзакционной согласованности. Они должны наследоваться от `CQRSlite.Domain.AggregateRoot`. Вместо прямого изменения полей, методы агрегата создают и применяют события с помощью `ApplyChange`.

`ChatMessage` может быть преобразован в ValueObject (record), так как он представляет собой неизменяемую структуру данных без собственного поведения.

```csharp
public class UserAggregate : AggregateRoot
{
    public string Name { get; private set; }

    // Конструктор без параметров необходим для CQRSlite
    private UserAggregate() { }

    public UserAggregate(Guid id, string name)
    {
        ApplyChange(new UserCreatedEvent
        {
            Id = Guid.NewGuid(),
            UserId = id.ToString(),
            Name = name,
            TimeStamp = DateTimeOffset.UtcNow
        });
    }

    public void ChangeName(string newName)
    {
        if (Name != newName)
        {
            ApplyChange(new UserNameChangedEvent
            {
                Id = Guid.NewGuid(),
                UserId = this.Id.ToString(),
                NewName = newName,
                TimeStamp = DateTimeOffset.UtcNow
            });
        }
    }

    // Методы Apply восстанавливают состояние из событий
    private void Apply(UserCreatedEvent e)
    {
        // Id уже устанавливается в CQRSlite из потока событий
        Name = e.Name;
    }

    private void Apply(UserNameChangedEvent e)
    {
        Name = e.NewName;
    }
}

public class UserChatAggregate : AggregateRoot
{
    public string UserId { get; private set; }
    public string Name { get; private set; }
    public string Model { get; private set; }
    public ChatState State { get; private set; }
    public string EnqueuedCompletionJobId { get; private set; }
    public List<ChatMessage> Messages { get; private set; } = new();

    private UserChatAggregate() { }

    public UserChatAggregate(Guid id, string userId, string name, string model)
    {
        ApplyChange(new ChatCreatedEvent
        {
            Id = Guid.NewGuid(),
            ChatId = id.ToString(),
            UserId = userId,
            Name = name,
            Model = model,
            TimeStamp = DateTimeOffset.UtcNow
        });
    }

    public void AddMessage(string messageId, ChatMessageRole role, string content)
    {
        ApplyChange(new MessageAddedEvent
        {
            Id = Guid.NewGuid(),
            ChatId = this.Id.ToString(),
            MessageId = messageId,
            Role = role,
            Content = content,
            Time = DateTimeOffset.UtcNow,
            TimeStamp = DateTimeOffset.UtcNow
        });
    }
    
    public void ChangeModel(string newModel)
    {
        ApplyChange(new ChatModelChangedEvent
        {
            Id = Guid.NewGuid(),
            ChatId = this.Id.ToString(),
            NewModel = newModel,
            TimeStamp = DateTimeOffset.UtcNow
        });
    }
    
    public void UpdateState(ChatState newState, string jobId)
    {
        ApplyChange(new ChatStateChangedEvent
        {
            Id = Guid.NewGuid(),
            ChatId = this.Id.ToString(),
            NewState = newState,
            JobId = jobId,
            TimeStamp = DateTimeOffset.UtcNow
        });
    }

    // Методы Apply для восстановления состояния
    private void Apply(ChatCreatedEvent e)
    {
        UserId = e.UserId;
        Name = e.Name;
        Model = e.Model;
        State = ChatState.PendingInput; // Начальное состояние
    }

    private void Apply(MessageAddedEvent e)
    {
        Messages.Add(new ChatMessage
        {
            Id = e.MessageId,
            ChatId = e.ChatId,
            Role = e.Role,
            Content = e.Content,
            Time = e.Time
        });
    }
    
    private void Apply(ChatStateChangedEvent e)
    {
        State = e.NewState;
        EnqueuedCompletionJobId = e.JobId;
    }
    
    private void Apply(ChatModelChangedEvent e)
    {
        Model = e.NewModel;
    }
}
```

## Шаг 4: Реализация хранилища событий с `SQLStreamStore`

Создадим реализацию `IEventStore`, которая будет работать с `SQLStreamStore`.

```csharp
using CQRSlite.Domain;
using CQRSlite.Events;
using Newtonsoft.Json;
using SqlStreamStore;
using SqlStreamStore.Streams;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SqlStreamStoreEventStore : IEventStore
{
    private readonly IStreamStore _streamStore;
    private readonly IEventPublisher _publisher;

    public SqlStreamStoreEventStore(IStreamStore streamStore, IEventPublisher publisher)
    {
        _streamStore = streamStore;
        _publisher = publisher;
    }

    public async Task Save<T>(T aggregate, int? expectedVersion = null) where T : AggregateRoot
    {
        var streamId = $"{aggregate.GetType().Name}-{aggregate.Id}";
        var events = aggregate.GetUncommittedChanges().ToArray();

        if (!events.Any()) return;

        var newStreamMessages = events.Select(e => new NewStreamMessage(
            Guid.NewGuid(),
            e.GetType().AssemblyQualifiedName, // Используем полное имя типа для десериализации
            JsonConvert.SerializeObject(e)
        )).ToArray();
        
        var expectedRevision = expectedVersion ?? aggregate.Version;

        await _streamStore.AppendToStream(streamId, expectedRevision, newStreamMessages);

        foreach (var @event in events)
        {
            await _publisher.Publish(@event);
        }

        aggregate.MarkChangesAsCommitted();
    }

    public async Task<T> Get<T>(Guid aggregateId, CancellationToken cancellationToken = default) where T : AggregateRoot, new()
    {
        var streamId = $"{typeof(T).Name}-{aggregateId}";
        var aggregate = new T();

        var page = await _streamStore.ReadStreamForwards(streamId, StreamVersion.Start, int.MaxValue, cancellationToken);

        if (page.Status == PageReadStatus.StreamNotFound)
        {
            throw new AggregateNotFoundException($"Aggregate '{aggregateId}' not found.");
        }
        
        var history = new List<IEvent>();
        foreach(var message in page.Messages)
        {
            var eventType = Type.GetType(message.Type);
            var jsonData = await message.GetJsonData(cancellationToken);
            var @event = JsonConvert.DeserializeObject(jsonData, eventType) as IEvent;
            history.Add(@event);
        }

        aggregate.LoadFromHistory(history);
        return aggregate;
    }
}
```

## Шаг 5: Новая реализация репозитория

Старый репозиторий, работавший с EF Core, заменяется новым, который оперирует агрегатами и хранилищем событий.

```csharp
public interface IAggregateRepository<T> where T : AggregateRoot, new()
{
    Task<T> GetByIdAsync(Guid id);
    Task SaveAsync(T aggregate, int? expectedVersion = null);
}

public class AggregateRepository<T> : IAggregateRepository<T> where T : AggregateRoot, new()
{
    private readonly IEventStore _eventStore;

    public AggregateRepository(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task<T> GetByIdAsync(Guid id)
    {
        return await _eventStore.Get<T>(id);
    }

    public async Task SaveAsync(T aggregate, int? expectedVersion = null)
    {
        await _eventStore.Save(aggregate, expectedVersion);
    }
}
```

## Шаг 6: Настройка Dependency Injection

В файле `Startup.cs` настройте контейнер зависимостей.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // ... другие сервисы

    // Настройка SQLStreamStore для SQLite
    var settings = new SqliteStreamStoreSettings("Data Source=events.db");
    var streamStore = new SqliteStreamStore(settings);
    
    // Важно: Инициализация схемы БД
    streamStore.CreateSchemaIfNotExists().GetAwaiter().GetResult(); 

    services.AddSingleton<IStreamStore>(streamStore);
    
    // Регистрация компонентов CQRSlite и нашего репозитория
    services.AddScoped<IEventStore, SqlStreamStoreEventStore>();
    services.AddScoped(typeof(IAggregateRepository<>), typeof(AggregateRepository<>));

    // Настройка MediatR для публикации и обработки событий
    services.AddScoped<IEventPublisher, MediatREventPublisher>();
    
    // ...
}

// Простая реализация IEventPublisher через MediatR
public class MediatREventPublisher : IEventPublisher
{
    private readonly IMediator _mediator;
    public MediatREventPublisher(IMediator mediator) => _mediator = mediator;
    public Task Publish<T>(T @event, CancellationToken cancellationToken = default) where T : IEvent
    {
        return _mediator.Publish(@event, cancellationToken);
    }
}
```

## Шаг 7: Триггеры на события для дополнительной логики

Это ответ на ваш вопрос о том, как реагировать на события. Вы создаете обработчики, которые подписываются на конкретные типы событий. MediatR отлично справляется с этой задачей.

```csharp
public class UserEventLoggingHandler : 
    INotificationHandler<UserCreatedEvent>,
    INotificationHandler<UserNameChangedEvent>
{
    private readonly ILogger<UserEventLoggingHandler> _logger;

    public UserEventLoggingHandler(ILogger<UserEventLoggingHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Пользователь создан. ID: {UserId}, Имя: {Name}", 
            notification.UserId, 
            notification.Name);
        return Task.CompletedTask;
    }

    public Task Handle(UserNameChangedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Имя пользователя изменено. ID: {UserId}, Новое имя: {NewName}", 
            notification.UserId, 
            notification.NewName);
        return Task.CompletedTask;
    }
}
```

MediatR автоматически обнаружит этот класс и будет вызывать его методы `Handle` каждый раз, когда `IEventPublisher` публикует `UserCreatedEvent` или `UserNameChangedEvent`.

## Команды и их обработчики (Write Side)

Взаимодействие с системой теперь происходит через отправку команд. Вместо традиционных контроллеров приложение использует Minimal API.

```csharp
// Команда на создание пользователя
public class CreateUserCommand : IRequest<Guid>
{
    public string Name { get; set; }
}

// Обработчик команды
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IAggregateRepository<UserAggregate> _repository;

    public CreateUserCommandHandler(IAggregateRepository<UserAggregate> repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var userId = Guid.NewGuid();
        var user = new UserAggregate(userId, request.Name);
        
        await _repository.SaveAsync(user);
        
        return userId;
    }
}

// Команда на отправку сообщения
public class SendMessageCommand : IRequest<string>
{
    public string Message { get; set; }
}

// Обработчик команды отправки сообщения
public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, string>
{
    private readonly IAggregateRepository<UserAggregate> _userRepository;
    private readonly IBackgroundJobClientV2 backgroundJobClient;
    private readonly ILogger<SendMessageCommandHandler> logger;

    public SendMessageCommandHandler(
        IAggregateRepository<UserAggregate> userRepository,
        IBackgroundJobClientV2 backgroundJobClient,
        ILogger<SendMessageCommandHandler> logger)
    {
        _userRepository = userRepository;
        this.backgroundJobClient = backgroundJobClient;
        this.logger = logger;
    }

    public async Task<string> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        string? jobId = null;
        try
        {
            // Получаем пользователя (в реальной реализации нужно определить, как получать текущего пользователя)
            var user = await _userRepository.GetByIdAsync(Guid.Parse("user-id")); // Замените на реальную логику получения пользователя
            var activeChat = user.GetOrCreateActiveChat(null, out bool _);

            logger.LogInformation("Will generate llm response from model {Model}", activeChat.Model);
            // создали таску на генерацию ответа. запуск через сколько угодно главное дать нам время обновить состояние чата перед постановкой в очередь.
            jobId = backgroundJobClient.Create<ILlmBackgroundService>(llmService => llmService.GenerateTextResponse(
                    request.Message,
                    activeChat.Model,
                    activeChat.Id,
                    activeChat.Messages),
                new ScheduledState(TimeSpan.FromHours(3)));

            activeChat.UpdateState(ChatState.WaitingMessageGeneration, jobId);
            await _userRepository.SaveAsync(user); // сохранили состояние чата с идентификатором задачи

            // поставили задачу в очередь
            backgroundJobClient.ChangeState(jobId, new EnqueuedState(), ScheduledState.StateName);

            logger.LogInformation("Returning job id {Id}, chat {Id} current state {State}",
                jobId,
                activeChat.Id,
                activeChat.State);
        }
        catch (Exception ex)
        {
            // Если что-то пошло не так - удалили задачу на генерацию ответа
            if (jobId != null)
            {
                backgroundJobClient.ChangeState(jobId, new DeletedState());
            }

            throw;
        }

        return jobId!;
    }
}
```

## Использование

Теперь в вашем Minimal API вы будете использовать MediatR для отправки команд и `IAggregateRepository` для получения данных.

```csharp
public static class Extensions
{
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

        // ... другие endpoints
    }
}
```

## Особенности внедрения Event Sourcing

Особенность внедрения Event Sourcing в нашем случае заключается в том, что мы не будем использовать readonly-проекции, а для операций чтения будем строить агрегаты из событий. Это упрощает архитектуру, но может повлиять на производительность при большом количестве событий.

## Слои, в которых происходят изменения

1. **Application Layer**: 
   - Замена команд и обработчиков для работы с агрегатами вместо сущностей EF
   - Обновление интерфейсов репозиториев

2. **Data Layer**:
   - Создание доменных событий
   - Преобразование сущностей в агрегаты
   - Создание ValueObjects (ChatMessage как record)

3. **Infrastructure Layer**:
   - Реализация хранилища событий с использованием SQLStreamStore
   - Новая реализация репозиториев для работы с агрегатами
   - Настройка Dependency Injection для новых компонентов

4. **Web Layer**:
   - Адаптация Minimal API для работы с новыми командами и обработчиками
   - Обновление конфигурации Startup.cs для регистрации новых сервисов
