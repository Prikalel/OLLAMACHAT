namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Services.Parser;

/// <inheritdoc />
public partial class EntityService(
    IMapperService mapperService,
    ILogger<EntityService> logger,
    IOptions<SolutionSettings> solutionSettings,
    ISolutionLoaderService solutionLoaderService) : IEntityService
{
    private static readonly ConcurrentDictionary<string, List<ParsedEntity>> entityCache = new();

    /// <summary>
    /// Очистить кеш.
    /// </summary>
    public static void ClearCache() => entityCache.Clear();

    /// <inheritdoc />
    public async Task<IEnumerable<ParsedEntity>> ExtractEntitiesAsync(Document document, ParserOptions? options = null)
    {
        if (document == null)
        {
            logger.LogError("Document is null, returning empty collection");
            return Enumerable.Empty<ParsedEntity>();
        }

        if (string.IsNullOrEmpty(document.FilePath))
        {
            logger.LogError("Document file path is null or empty, returning empty collection");
            return Enumerable.Empty<ParsedEntity>();
        }

        var startTime = DateTime.UtcNow;
        logger.LogTrace("Starting entity extraction from document: {DocumentPath} at {StartTime}", document.FilePath, startTime);

        try
        {
            var optionsKey = options != null
                ? $"inh:{options.ExtractFullExtractInheritance ?? true}_depth:{options.MaxDepth ?? int.MaxValue}_using:{options.ExtractUsingStatementData ?? true}"
                : "inh:true_depth:int.MaxValue_using:true";
            var cacheKey = $"{document.FilePath}_{document.Id}_{optionsKey}";

            if (entityCache.TryGetValue(cacheKey, out var cachedEntity))
            {
                var cacheTime = DateTime.UtcNow - startTime;
                logger.LogInformation("Returning cached entities for document: {DocumentPath} with options: {OptionsKey} in {ElapsedMs}ms",
                    document.FilePath, optionsKey, cacheTime.TotalMilliseconds);
                return cachedEntity;
            }

            logger.LogDebug("Getting syntax tree for document: {DocumentPath}", document.FilePath);
            var syntaxTree = await document.GetSyntaxTreeAsync();
            if (syntaxTree == null)
            {
                logger.LogWarning("Syntax tree is null for document: {DocumentPath}", document.FilePath);
                return Enumerable.Empty<ParsedEntity>();
            }

            logger.LogDebug("Getting syntax root for document: {DocumentPath}", document.FilePath);
            var root = await syntaxTree.GetRootAsync();
            if (root == null)
            {
                logger.LogWarning("Syntax root is null for document: {DocumentPath}", document.FilePath);
                return Enumerable.Empty<ParsedEntity>();
            }

            GetSyntaxErrors(document, syntaxTree);

            logger.LogDebug("Getting semantic model for document: {DocumentPath}", document.FilePath);
            var semanticModel = await document.GetSemanticModelAsync();
            if (semanticModel == null)
            {
                logger.LogWarning("Semantic model is null for document: {DocumentPath}", document.FilePath);
                return Enumerable.Empty<ParsedEntity>();
            }

            // Проверка на очень большие файлы
            var text = await document.GetTextAsync();
            if (text.Length > MaxFileSizeWarningBytes)
            {
                logger.LogWarning("Large file detected ({Size} bytes) for document: {DocumentPath}", text.Length, document.FilePath);
            }

            var entities = new List<ParsedEntity>();
            var entityTasks = new List<Task<ParsedEntity?>>();

            // Оптимизация: получаем все узлы одного типа за один вызов DescendantNodes()
            var allNodes = root.DescendantNodes().ToList();

            // Extract using directives - только если опция ExtractUsingStatementData включена
            var usingDirectives = allNodes.OfType<UsingDirectiveSyntax>();
            if (options?.ExtractUsingStatementData != false)
            {
                foreach (var usingDirective in usingDirectives)
                {
                    var usingEntity = await ExtractUsingDirectiveAsync(usingDirective, semanticModel, options?.ExtractFullExtractInheritance is true);
                    if (usingEntity != null)
                    {
                        entities.Add(usingEntity);
                    }
                }
            }

            // Extract namespace declarations
            var namespaceDeclarations = allNodes.OfType<BaseNamespaceDeclarationSyntax>();
            foreach (var namespaceDeclaration in namespaceDeclarations)
            {
                var namespaceSymbol = semanticModel.GetDeclaredSymbol(namespaceDeclaration);
                if (namespaceSymbol != null)
                {
                    entityTasks.Add(ExtractEntityAsync(namespaceSymbol, semanticModel, 0, options));
                }
            }

            // Extract type declarations (classes, interfaces, structs, enums, records)
            var typeDeclarations = allNodes.OfType<TypeDeclarationSyntax>();
            foreach (var typeDeclaration in typeDeclarations)
            {
                var typeSymbol = semanticModel.GetDeclaredSymbol(typeDeclaration);
                if (typeSymbol != null)
                {
                    entityTasks.Add(ExtractEntityAsync(typeSymbol, semanticModel, 0, options));
                }
                else
                {
                    logger.LogWarning("Could not get symbol for type declaration at line {LineNumber}",
                        typeDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line + 1);
                }
            }

            // Extract method declarations (standalone methods)
            var methodDeclarations = allNodes.OfType<MethodDeclarationSyntax>();
            foreach (var methodDeclaration in methodDeclarations)
            {
                var methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration);
                if (methodSymbol != null && methodSymbol.ContainingType == null)
                {
                    entityTasks.Add(ExtractEntityAsync(methodSymbol, semanticModel, 0, options));
                }
            }

            // Extract property declarations (standalone properties)
            var propertyDeclarations = allNodes.OfType<PropertyDeclarationSyntax>();
            foreach (var propertyDeclaration in propertyDeclarations)
            {
                var propertySymbol = semanticModel.GetDeclaredSymbol(propertyDeclaration);
                if (propertySymbol != null && propertySymbol.ContainingType == null)
                {
                    entityTasks.Add(ExtractEntityAsync(propertySymbol, semanticModel, 0, options));
                }
            }

            // Extract field declarations (standalone fields)
            var fieldDeclarations = allNodes.OfType<FieldDeclarationSyntax>();
            foreach (var fieldDeclaration in fieldDeclarations)
            {
                foreach (var variable in fieldDeclaration.Declaration.Variables)
                {
                    var fieldSymbol = semanticModel.GetDeclaredSymbol(variable);
                    if (fieldSymbol != null && fieldSymbol.ContainingType == null)
                    {
                        entityTasks.Add(ExtractEntityAsync(fieldSymbol, semanticModel, 0, options));
                    }
                }
            }

            // Extract event declarations (standalone events)
            var eventDeclarations = allNodes.OfType<EventDeclarationSyntax>();
            foreach (var eventDeclaration in eventDeclarations)
            {
                var eventSymbol = semanticModel.GetDeclaredSymbol(eventDeclaration);
                if (eventSymbol != null && eventSymbol.ContainingType == null)
                {
                    entityTasks.Add(ExtractEntityAsync(eventSymbol, semanticModel, 0, options));
                }
            }

            // Extract event field declarations (standalone event fields)
            var eventFieldDeclarations = allNodes.OfType<EventFieldDeclarationSyntax>();
            foreach (var eventFieldDeclaration in eventFieldDeclarations)
            {
                foreach (var variable in eventFieldDeclaration.Declaration.Variables)
                {
                    var eventSymbol = semanticModel.GetDeclaredSymbol(variable);
                    if (eventSymbol != null && eventSymbol.ContainingType == null)
                    {
                        entityTasks.Add(ExtractEntityAsync(eventSymbol, semanticModel, 0, options));
                    }
                }
            }

            // Extract enum member declarations
            var enumMemberDeclarations = allNodes.OfType<EnumMemberDeclarationSyntax>();
            foreach (var enumMember in enumMemberDeclarations)
            {
                var enumMemberSymbol = semanticModel.GetDeclaredSymbol(enumMember);
                if (enumMemberSymbol != null && enumMemberSymbol.ContainingType == null && enumMemberSymbol.ContainingNamespace == null)
                {
                    entityTasks.Add(ExtractEntityAsync(enumMemberSymbol, semanticModel, 0, options));
                }
            }

            // Оптимизация: выполняем все задачи параллельно
            logger.LogDebug("Processing {TaskCount} entity extraction tasks in parallel for document: {DocumentPath}",
                entityTasks.Count, document.FilePath);

            var entityResults = await Task.WhenAll(entityTasks);
            var validEntities = entityResults.Where(entity => entity != null)!;
            entities.AddRange(validEntities);

            // Фильтруем дочерние сущности, чтобы оставить только сущности верхнего уровня
            var filteredEntities = FilterTopLevelEntities(entities);

            var endTime = DateTime.UtcNow;
            var elapsedTime = endTime - startTime;

            logger.LogInformation("Successfully extracted {EntityCount} entities from document: {DocumentPath} in {ElapsedMs}ms, filtered to {FilteredCount} top-level entities",
                entities.Count, document.FilePath, elapsedTime.TotalMilliseconds, filteredEntities.Count);

            if (filteredEntities.Any())
            {
                entityCache.TryAdd(cacheKey, filteredEntities);
                logger.LogDebug("Cached extraction result for document: {DocumentPath} with options: {OptionsKey}",
                    document.FilePath, optionsKey);
            }

            return filteredEntities;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error extracting entities from document: {DocumentPath}", document.FilePath);
            throw;
        }
    }

    private List<Diagnostic> GetSyntaxErrors(Document document, SyntaxTree syntaxTree)
    {
        var diagnostics = syntaxTree.GetDiagnostics();
        var syntaxErrors = diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
        if (syntaxErrors.Any())
        {
            logger.LogWarning("Found {ErrorCount} syntax errors in document: {DocumentPath}", syntaxErrors.Count, document.FilePath);
            foreach (var error in syntaxErrors)
            {
                throw new Exception($"Syntax error at line {error.Location.GetLineSpan().StartLinePosition.Line + 1}: {error.GetMessage()}");
            }
        }

        return syntaxErrors;
    }

    /// <summary>
    /// Фильтрует сущности, оставляя только те, что находятся на верхнем уровне иерархии
    /// Исключает дочерние сущности, которые уже содержатся в Children других сущностей
    /// </summary>
    /// <param name="entities">Список всех извлеченных сущностей</param>
    /// <returns>Отфильтрованный список сущностей верхнего уровня</returns>
    private List<ParsedEntity> FilterTopLevelEntities(List<ParsedEntity> entities)
    {
        if (entities == null || entities.Count == 0)
        {
            return new List<ParsedEntity>();
        }

        logger.LogDebug("Filtering {EntityCount} entities to top-level only", entities.Count);

        var childEntityFullNames = new HashSet<string>();

        foreach (var entity in entities)
        {
            CollectChildEntityFullNames(entity, childEntityFullNames);
        }

        logger.LogDebug("Found {ChildCount} child entities to exclude", childEntityFullNames.Count);

        var topLevelEntities = entities
            .Where(entity => !childEntityFullNames.Contains(entity.FullName))
            .ToList();

        logger.LogDebug("Filtered to {TopLevelCount} top-level entities", topLevelEntities.Count);

        return topLevelEntities;
    }

    /// <summary>
    /// Рекурсивно собирает FullName всех дочерних сущностей
    /// </summary>
    /// <param name="entity">Родительская сущность</param>
    /// <param name="childFullNames">Набор для хранения FullName дочерних сущностей</param>
    private void CollectChildEntityFullNames(ParsedEntity entity, HashSet<string> childFullNames)
    {
        if (entity?.Children == null || !entity.Children.Any())
        {
            return;
        }

        foreach (var child in entity.Children)
        {
            if (!string.IsNullOrEmpty(child.FullName))
            {
                childFullNames.Add(child.FullName);
            }

            CollectChildEntityFullNames(child, childFullNames);
        }
    }
}
