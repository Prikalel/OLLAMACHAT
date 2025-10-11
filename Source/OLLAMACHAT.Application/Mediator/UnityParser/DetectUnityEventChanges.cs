namespace VelikiyPrikalel.OLLAMACHAT.Application.Mediator.UnityParser;

/// <summary>
/// Команда для сравнения изменений в UnityEvent между существующими и новыми данными.
/// </summary>
public sealed class DetectUnityEventChanges
{
    /// <summary>
    /// Запрос команды для сравнения UnityEvent.
    /// </summary>
    /// <param name="ExistingEvents">Существующие UnityEvent из базы данных.</param>
    /// <param name="NewEvents">Новые UnityEvent из анализа кода.</param>
    public sealed record Command(
        IReadOnlyList<UnityEvent> ExistingEvents,
        IReadOnlyList<UnityEvent> NewEvents) : IRequest<Response>;

    /// <summary>
    /// Ответ команды с результатами сравнения.
    /// </summary>
    public sealed record Response(ChangeDetectionResult<UnityEvent> Result);

    /// <summary>
    /// Обработчик команды для сравнения UnityEvent.
    /// </summary>
    public sealed class Handler(
        ILogger<Handler> logger) : IRequestHandler<Command, Response>
    {
        /// <summary>
        /// Обрабатывает команду сравнения UnityEvent.
        /// </summary>
        /// <param name="request">Запрос команды.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Результат сравнения.</returns>
        public async ValueTask<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            DateTime startTime = DateTime.UtcNow;
            logger.LogInformation("Начало сравнения UnityEvent: {ExistingCount} существующих, {NewCount} новых",
                request.ExistingEvents.Count, request.NewEvents.Count);

            try
            {
                // Создаем словари для быстрого поиска
                Dictionary<string, UnityEvent> existingEventsDict = request.ExistingEvents.ToDictionary(e => e.FullName);
                Dictionary<string, UnityEvent> newEventsDict = request.NewEvents.ToDictionary(e => e.FullName);

                List<UnityEvent> newItems = new();
                List<ChangeDetectionResult<UnityEvent>.ModifiedItem> modifiedItems = new();
                List<UnityEvent> deletedItems = new();
                List<UnityEvent> unchangedItems = new();

                // Поиск новых и измененных элементов
                foreach (UnityEvent newEvent in request.NewEvents)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (existingEventsDict.TryGetValue(newEvent.FullName, out UnityEvent? existingEvent))
                    {
                        // Проверяем, изменился ли элемент
                        List<string> changedProperties = GetChangedProperties(existingEvent, newEvent);
                        if (changedProperties.Any())
                        {
                            modifiedItems.Add(new(
                                existingEvent, newEvent, changedProperties));
                            logger.LogDebug("Обнаружены изменения в UnityEvent {FullName}: {ChangedProperties}",
                                newEvent.FullName, string.Join(", ", changedProperties));
                        }
                        else
                        {
                            unchangedItems.Add(newEvent);
                        }
                    }
                    else
                    {
                        // Новый элемент
                        newItems.Add(newEvent);
                        logger.LogDebug("Обнаружен новый UnityEvent: {FullName}", newEvent.FullName);
                    }
                }

                // Поиск удаленных элементов
                foreach (UnityEvent existingEvent in request.ExistingEvents)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (!newEventsDict.ContainsKey(existingEvent.FullName))
                    {
                        deletedItems.Add(existingEvent);
                        logger.LogDebug("Обнаружен удаленный UnityEvent: {FullName}", existingEvent.FullName);
                    }
                }

                TimeSpan executionTime = DateTime.UtcNow - startTime;
                logger.LogInformation(
                    "Сравнение UnityEvent завершено за {ExecutionTime}. " +
                    "Новых: {NewCount}, измененных: {ModifiedCount}, удаленных: {DeletedCount}, неизмененных: {UnchangedCount}",
                    executionTime, newItems.Count, modifiedItems.Count, deletedItems.Count, unchangedItems.Count);

                ChangeDetectionResult<UnityEvent> result = new(
                    newItems,
                    modifiedItems,
                    deletedItems,
                    unchangedItems,
                    newItems.Count,
                    modifiedItems.Count,
                    deletedItems.Count,
                    unchangedItems.Count,
                    executionTime);

                return new(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка при сравнении UnityEvent");
                throw;
            }
        }

        /// <summary>
        /// Определяет измененные свойства между двумя UnityEvent.
        /// </summary>
        /// <param name="existing">Существующий UnityEvent.</param>
        /// <param name="newEvent">Новый UnityEvent.</param>
        /// <returns>Список измененных свойств.</returns>
        private static List<string> GetChangedProperties(UnityEvent existing, UnityEvent newEvent)
        {
            List<string> changedProperties = new();

            // Сравнение основных свойств
            if (existing.Name != newEvent.Name)
            {
                changedProperties.Add(nameof(UnityEvent.Name));
            }

            if (existing.EventType != newEvent.EventType)
            {
                changedProperties.Add(nameof(UnityEvent.EventType));
            }

            if (existing.ArgumentsHash != newEvent.ArgumentsHash)
            {
                changedProperties.Add(nameof(UnityEvent.ArgumentsHash));
            }

            if (existing.FilePath != newEvent.FilePath)
            {
                changedProperties.Add(nameof(UnityEvent.FilePath));
            }

            if (existing.IsInherited != newEvent.IsInherited)
            {
                changedProperties.Add(nameof(UnityEvent.IsInherited));
            }

            if (existing.BaseClassFullName != newEvent.BaseClassFullName)
            {
                changedProperties.Add(nameof(UnityEvent.BaseClassFullName));
            }

            // Сравнение массива аргументов
            if (!AreArraysEqual(existing.GenericTypeArguments, newEvent.GenericTypeArguments))
            {
                changedProperties.Add(nameof(UnityEvent.GenericTypeArguments));
            }

            return changedProperties;
        }

        /// <summary>
        /// Проверяет равенство двух массивов строк.
        /// </summary>
        private static bool AreArraysEqual(string[]? array1, string[]? array2)
        {
            if (array1 == null && array2 == null)
            {
                return true;
            }

            if (array1 == null || array2 == null)
            {
                return false;
            }

            if (array1.Length != array2.Length)
            {
                return false;
            }

            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
