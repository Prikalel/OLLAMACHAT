namespace VelikiyPrikalel.OLLAMACHAT.Application.Services.Parser.Factories;

/// <summary>
/// Фабрика <see cref="UnityEvent"/>.
/// </summary>
public sealed class UnityEventFactory(ILogger<UnityEventFactory> logger)
{
    /// <summary>
    /// Формирует <see cref="UnityEvent"/>.
    /// </summary>
    /// <param name="entity">Запрос команды.</param>
    /// <param name="filePath">Путь до файла, где определена сущность.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns><see cref="UnityEvent"/>.</returns>
    public Task<UnityEvent> Create(ParsedEntity entity, string filePath, CancellationToken cancellationToken)
    {
        logger.LogDebug("Обработка UnityEvent: {EntityName} из файла: {FilePath}", entity.SimpleName, filePath);

        // Извлекаем аргументы из типа события
        string[] genericArguments = ExtractGenericArguments(entity);
        string argumentsHash = ComputeArgumentsHash(genericArguments);

        // Создаем новую запись
        logger.LogDebug("Создание нового UnityEvent: {EntityName}", entity.SimpleName);

        UnityEvent newEvent = new()
        {
            Id = Guid.NewGuid().ToString(),
            Name = entity.SimpleName,
            FullName = entity.FullName ?? string.Empty,
            EventType = "UnityEvent",
            GenericTypeArguments = genericArguments,
            ArgumentsHash = argumentsHash,
            FilePath = filePath,
            IsInherited = false, // entity.Inheritance?.AllBaseClasses?.Any() == true,
            BaseClassFullName = null, // DetermineBaseClassFullName(entity),
            IsDeleted = false
        };

        logger.LogDebug("UnityEvent успешно создан: {EntityName}", entity.SimpleName);
        return Task.FromResult(newEvent);
    }

    /// <summary>
    /// Определяет базовый класс, в котором определено событие или метод.
    /// </summary>
    private static string? DetermineBaseClassFullName(ParsedEntity entity)
    {
        // Если у сущности есть наследование, ищем ближайший базовый класс
        if (entity.Inheritance?.AllBaseClasses?.Any() == true)
        {
            // Для UnityEvent ищем базовый класс, который может содержать это событие
            // Возвращаем первый базовый класс из цепочки наследования
            return entity.Inheritance.AllBaseClasses.FirstOrDefault();
        }

        return null;
    }

    /// <summary>
    /// Извлекает универсальные аргументы из типа UnityEvent.
    /// </summary>
    private static string[] ExtractGenericArguments(ParsedEntity entity)
    {
        // Извлекаем аргументы из имени типа или атрибутов
        // Это упрощенная реализация, в реальном сценарии может потребоваться более сложный анализ
        List<string> result = new();

        if (entity.Parameters?.Any() == true)
        {
            result.AddRange(entity.Parameters.Select(p => p.FullTypeName ?? string.Empty));
        }

        return result.ToArray();
    }

    /// <summary>
    /// Вычисляет хэш аргументов для быстрого сравнения.
    /// </summary>
    private static string ComputeArgumentsHash(string[] arguments)
    {
        if (arguments == null || arguments.Length == 0)
        {
            return "empty";
        }

        string concatenated = string.Join(",", arguments.OrderBy(x => x));
        using SHA256 sha256 = SHA256.Create();
        byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(concatenated));
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}
