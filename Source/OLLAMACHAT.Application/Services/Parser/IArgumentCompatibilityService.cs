namespace VelikiyPrikalel.OLLAMACHAT.Application.Services.Parser;

/// <summary>
/// Сервис для проверки совместимости аргументов между UnityEvent и EventHandler.
/// </summary>
public interface IArgumentCompatibilityService
{
    /// <summary>
    /// Проверяет совместимость сигнатур между UnityEvent и EventHandler.
    /// </summary>
    /// <param name="unityEvent">UnityEvent для проверки.</param>
    /// <param name="eventHandler">EventHandler для проверки.</param>
    /// <returns>True, если сигнатуры совместимы, иначе false.</returns>
    bool AreSignaturesCompatible(UnityEvent unityEvent, EventHandler eventHandler);
}
