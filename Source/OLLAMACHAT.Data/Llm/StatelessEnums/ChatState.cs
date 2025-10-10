namespace VelikiyPrikalel.OLLAMACHAT.Data.Llm.StatelessEnums;

/// <summary>
/// Состояния чата.
/// </summary>
public enum ChatState
{
    /// <summary>
    /// Ожидает ввода пользователя.
    /// </summary>
    PendingInput = 0,

    /// <summary>
    /// Ожидает завершения генерации текстового ответа.
    /// </summary>
    WaitingMessageGeneration,

    /// <summary>
    /// Ожидает завершения генерации картинки.
    /// </summary>
    WaitingImageGeneration,
}
