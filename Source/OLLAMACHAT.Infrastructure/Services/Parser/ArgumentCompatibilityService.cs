namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Services.Parser;

/// <summary>
/// Сервис для проверки совместимости аргументов между UnityEvent и EventHandler.
/// Реализует сложную логику совместимости согласно документу #29.
/// </summary>
public class ArgumentCompatibilityService(
    ILogger<ArgumentCompatibilityService> logger) : IArgumentCompatibilityService
{
    /// <inheritdoc />
    public bool AreSignaturesCompatible(UnityEvent unityEvent, EventHandler eventHandler)
    {
        if (unityEvent == null || eventHandler == null)
        {
            logger.LogWarning("AreSignaturesCompatible: unityEvent или eventHandler равен null");
            return false;
        }

        logger.LogDebug("Проверка совместимости сигнатур: UnityEvent={UnityEventName} (EventType={EventType}), EventHandler={EventHandlerName}",
            unityEvent.Name, unityEvent.EventType, eventHandler.Name);

        // Для UnityEvent (без generic) - обработчик без параметров
        if (IsNonGenericUnityEvent(unityEvent))
        {
            bool isCompatible = eventHandler.ParameterTypes == null || eventHandler.ParameterTypes.Length == 0;
            logger.LogDebug("UnityEvent без generic: совместимость={IsCompatible} (параметров обработчика: {ParamCount})",
                isCompatible, eventHandler.ParameterTypes?.Length ?? 0);
            return isCompatible;
        }

        // Для UnityEvent<T> и других generic вариантов - проверка типов параметров
        return AreArgumentTypesCompatible(unityEvent.GenericTypeArguments, eventHandler.ParameterTypes);
    }

    private bool AreArgumentTypesCompatible(string[]? unityEventArgumentTypes, string[] eventHandlerParameterTypes)
    {
        // Если у UnityEvent нет аргументов, то обработчик должен быть без параметров
        if (unityEventArgumentTypes == null || unityEventArgumentTypes.Length == 0)
        {
            bool isCompatible = eventHandlerParameterTypes == null || eventHandlerParameterTypes.Length == 0;
            logger.LogDebug("UnityEvent без аргументов: совместимость={IsCompatible}", isCompatible);
            return isCompatible;
        }

        // Если у обработчика нет параметров, а у UnityEvent есть - несовместимо
        if (eventHandlerParameterTypes == null || eventHandlerParameterTypes.Length == 0)
        {
            logger.LogDebug("UnityEvent имеет аргументы, а обработчик нет: несовместимо");
            return false;
        }

        // Количество параметров должно совпадать
        if (unityEventArgumentTypes.Length != eventHandlerParameterTypes.Length)
        {
            logger.LogDebug("Несовпадение количества параметров: UnityEvent={EventCount}, EventHandler={HandlerCount}",
                unityEventArgumentTypes.Length, eventHandlerParameterTypes.Length);
            return false;
        }

        // Проверяем совместимость каждого типа параметра
        for (int i = 0; i < unityEventArgumentTypes.Length; i++)
        {
            string eventType = unityEventArgumentTypes[i];
            string handlerType = eventHandlerParameterTypes[i];

            if (!IsTypeCompatible(eventType, handlerType))
            {
                logger.LogDebug("Несовместимость типов параметров [{Index}]: UnityEvent={EventType}, EventHandler={HandlerType}",
                    i, eventType, handlerType);
                return false;
            }
        }

        logger.LogDebug("Все типы параметров совместимы");
        return true;
    }

    private bool IsNonGenericUnityEvent(UnityEvent unityEvent)
    {
        if (unityEvent == null)
        {
            logger.LogWarning("IsNonGenericUnityEvent: unityEvent равен null");
            return false;
        }

        bool isNonGeneric = unityEvent.EventType == "UnityEvent" &&
                          (unityEvent.GenericTypeArguments == null || unityEvent.GenericTypeArguments.Length == 0);

        logger.LogDebug("IsNonGenericUnityEvent для {EventName}: {IsNonGeneric}",
            unityEvent.Name, isNonGeneric);
        return isNonGeneric;
    }

    private bool IsTypeCompatible(string expectedType, string actualType)
    {
        if (string.IsNullOrEmpty(expectedType) || string.IsNullOrEmpty(actualType))
        {
            logger.LogWarning("IsTypeCompatible: один из типов равен null или пуст");
            return false;
        }

        // Прямое совпадение типов
        if (expectedType == actualType)
        {
            logger.LogDebug("Прямое совпадение типов: {Type}", expectedType);
            return true;
        }

        // Убираем пробелы и проверяем снова
        string normalizedExpected = expectedType.Trim();
        string normalizedActual = actualType.Trim();

        if (normalizedExpected == normalizedActual)
        {
            logger.LogDebug("Совпадение типов после нормализации: {Type}", normalizedExpected);
            return true;
        }

        // Проверка на совместимость базовых типов Unity
        if (IsUnityTypeCompatible(normalizedExpected, normalizedActual))
        {
            logger.LogDebug("Совместимость Unity типов: {ExpectedType} <-> {ActualType}",
                normalizedExpected, normalizedActual);
            return true;
        }

        // Проверка на совместимость с учетом nullability
        if (IsNullableCompatible(normalizedExpected, normalizedActual))
        {
            logger.LogDebug("Совместимость с учетом nullability: {ExpectedType} <-> {ActualType}",
                normalizedExpected, normalizedActual);
            return true;
        }

        logger.LogDebug("Типы несовместимы: {ExpectedType} <-> {ActualType}",
            normalizedExpected, normalizedActual);
        return false;
    }

    /// <summary>
    /// Проверяет совместимость базовых типов Unity.
    /// </summary>
    private static bool IsUnityTypeCompatible(string expectedType, string actualType)
    {
        // Базовые типы Unity, которые могут быть взаимозаменяемы
        Dictionary<string, string[]> unityTypeMappings = new(StringComparer.OrdinalIgnoreCase)
        {
            { "UnityEngine.GameObject", ["UnityEngine.GameObject", "UnityEngine.Component"] },
            { "UnityEngine.Component", ["UnityEngine.Component", "UnityEngine.GameObject"] },
            { "UnityEngine.Transform", ["UnityEngine.Transform", "UnityEngine.Component"] },
            { "UnityEngine.MonoBehaviour", ["UnityEngine.MonoBehaviour", "UnityEngine.Component"] },
            { "UnityEngine.Object", ["UnityEngine.Object", "UnityEngine.GameObject", "UnityEngine.Component", "UnityEngine.MonoBehaviour"] },
            { "string", ["string", "System.String"] },
            { "int", ["int", "System.Int32"] },
            { "float", ["float", "System.Single"] },
            { "bool", ["bool", "System.Boolean"] },
            { "double", ["double", "System.Double"] },
            { "decimal", ["decimal", "System.Decimal"] }
        };

        if (unityTypeMappings.TryGetValue(expectedType, out string[]? compatibleTypes))
        {
            return compatibleTypes.Contains(actualType, StringComparer.OrdinalIgnoreCase);
        }

        return false;
    }

    /// <summary>
    /// Проверяет совместимость типов с учетом nullability.
    /// </summary>
    private static bool IsNullableCompatible(string expectedType, string actualType)
    {
        // Убираем nullable аннотации
        string expectedWithoutNullability = expectedType.Replace("?", "").Trim();
        string actualWithoutNullability = actualType.Replace("?", "").Trim();

        return expectedWithoutNullability.Equals(actualWithoutNullability, StringComparison.OrdinalIgnoreCase);
    }
}
