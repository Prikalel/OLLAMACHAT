namespace VelikiyPrikalel.OLLAMACHAT.Application.Mediator.UnityParser;

/// <summary>
/// Команда для сравнения изменений в связях между существующими и новыми данными.
/// </summary>
public sealed class DetectRelationshipChanges
{
    /// <summary>
    /// Запрос команды для сравнения связей.
    /// </summary>
    /// <param name="ExistingRelationships">Существующие связи из базы данных.</param>
    /// <param name="NewRelationships">Новые связи из анализа кода.</param>
    public sealed record Command(
        IReadOnlyList<EventHandlerRelationship> ExistingRelationships,
        IReadOnlyList<EventHandlerRelationship> NewRelationships) : IRequest<Response>;

    /// <summary>
    /// Ответ команды с результатами сравнения.
    /// </summary>
    public sealed record Response(ChangeDetectionResult<EventHandlerRelationship> Result);

    /// <summary>
    /// Обработчик команды для сравнения связей.
    /// </summary>
    public sealed class Handler(
        ILogger<Handler> logger) : IRequestHandler<Command, Response>
    {
        /// <summary>
        /// Обрабатывает команду сравнения связей.
        /// </summary>
        /// <param name="request">Запрос команды.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Результат сравнения.</returns>
        public async ValueTask<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            DateTime startTime = DateTime.UtcNow;
            logger.LogInformation("Начало сравнения связей: {ExistingCount} существующих, {NewCount} новых",
                request.ExistingRelationships.Count, request.NewRelationships.Count);

            try
            {
                // Для связей используем комбинацию EventId + HandlerId как уникальный ключ
                Dictionary<string, EventHandlerRelationship> existingRelationshipsDict = request.ExistingRelationships.ToDictionary(
                    r => $"{r.EventId}_{r.HandlerId}");
                Dictionary<string, EventHandlerRelationship> newRelationshipsDict = request.NewRelationships.ToDictionary(
                    r => $"{r.EventId}_{r.HandlerId}");

                List<EventHandlerRelationship> newItems = new();
                List<ChangeDetectionResult<EventHandlerRelationship>.ModifiedItem> modifiedItems = new();
                List<EventHandlerRelationship> deletedItems = new();
                List<EventHandlerRelationship> unchangedItems = new();

                // Поиск новых и измененных элементов
                foreach (EventHandlerRelationship newRelationship in request.NewRelationships)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    string relationshipKey = $"{newRelationship.EventId}_{newRelationship.HandlerId}";
                    if (existingRelationshipsDict.TryGetValue(relationshipKey, out EventHandlerRelationship? existingRelationship))
                    {
                        // Проверяем, изменился ли элемент
                        List<string> changedProperties = GetChangedProperties(existingRelationship, newRelationship);
                        if (changedProperties.Any())
                        {
                            modifiedItems.Add(new(
                                existingRelationship, newRelationship, changedProperties));
                            logger.LogDebug("Обнаружены изменения в связи {EventId}-{HandlerId}: {ChangedProperties}",
                                newRelationship.EventId, newRelationship.HandlerId, string.Join(", ", changedProperties));
                        }
                        else
                        {
                            unchangedItems.Add(newRelationship);
                        }
                    }
                    else
                    {
                        // Новая связь
                        newItems.Add(newRelationship);
                        logger.LogDebug("Обнаружена новая связь: EventId={EventId}, HandlerId={HandlerId}",
                            newRelationship.EventId, newRelationship.HandlerId);
                    }
                }

                // Поиск удаленных связей
                foreach (EventHandlerRelationship existingRelationship in request.ExistingRelationships)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    string relationshipKey = $"{existingRelationship.EventId}_{existingRelationship.HandlerId}";
                    if (!newRelationshipsDict.ContainsKey(relationshipKey))
                    {
                        deletedItems.Add(existingRelationship);
                        logger.LogDebug("Обнаружена удаленная связь: EventId={EventId}, HandlerId={HandlerId}",
                            existingRelationship.EventId, existingRelationship.HandlerId);
                    }
                }

                TimeSpan executionTime = DateTime.UtcNow - startTime;
                logger.LogInformation(
                    "Сравнение связей завершено за {ExecutionTime}. " +
                    "Новых: {NewCount}, измененных: {ModifiedCount}, удаленных: {DeletedCount}, неизмененных: {UnchangedCount}",
                    executionTime, newItems.Count, modifiedItems.Count, deletedItems.Count, unchangedItems.Count);

                ChangeDetectionResult<EventHandlerRelationship> result = new(
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
                logger.LogError(ex, "Ошибка при сравнении связей");
                throw;
            }
        }

        /// <summary>
        /// Определяет измененные свойства между двумя связями.
        /// </summary>
        /// <param name="existing">Существующая связь.</param>
        /// <param name="newRelationship">Новая связь.</param>
        /// <returns>Список измененных свойств.</returns>
        private static List<string> GetChangedProperties(EventHandlerRelationship existing, EventHandlerRelationship newRelationship)
        {
            List<string> changedProperties = new();

            // Сравнение типа связи
            if (existing.EventHandlerRelationshipType != newRelationship.EventHandlerRelationshipType)
            {
                changedProperties.Add(nameof(EventHandlerRelationship.EventHandlerRelationshipType));
            }

            // Сравнение источника
            if (existing.Source != newRelationship.Source)
            {
                changedProperties.Add(nameof(EventHandlerRelationship.Source));
            }

            // Сравнение причины удаления
            if (existing.DeletionReason != newRelationship.DeletionReason)
            {
                changedProperties.Add(nameof(EventHandlerRelationship.DeletionReason));
            }

            // Сравнение флага удаления
            if (existing.IsDeleted != newRelationship.IsDeleted)
            {
                changedProperties.Add(nameof(EventHandlerRelationship.IsDeleted));
            }

            return changedProperties;
        }
    }
}
