namespace VelikiyPrikalel.OLLAMACHAT.Application.Mediator.UnityParser;

/// <summary>
/// Команда для заполнения данных UnityEvent из исходного кода проекта.
/// Реализует подход "сканирование-сравнение-обновление" согласно документу #29.
/// </summary>
public sealed class PopulateUnityEventData
{
    /// <summary>
    /// Запрос команды для заполнения данных UnityEvent.
    /// </summary>
    public sealed record Command : IRequest<Response>;

    /// <summary>
    /// Ответ команды с результатом выполнения.
    /// </summary>
    /// <param name="ProcessedFiles">Количество обработанных файлов.</param>
    /// <param name="UnityEvents">Найдено UnityEvent.</param>
    /// <param name="Handlers">Найдено обработчиков.</param>
    /// <param name="Relationships">Создано связей.</param>
    /// <param name="ExecutionTime">Время выполнения.</param>
    public sealed record Response(
        int ProcessedFiles,
        int UnityEvents,
        int Handlers,
        int Relationships,
        TimeSpan ExecutionTime);

    /// <summary>
    /// Обработчик команды для заполнения данных UnityEvent.
    /// </summary>
    public sealed class Handler(
        ISolutionLoaderService solutionLoaderService,
        IEntityService entityService,
        IRepository<UnityEvent> eventRepository,
        IRepository<EventHandler> handlerRepository,
        IRepository<EventHandlerRelationship> relationshipRepository,
        IChangeDetectionService changeDetectionService,
        IArgumentCompatibilityService argumentCompatibilityService,
        ILogger<Handler> logger,
        UnityEventFactory unityEventFactory,
        EventHandlerFactory eventHandlerFactory
        ) : IRequestHandler<Command, Response>
    {
        /// <summary>
        /// Обрабатывает команду заполнения данных UnityEvent.
        /// </summary>
        /// <param name="request">Запрос команды.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Результат выполнения команды.</returns>
        public async ValueTask<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            DateTime startTime = DateTime.UtcNow;
            logger.LogInformation("Начало заполнения данных UnityEvent согласно блок-схеме документа #29");

            try
            {
                // 1. Загрузить существующие данные из БД
                logger.LogDebug("Шаг 1: Загрузка существующих данных из БД");
                IReadOnlyList<UnityEvent> existingEvents = await GetAllAsync(eventRepository);
                IReadOnlyList<EventHandler> existingHandlers = await GetAllAsync(handlerRepository);
                IReadOnlyList<EventHandlerRelationship> existingRelationships = await GetAllAsync(relationshipRepository);

                logger.LogInformation("Загружено существующих данных: {EventsCount} событий, {HandlersCount} обработчиков, {RelationshipsCount} связей",
                    existingEvents.Count, existingHandlers.Count, existingRelationships.Count);

                // 2. Сканировать проект и получить новые данные
                logger.LogDebug("Шаг 2: Сканирование проекта и извлечение данных");
                (IReadOnlyList<UnityEvent> scannedEvents, IReadOnlyList<EventHandler> scannedHandlers, int processedFiles) = await ScanProjectAsync(cancellationToken);

                logger.LogInformation("Отсканировано: {FilesCount} файлов, найдено {EventsCount} событий, {HandlersCount} обработчиков",
                    processedFiles, scannedEvents.Count, scannedHandlers.Count);

                // 3. Сравнить и обновить события
                logger.LogDebug("Шаг 3: Сравнение и обновление событий");
                ChangeDetectionResult<UnityEvent> eventComparisonResult = await changeDetectionService.CompareUnityEventsAsync(
                    existingEvents, scannedEvents, cancellationToken);

                await ProcessEventChangesAsync(eventComparisonResult, cancellationToken);

                // 4. Сравнить и обновить обработчики
                logger.LogDebug("Шаг 4: Сравнение и обновление обработчиков");
                ChangeDetectionResult<EventHandler> handlerComparisonResult = await changeDetectionService.CompareEventHandlersAsync(
                    existingHandlers, scannedHandlers, cancellationToken);

                await ProcessHandlerChangesAsync(handlerComparisonResult, cancellationToken);

                // 5. Загрузить обновленные данные после изменений
                logger.LogDebug("Шаг 5: Загрузка обновленных данных после изменений");
                IReadOnlyList<UnityEvent> updatedEvents = await GetAllAsync(eventRepository);
                IReadOnlyList<EventHandler> updatedHandlers = await GetAllAsync(handlerRepository);

                // 6. Анализ возможных связей и обновление существующих
                logger.LogDebug("Шаг 6: Анализ и обновление связей");
                RelationshipAnalysisResult relationshipAnalysisResult = await AnalyzeAndUpdateRelationshipsAsync(
                    updatedEvents, updatedHandlers, existingRelationships, cancellationToken);

                // 7. Пометить устаревшие связи
                logger.LogDebug("Шаг 7: Пометка устаревших связей");
                await MarkObsoleteRelationshipsAsync(relationshipAnalysisResult, cancellationToken);

                TimeSpan executionTime = DateTime.UtcNow - startTime;
                int totalRelationships = relationshipAnalysisResult.CreatedCount + relationshipAnalysisResult.UpdatedCount;

                logger.LogInformation("Заполнение данных UnityEvent завершено за {ExecutionTime}: " +
                    "обработано {FilesCount} файлов, найдено {EventsCount} событий, {HandlersCount} обработчиков, создано/обновлено {RelationshipsCount} связей",
                    executionTime, processedFiles, updatedEvents.Count, updatedHandlers.Count, totalRelationships);

                return new(processedFiles, updatedEvents.Count, updatedHandlers.Count, totalRelationships, executionTime);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка при заполнении данных UnityEvent");
                throw;
            }
        }

        /// <summary>
        /// Сканирует проект и извлекает UnityEvent и EventHandler из исходного кода.
        /// </summary>
        private async Task<(IReadOnlyList<UnityEvent> Events, IReadOnlyList<EventHandler> Handlers, int ProcessedFiles)> ScanProjectAsync(
            CancellationToken cancellationToken)
        {
            List<UnityEvent> events = new();
            List<EventHandler> handlers = new();
            int processedFiles = 0;

            if (!solutionLoaderService.IsSolutionLoaded)
            {
                logger.LogWarning("Решение не загружено");
                return (events, handlers, processedFiles);
            }

            IEnumerable<Project> projects = solutionLoaderService.GetProjects();
            logger.LogDebug("Найдено {ProjectsCount} проектов в решении", projects.Count());

            foreach (Project project in projects)
            {
                IEnumerable<Document> documents = solutionLoaderService.GetProjectDocuments(project.Name);
                logger.LogDebug("Проект {ProjectName}: {DocumentsCount} документов", project.Name, documents.Count());

                foreach (Document document in documents)
                {
                    try
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        // Извлекаем сущности из документа
                        List<ParsedEntity> entities = (await entityService.ExtractEntitiesAsync(document)).FlattenAllTrees();
                        if (!entities.Any())
                        {
                            continue;
                        }

                        processedFiles++;

                        // Обрабатываем UnityEvent
                        IEnumerable<ParsedEntity> unityEvents = entities.Where(e => IsUnityEventEntity(e));
                        foreach (ParsedEntity entity in unityEvents)
                        {
                            UnityEvent eventResult = await unityEventFactory.Create(entity, document.FilePath!, cancellationToken);
                            if (!eventResult.IsDeleted)
                            {
                                events.Add(eventResult);
                            }
                        }

                        // Обрабатываем EventHandler
                        IEnumerable<ParsedEntity> eventHandlers = entities.Where(e => IsEventHandlerEntity(e));
                        foreach (ParsedEntity entity in eventHandlers)
                        {
                            EventHandler handlerResult = await eventHandlerFactory.Create(entity, document.FilePath!, cancellationToken);
                            if (!handlerResult.IsDeleted)
                            {
                                handlers.Add(handlerResult);
                            }
                        }

                        // Обрабатываем унаследованные обработчики
                        await ProcessInheritedEventHandlersAsync(entities, document.FilePath!, handlers, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Ошибка при обработке файла: {FilePath}", document.FilePath);
                    }
                }
            }

            return (events, handlers, processedFiles);
        }

        /// <summary>
        /// Обрабатывает изменения в событиях на основе результата сравнения.
        /// </summary>
        private async Task ProcessEventChangesAsync(ChangeDetectionResult<UnityEvent> comparisonResult, CancellationToken cancellationToken)
        {
            // Новые события уже созданы в процессе сканирования
            logger.LogDebug("Найдено изменений событий: {NewCount} новых, {ModifiedCount} измененных, {DeletedCount} удаленных",
                comparisonResult.TotalNewCount, comparisonResult.TotalModifiedCount, comparisonResult.TotalDeletedCount);

            // Обрабатываем измененные события
            foreach (ChangeDetectionResult<UnityEvent>.ModifiedItem modifiedEvent in comparisonResult.ModifiedItems)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    UnityEvent existingEvent = modifiedEvent.ExistingItem;
                    UnityEvent newEvent = modifiedEvent.NewItem;

                    // Обновляем существующее событие
                    existingEvent.EventType = newEvent.EventType;
                    existingEvent.GenericTypeArguments = newEvent.GenericTypeArguments;
                    existingEvent.ArgumentsHash = newEvent.ArgumentsHash;
                    existingEvent.FilePath = newEvent.FilePath;
                    existingEvent.IsInherited = newEvent.IsInherited;
                    existingEvent.BaseClassFullName = newEvent.BaseClassFullName;
                    existingEvent.UpdateLastModified();

                    await eventRepository.UpdateAsync(existingEvent);
                    logger.LogDebug("Обновлено событие: {EventName}", existingEvent.Name);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Ошибка при обновлении события: {EventName}", modifiedEvent.ExistingItem.Name);
                }
            }

            // Помечаем удаленные события
            foreach (UnityEvent deletedEvent in comparisonResult.DeletedItems)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    deletedEvent.MarkAsDeleted();
                    await eventRepository.UpdateAsync(deletedEvent);
                    logger.LogDebug("Помечено как удаленное событие: {EventName}", deletedEvent.Name);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Ошибка при пометке события как удаленного: {EventName}", deletedEvent.Name);
                }
            }
        }

        /// <summary>
        /// Обрабатывает изменения в обработчиках на основе результата сравнения.
        /// </summary>
        private async Task ProcessHandlerChangesAsync(ChangeDetectionResult<EventHandler> comparisonResult, CancellationToken cancellationToken)
        {
            // Новые обработчики уже созданы в процессе сканирования
            logger.LogDebug("Найдено изменений обработчиков: {NewCount} новых, {ModifiedCount} измененных, {DeletedCount} удаленных",
                comparisonResult.TotalNewCount, comparisonResult.TotalModifiedCount, comparisonResult.TotalDeletedCount);

            // Обрабатываем измененные обработчики
            foreach (ChangeDetectionResult<EventHandler>.ModifiedItem modifiedHandler in comparisonResult.ModifiedItems)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    EventHandler existingHandler = modifiedHandler.ExistingItem;
                    EventHandler newHandler = modifiedHandler.NewItem;

                    // Обновляем существующий обработчик
                    existingHandler.ParameterTypes = newHandler.ParameterTypes;
                    existingHandler.ArgumentsHash = newHandler.ArgumentsHash;
                    existingHandler.FilePath = newHandler.FilePath;
                    existingHandler.IsInherited = newHandler.IsInherited;
                    existingHandler.BaseClassFullName = newHandler.BaseClassFullName;
                    existingHandler.IsPublic = newHandler.IsPublic;
                    existingHandler.UpdateLastModified();

                    await handlerRepository.UpdateAsync(existingHandler);
                    logger.LogDebug("Обновлен обработчик: {HandlerName}", existingHandler.Name);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Ошибка при обновлении обработчика: {HandlerName}", modifiedHandler.ExistingItem.Name);
                }
            }

            // Помечаем удаленные обработчики
            foreach (EventHandler deletedHandler in comparisonResult.DeletedItems)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    deletedHandler.MarkAsDeleted();
                    await handlerRepository.UpdateAsync(deletedHandler);
                    logger.LogDebug("Помечен как удаленный обработчик: {HandlerName}", deletedHandler.Name);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Ошибка при пометке обработчика как удаленного: {HandlerName}", deletedHandler.Name);
                }
            }
        }

        /// <summary>
        /// Анализирует и обновляет связи между событиями и обработчиками.
        /// </summary>
        private async Task<RelationshipAnalysisResult> AnalyzeAndUpdateRelationshipsAsync(
            IReadOnlyList<UnityEvent> events,
            IReadOnlyList<EventHandler> handlers,
            IReadOnlyList<EventHandlerRelationship> existingRelationships,
            CancellationToken cancellationToken)
        {
            int createdCount = 0;
            int updatedCount = 0;
            List<EventHandlerRelationship> relationshipsToCheck = new();

            // Анализируем возможные связи
            foreach (UnityEvent unityEvent in events)
            {
                foreach (EventHandler handler in handlers)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    // Проверяем совместимость сигнатур
                    if (argumentCompatibilityService.AreSignaturesCompatible(unityEvent, handler))
                    {
                        // Ищем существующую связь
                        EventHandlerRelationship? existingRelationship = existingRelationships.FirstOrDefault(r =>
                            r.EventId == unityEvent.Id && r.HandlerId == handler.Id && !r.IsDeleted);

                        if (existingRelationship != null)
                        {
                            // Связь существует, проверяем нужно ли обновить
                            if (existingRelationship.Source != Source.Manual &&
                                existingRelationship.EventHandlerRelationshipType != EventHandlerRelationshipType.Confirmed)
                            {
                                // Обновляем только автоматические связи
                                existingRelationship.Source = Source.Parser;
                                existingRelationship.EventHandlerRelationshipType = EventHandlerRelationshipType.Potential;
                                existingRelationship.UpdateLastModified();

                                await relationshipRepository.UpdateAsync(existingRelationship);
                                updatedCount++;
                                logger.LogDebug("Обновлена автоматическая связь: {EventName} -> {HandlerName}", unityEvent.Name, handler.Name);
                            }
                            else
                            {
                                // Сохраняем пользовательские и подтвержденные связи
                                logger.LogDebug("Сохранена пользовательская/подтвержденная связь: {EventName} -> {HandlerName}", unityEvent.Name, handler.Name);
                            }
                        }
                        else
                        {
                            // Создаем новую связь
                            EventHandlerRelationship newRelationship = new()
                            {
                                Id = Guid.NewGuid().ToString(),
                                EventId = unityEvent.Id,
                                HandlerId = handler.Id,
                                EventHandlerRelationshipType = EventHandlerRelationshipType.Potential,
                                Source = Source.Parser,
                                IsDeleted = false
                            };

                            await relationshipRepository.AddAsync(newRelationship);
                            createdCount++;
                            logger.LogDebug("Создана новая связь: {EventName} -> {HandlerName}", unityEvent.Name, handler.Name);
                        }
                    }
                }
            }

            return new(createdCount, updatedCount, relationshipsToCheck);
        }

        /// <summary>
        /// Помечает устаревшие связи.
        /// </summary>
        private async Task MarkObsoleteRelationshipsAsync(RelationshipAnalysisResult analysisResult, CancellationToken cancellationToken)
        {
            // Проверяем связи с удаленными событиями/обработчиками
            IReadOnlyList<EventHandlerRelationship> allRelationships = await GetAllAsync(relationshipRepository);
            IReadOnlyList<UnityEvent> allEvents = await GetAllAsync(eventRepository);
            IReadOnlyList<EventHandler> allHandlers = await GetAllAsync(handlerRepository);

            int obsoleteCount = 0;

            foreach (EventHandlerRelationship relationship in allRelationships)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Пропускаем пользовательские и подтвержденные связи
                if (relationship.Source == Source.Manual ||
                    relationship.EventHandlerRelationshipType == EventHandlerRelationshipType.Confirmed)
                {
                    continue;
                }

                UnityEvent? relatedEvent = allEvents.FirstOrDefault(e => e.Id == relationship.EventId);
                EventHandler? relatedHandler = allHandlers.FirstOrDefault(h => h.Id == relationship.HandlerId);

                // Если событие или обработчик удален, помечаем связь как устаревшую
                if (relatedEvent?.IsDeleted == true || relatedHandler?.IsDeleted == true)
                {
                    DeletionReason reason = relatedEvent?.IsDeleted == true ? DeletionReason.EventDeleted : DeletionReason.HandlerDeleted;
                    relationship.MarkAsDeleted(reason);
                    await relationshipRepository.UpdateAsync(relationship);
                    obsoleteCount++;
                    logger.LogDebug("Помечена как устаревшая связь из-за удаления: {Reason}", reason);
                }
                // Если сигнатуры больше не совместимы, помечаем связь как устаревшую
                else if (relatedEvent != null && relatedHandler != null &&
                         !argumentCompatibilityService.AreSignaturesCompatible(relatedEvent, relatedHandler))
                {
                    relationship.MarkAsDeleted(DeletionReason.SignatureChanged);
                    await relationshipRepository.UpdateAsync(relationship);
                    obsoleteCount++;
                    logger.LogDebug("Помечена как устаревшая связь из-за изменения сигнатуры: {EventName} -> {HandlerName}", relatedEvent.Name, relatedHandler.Name);
                }
            }

            if (obsoleteCount > 0)
            {
                logger.LogInformation("Помечено как устаревших связей: {Count}", obsoleteCount);
            }
        }

        /// <summary>
        /// Обрабатывает унаследованные обработчики событий.
        /// </summary>
        private async Task ProcessInheritedEventHandlersAsync(
            IEnumerable<ParsedEntity> entities,
            string filePath,
            List<EventHandler> handlers,
            CancellationToken cancellationToken)
        {
            foreach (ParsedEntity entity in entities.Where(e => e.Type == ParsedEntityType.Class))
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (entity.Inheritance?.AllBaseClasses?.Any() != true)
                {
                    continue;
                }

                // Для каждого базового класса ищем обработчики
                foreach (string baseClassFullName in entity.Inheritance.AllBaseClasses)
                {
                    // Ищем обработчики в базовых классах
                    IEnumerable<ParsedEntity> baseClassHandlers = entities.Where(e =>
                        e.Type == ParsedEntityType.Method &&
                        e.FullName?.StartsWith(baseClassFullName + ".") == true &&
                        IsEventHandlerEntity(e));

                    foreach (ParsedEntity handlerEntity in baseClassHandlers)
                    {
                        try
                        {
                            EventHandler handlerResult = await eventHandlerFactory.Create(handlerEntity, filePath, cancellationToken);
                            if (!handlerResult.IsDeleted)
                            {
                                handlers.Add(handlerResult);
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Ошибка при обработке унаследованного обработчика: {HandlerName}", handlerEntity.FullName);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Проверяет, является ли сущность UnityEvent.
        /// </summary>
        private static bool IsUnityEventEntity(ParsedEntity entity)
        {
            return entity.Type == ParsedEntityType.Property &&
                   (entity.ReturnType?.Contains("UnityEvent") == true ||
                    entity.ReturnType?.StartsWith("UnityEvent<") == true);
        }

        /// <summary>
        /// Проверяет, является ли сущность EventHandler.
        /// </summary>
        private static bool IsEventHandlerEntity(ParsedEntity entity)
        {
            return entity.Type == ParsedEntityType.Method &&
                   entity.ReturnType == "void" &&
                   entity.Modifiers?.Contains("public") == true &&
                   (entity.Modifiers?.Contains("abstract") is not true) &&
                   (entity.Modifiers?.Contains("static") is not true) &&
                   entity.SimpleName != ".ctor" &&
                   !entity.SimpleName.StartsWith("set_") &&
                   !entity.SimpleName.StartsWith("get_");
        }

        /// <summary>
        /// Вспомогательный метод для получения всех сущностей по условию "не удалено".
        /// </summary>
        private static async Task<IReadOnlyList<TEntity>> GetAllAsync<TEntity>(IRepository<TEntity> repository)
            where TEntity : IEntity
        {
            if (typeof(TEntity) == typeof(UnityEvent))
            {
                return await repository.GetMany(x => !(x as UnityEvent)!.IsDeleted);
            }
            else if (typeof(TEntity) == typeof(EventHandler))
            {
                return await repository.GetMany(x => !(x as EventHandler)!.IsDeleted);
            }
            else if (typeof(TEntity) == typeof(EventHandlerRelationship))
            {
                return await repository.GetMany(x => !(x as EventHandlerRelationship)!.IsDeleted);
            }

            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Результат анализа связей.
    /// </summary>
    private record RelationshipAnalysisResult(
        int CreatedCount,
        int UpdatedCount,
        List<EventHandlerRelationship> RelationshipsToCheck);
}
