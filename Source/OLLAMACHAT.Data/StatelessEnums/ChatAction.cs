namespace VelikiyPrikalel.OLLAMACHAT.Data.StatelessEnums;

/// <summary>
/// Операции для описания переходов между состояниями чата.
/// </summary>
public enum ChatAction
{
    /// <summary>
    /// Юзер ввёл промпт, требуется текстовый ответ.
    /// </summary>
    UserRequestedTextResponse = 0,

    /// <summary>
    /// Юзер ввёл промпт, требуется ответ картинкой.
    /// </summary>
    UserRequestedImageResponse
}
