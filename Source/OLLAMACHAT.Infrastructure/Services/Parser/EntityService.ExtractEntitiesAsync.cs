namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Services.Parser;

/// <summary>
/// Часть класса EntityService, содержащая метод ExtractEntitiesAsync
/// </summary>
public partial class EntityService
{
    /// <inheritdoc />
    public async Task<IEnumerable<ParsedEntity>> ExtractEntitiesAsync(Document document, ParserOptions? options = null)
    {
        // Проверка на null документ
        if (document == null)
        {
            logger.LogWarning("Document is null, returning empty collection");
            return Enumerable.Empty<ParsedEntity>();
        }

        // Проверка на пустой путь файла
        if (string.IsNullOrEmpty(document.FilePath))
        {
            logger.LogWarning("Document file path is null or empty, returning empty collection");
            return Enumerable.Empty<ParsedEntity>();
        }

        var startTime = DateTime.UtcNow;
        logger.LogInformation("Starting entity extraction from document: {DocumentPath} at {StartTime}", document.FilePath, startTime);

        try
        {
            // Формируем ключ кэша с учетом опций парсинга
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

            // Проверка на наличие синтаксических ошибок
            var diagnostics = syntaxTree.GetDiagnostics();
            var syntaxErrors = diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
            if (syntaxErrors.Any())
            {
                logger.LogWarning("Found {ErrorCount} syntax errors in document: {DocumentPath}", syntaxErrors.Count, document.FilePath);
                foreach (var error in syntaxErrors.Take(MaxSyntaxErrorsToLog))
                {
                    throw new Exception($"Syntax error at line {error.Location.GetLineSpan().StartLinePosition.Line + 1}: {error.GetMessage()}");
                }
            }

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

            logger.LogDebug("File analysis complete for document: {DocumentPath}, size: {Size} bytes, lines: {LineCount}",
                document.FilePath, text.Length, text.Lines.Count);

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
                if (typeSymbol != null && typeSymbol.ContainingNamespace == null)
                {
                    entityTasks.Add(ExtractEntityAsync(typeSymbol, semanticModel, 0, options));
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

            var endTime = DateTime.UtcNow;
            var elapsedTime = endTime - startTime;

            logger.LogInformation("Successfully extracted {EntityCount} entities from document: {DocumentPath} in {ElapsedMs}ms",
                entities.Count, document.FilePath, elapsedTime.TotalMilliseconds);

            // Детальная статистика по типам сущностей
            var entityStats = entities.GroupBy(e => e.Type)
                .ToDictionary(g => g.Key, g => g.Count());

            if (entityStats.Any())
            {
                logger.LogDebug("Entity types extracted from {DocumentPath}: {EntityStats}",
                    document.FilePath, string.Join(", ", entityStats.Select(kvp => $"{kvp.Key}({kvp.Value})")));
            }

            // Сохраняем в кэш только если есть сущности
            if (entities.Any())
            {
                entityCache.TryAdd(cacheKey, entities);
                logger.LogDebug("Cached extraction result for document: {DocumentPath} with options: {OptionsKey}",
                    document.FilePath, optionsKey);
            }

            return entities;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error extracting entities from document: {DocumentPath}", document.FilePath);
            throw;
        }
    }
}