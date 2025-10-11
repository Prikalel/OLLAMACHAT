namespace VelikiyPrikalel.OLLAMACHAT.Application.Services.Parser.Factories;

/// <summary>
/// Создание <see cref="EventHandler"/>.
/// </summary>
public sealed class EventHandlerFactory(ILogger<EventHandlerFactory> logger)
{
    /// <summary>
    /// Обрабатывает создание EventHandler.
    /// </summary>
    /// <param name="entity">Сущность.</param>
    /// <param name="filePath">Путь до файла.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns><see cref="EventHandler"/>.</returns>
    public Task<EventHandler> Create(ParsedEntity entity, string filePath, CancellationToken cancellationToken)
    {
        logger.LogDebug("Обработка EventHandler: {EntityName} из файла: {FilePath}", entity.SimpleName, filePath);

        // Добавляем проверку перед сохранением обработчика
        if (entity.ReturnType != "void")
        {
            throw new ArgumentException(
                paramName: nameof(entity),
                message: $"Метод {entity.FullName} не является обработчиком UnityEvent, так как возвращает {entity.ReturnType}");
        }

        // Извлекаем параметры метода
        string[] parameterTypes = entity.Parameters?.Select(p => p.FullTypeName ?? string.Empty).ToArray() ?? [];
        string argumentsHash = ComputeArgumentsHash(parameterTypes);

        // Создаем новую запись
        logger.LogDebug("Создание нового EventHandler: {EntityName}", entity.SimpleName);

        EventHandler newHandler = new()
        {
            Id = Guid.NewGuid().ToString(),
            Name = entity.SimpleName,
            FullName = entity.FullName ?? string.Empty,
            ParameterTypes = parameterTypes,
            ArgumentsHash = argumentsHash,
            FilePath = filePath,
            IsInherited = false, // entity.Inheritance?.AllBaseClasses?.Any() == true,
            BaseClassFullName = null, // DetermineBaseClassFullName(entity),
            IsPublic = entity.Modifiers?.Contains("public") == true,
            IsDeleted = false
        };

        logger.LogDebug("EventHandler успешно создан: {EntityName}", entity.SimpleName);
        return Task.FromResult(newHandler);
    }

    /// <summary>
    /// Определяет базовый класс, в котором определено событие или метод.
    /// </summary>
    private static string? DetermineBaseClassFullName(ParsedEntity entity)
    {
        // Если у сущности есть наследование, ищем ближайший базовый класс
        if (entity.Inheritance?.AllBaseClasses?.Any() == true)
        {
            // Для обработчиков событий ищем базовый класс, который может содержать этот метод
            // Возвращаем первый базовый класс из цепочки наследования
            return entity.Inheritance.AllBaseClasses.FirstOrDefault();
        }

        return null;
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
