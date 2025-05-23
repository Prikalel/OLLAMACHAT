namespace VelikiyPrikalel.OLLAMACHAT.Data;

/// <summary>
/// Чат пользователя.
/// </summary>
public class UserChat : IEntity
{
    private readonly StateMachine<ChatState, ChatAction> chatStateMachine;

    /// <summary>
    /// Чат пользователя.
    /// </summary>
    /// <param name="id">Идентификатор чата.</param>
    /// <param name="userId">Идентификатор пользователя, которому принадлежит чат.</param>
    /// <param name="name">Имя чата.</param>
    /// <param name="model">Модель чата.</param>
    /// <param name="active">Активный для пользователя.</param>
    /// <param name="state">Состояние.</param>
    public UserChat(string id, string userId, string name, string model, bool active, ChatState state)
    {
        Id = id;
        UserId = userId;
        Name = name;
        Model = model;
        Active = active;
        chatStateMachine = new(state);
        chatStateMachine
            .Configure(ChatState.PendingInput)
                .Permit(ChatAction.UserRequestedImageResponse, ChatState.WaitingImageGeneration)
                .Permit(ChatAction.UserRequestedTextResponse, ChatState.WaitingMessageGeneration);
        chatStateMachine
            .Configure(ChatState.WaitingImageGeneration)
                .Permit(ChatAction.GenerationComplete, ChatState.PendingInput);
        chatStateMachine
            .Configure(ChatState.WaitingMessageGeneration)
            .Permit(ChatAction.GenerationComplete, ChatState.PendingInput);
    }

    /// <summary>
    /// Id чата.
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }

    /// <summary>
    /// Id пользователя, которому принадлежит чат.
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Имя чата.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Модель, использующаяся для генерации ответов в чате.
    /// </summary>
    public string Model { get; set; }

    /// <summary>
    /// Флаг активности на случай если у пользователя несколько чатов.
    /// В один момент времени может быть только 1 открытый чат.
    /// </summary>
    public bool Active { get; set; }

    /// <summary>
    /// Состояние.
    /// </summary>
    public ChatState State { get; private set; }

    /// <summary>
    /// Сообщения этого чата.
    /// </summary>
    public ICollection<ChatMessage> Messages { get; set; } = [];

    /// <summary>
    /// Действие: пользователь ввёл промпт.
    /// </summary>
    /// <param name="prompt">Промпт пользователя.</param>
    /// <returns>Новое состояние.</returns>
    public ChatState UserEnteredPrompt(string prompt)
    {
        chatStateMachine.Fire(ChatAction.UserRequestedTextResponse);
        this.State = chatStateMachine.State;
        return chatStateMachine.State;
    }

    /// <summary>
    /// Выполнено.
    /// </summary>
    /// <returns>.</returns>
    public ChatState LlmReturnedResponse(string prompt, string response)
    {
        chatStateMachine.Fire(ChatAction.GenerationComplete);
        this.State = chatStateMachine.State;
        this.Messages.Add(new ChatMessage
        {
            Id = null,
            ChatId = this.Id,
            Role = ChatMessageRole.User,
            Content = prompt
        });
        this.Messages.Add(new ChatMessage
        {
            Id = null,
            ChatId = this.Id,
            Role = ChatMessageRole.Assistant,
            Content = response
        });
        return chatStateMachine.State;
    }
}
