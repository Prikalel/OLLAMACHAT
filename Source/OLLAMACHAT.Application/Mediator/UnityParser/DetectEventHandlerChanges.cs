namespace VelikiyPrikalel.OLLAMACHAT.Application.Mediator.UnityParser;

/// <summary>
/// Команда для сравнения изменений в EventHandler между существующими и новыми данными.
/// </summary>
public sealed class DetectEventHandlerChanges
{
    /// <summary>
    /// Запрос команды для сравнения EventHandler.
    /// </summary>
    /// <param name="ExistingHandlers">Существующие EventHandler из базы данных.</param>
    /// <param name="NewHandlers">Новые EventHandler из анализа кода.</param>
    public sealed record Command(
        IReadOnlyList<EventHandler> ExistingHandlers,
        IReadOnlyList<EventHandler> NewHandlers) : IRequest<Response>;

    /// <summary>
    /// Ответ команды с результатами сравнения.
    /// </summary>
    public sealed record Response(ChangeDetectionResult<EventHandler> Result);

    /// <summary>
    /// Обработчик команды для сравнения EventHandler.
    /// </summary>
    public sealed class Handler(
        ILogger<Handler> logger) : IRequestHandler<Command, Response>
    {
        /// <summary>
        /// Обрабатывает команду сравнения EventHandler.
        /// </summary>
        /// <param name="request">Запрос команды.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Результат сравнения.</returns>
        public async ValueTask<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            DateTime startTime = DateTime.UtcNow;
            logger.LogInformation("Начало сравнения EventHandler: {ExistingCount} существующих, {NewCount} новых",
                request.ExistingHandlers.Count, request.NewHandlers.Count);

            try
            {
                // Создаем словари для быстрого поиска
                Dictionary<string, EventHandler> existingHandlersDict = request.ExistingHandlers.ToDictionary(h => h.FullName);
                Dictionary<string, EventHandler> newHandlersDict = request.NewHandlers
                    .DistinctBy(x => x.FullName)
                    .ToDictionary(h => h.FullName);

                List<EventHandler> newItems = new();
                List<ChangeDetectionResult<EventHandler>.ModifiedItem> modifiedItems = new();
                List<EventHandler> deletedItems = new();
                List<EventHandler> unchangedItems = new();

                // Поиск новых и измененных элементов
                foreach (EventHandler newHandler in request.NewHandlers)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (existingHandlersDict.TryGetValue(newHandler.FullName, out EventHandler? existingHandler))
                    {
                        // Проверяем, изменился ли элемент
                        List<string> changedProperties = GetChangedProperties(existingHandler, newHandler);
                        if (changedProperties.Any())
                        {
                            modifiedItems.Add(new(
                                existingHandler, newHandler, changedProperties));
                            logger.LogDebug("Обнаружены изменения в EventHandler {FullName}: {ChangedProperties}",
                                newHandler.FullName, string.Join(", ", changedProperties));
                        }
                        else
                        {
                            unchangedItems.Add(newHandler);
                        }
                    }
                    else
                    {
                        // Новый элемент
                        newItems.Add(newHandler);
                        logger.LogDebug("Обнаружен новый EventHandler: {FullName}", newHandler.FullName);
                    }
                }

                // Поиск удаленных элементов
                foreach (EventHandler existingHandler in request.ExistingHandlers)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (!newHandlersDict.ContainsKey(existingHandler.FullName))
                    {
                        deletedItems.Add(existingHandler);
                        logger.LogDebug("Обнаружен удаленный EventHandler: {FullName}", existingHandler.FullName);
                    }
                }

                TimeSpan executionTime = DateTime.UtcNow - startTime;
                logger.LogInformation(
                    "Сравнение EventHandler завершено за {ExecutionTime}. " +
                    "Новых: {NewCount}, измененных: {ModifiedCount}, удаленных: {DeletedCount}, неизмененных: {UnchangedCount}",
                    executionTime, newItems.Count, modifiedItems.Count, deletedItems.Count, unchangedItems.Count);

                ChangeDetectionResult<EventHandler> result = new(
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
                logger.LogError(ex, "Ошибка при сравнении EventHandler");
                throw;
            }
        }

        /// <summary>
        /// Определяет измененные свойства между двумя EventHandler.
        /// </summary>
        /// <param name="existing">Существующий EventHandler.</param>
        /// <param name="newHandler">Новый EventHandler.</param>
        /// <returns>Список измененных свойств.</returns>
        private static List<string> GetChangedProperties(EventHandler existing, EventHandler newHandler)
        {
            List<string> changedProperties = new();

            // Сравнение основных свойств
            if (existing.Name != newHandler.Name)
            {
                changedProperties.Add(nameof(EventHandler.Name));
            }

            if (existing.ArgumentsHash != newHandler.ArgumentsHash)
            {
                changedProperties.Add(nameof(EventHandler.ArgumentsHash));
            }

            if (existing.FilePath != newHandler.FilePath)
            {
                changedProperties.Add(nameof(EventHandler.FilePath));
            }

            if (existing.IsInherited != newHandler.IsInherited)
            {
                changedProperties.Add(nameof(EventHandler.IsInherited));
            }

            if (existing.BaseClassFullName != newHandler.BaseClassFullName)
            {
                changedProperties.Add(nameof(EventHandler.BaseClassFullName));
            }

            if (existing.IsPublic != newHandler.IsPublic)
            {
                changedProperties.Add(nameof(EventHandler.IsPublic));
            }

            // Сравнение массива параметров
            if (!AreArraysEqual(existing.ParameterTypes, newHandler.ParameterTypes))
            {
                changedProperties.Add(nameof(EventHandler.ParameterTypes));
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
