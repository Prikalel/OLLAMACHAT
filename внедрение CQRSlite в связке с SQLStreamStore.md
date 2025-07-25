# Обзор изменений

Переход на Event Sourcing фундаментально меняет подход к хранению данных. Вместо того чтобы сохранять в базе данных только последнее состояние ваших сущностей (`User`, `UserChat`), вы будете хранить полную последовательность событий, которые привели к этому состоянию. Состояние объекта (агрегата) будет восстанавливаться "на лету" путем проигрывания всех его событий.

### Шаг 1: Установка необходимых пакетов

Для начала добавьте в ваш проект следующие NuGet-пакеты:

```xml
<PackageReference Include="CQRSlite" Version="0.25.0" />
<PackageReference Include="SqlStreamStore" Version="1.2.0" />
<PackageReference Include="SqlStreamStore.Sqlite" Version="1.2.0" />
<PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
```

Будьте умными и не добавляйте непаравильные лишние зависимости в Application-слой а меняйте зависимости только Infrastructure или Web-слоя.

### Шаг 2: Создание доменных событий

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
```


### Шаг 3: Преобразование сущностей в аггрегаты

Ваши доменные модели (`User`, `UserChat`) теперь станут агрегатами. Агрегат — это граница транзакционной согласованности. Они должны наследоваться от `CQRSlite.Domain.AggregateRoot`. Вместо прямого изменения полей, методы агрегата создают и применяют события с помощью `ApplyChange`.

```csharp
public class UserAggregate : AggregateRoot
{
    public string Name { get; private set; }

    // Конструктор без параметров необходим для CQRSlite
    private UserAggregate() { }

    public UserAggregate(Guid id, string name)
    {
        Id = id;
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
    public ChatState State { get; private set; }
    public string EnqueuedCompletionJobId { get; private set; }
    public List<ChatMessage> Messages { get; private set; } = new();

    private UserChatAggregate() { }

    public UserChatAggregate(Guid id, string userId, string name, string model)
    {
        Id = id;
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
    
    // Другие методы для изменения состояния...

    // Методы Apply для восстановления состояния
    private void Apply(ChatCreatedEvent e)
    {
        UserId = e.UserId;
        Name = e.Name;
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
}
```


### Шаг 4: Реализация хранилища событий с `SQLStreamStore`

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
        
        var expectedRevision = expectedVersion ?? aggregate.Version - events.Length;

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
            throw new Exception($"Aggregate '{aggregateId}' not found."); // Или верните null
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


### Шаг 5: Новая реализация репозитория

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


### Шаг 6: Настройка Dependency Injection

В файле `Program.cs` или `Startup.cs` настройте контейнер зависимостей.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // ... другие сервисы

    // Настройка SQLStreamStore для SQLite
    var settings = new SqliteStreamStoreSettings("Data Source=events.db");
    var streamStore = new SqliteStreamStore(settings);
    
    // Важно: Инициализация схемы БД
    streamStore.CreateSchemaIfNotExists().Wait(); 

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


### Шаг 7: Триггеры на события для дополнительной логики

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

### Шаг 8: Команды и их обработчики (Write Side)

Взаимодействие с системой теперь происходит через отправку команд.

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
```


### Использование

Теперь в вашем API контроллере или сервисе вы будете использовать MediatR для отправки команд и `IAggregateRepository` для получения данных.

```csharp
[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAggregateRepository<UserAggregate> _userRepository;

    public UsersController(IMediator mediator, IAggregateRepository<UserAggregate> userRepository)
    {
        _mediator = mediator;
        _userRepository = userRepository;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
    {
        var userId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetUser), new { id = userId }, null);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        try 
        {
            // Чтение данных путем восстановления агрегата из событий
            var user = await _userRepository.GetByIdAsync(id);
            return Ok(new { user.Id, user.Name, user.Version });
        }
        catch (Exception)
        {
            return NotFound();
        }
    }
}
```
