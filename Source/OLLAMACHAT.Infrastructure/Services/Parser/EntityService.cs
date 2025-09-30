namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Services.Parser;

/// <inheritdoc />
public partial class EntityService(
    IMapperService mapperService,
    ILogger<EntityService> logger,
    IOptions<SolutionSettings> solutionSettings,
    ISolutionLoaderService solutionLoaderService) : IEntityService
{
    private static readonly Regex UnityEventRegex = new(@"UnityEvent<.*>", RegexOptions.Compiled);
    private static readonly string[] UnityEventNames = { "UnityEvent", "UnityEvent<T>", "UnityEvent<T0, T1>", "UnityEvent<T0, T1, T2>", "UnityEvent<T0, T1, T2, T3>" };

    // Кэш для повышения производительности при анализе больших кодовых баз
    private static readonly ConcurrentDictionary<string, List<ParsedEntity>> entityCache = new();

    private async Task<List<ParsedEntity>> ExtractChildEntitiesAsync(ISymbol symbol, SemanticModel semanticModel, int currentDepth, ParserOptions? options = null)
    {
        // Проверка на null символ
        if (symbol == null)
        {
            logger.LogWarning("Symbol is null in ExtractChildEntitiesAsync");
            return new List<ParsedEntity>();
        }

        // Проверка на null семантическую модель
        if (semanticModel == null)
        {
            logger.LogWarning("Semantic model is null in ExtractChildEntitiesAsync for symbol: {SymbolName}", symbol.Name);
            return new List<ParsedEntity>();
        }

        // Проверка на специальные символы в имени
        if (symbol.Name != null && symbol.Name.Any(c => !char.IsLetterOrDigit(c) && c != '_' && c != '`'))
        {
            logger.LogWarning("Symbol contains special characters: {SymbolName}", symbol.Name);
        }

        logger.LogInformation("Extracting child entities for symbol: {SymbolName}", symbol.Name);

        // Проверка на превышение максимальной глубины
        if (options?.MaxDepth != null && currentDepth >= options.MaxDepth.Value)
        {
            logger.LogWarning("Maximum depth ({MaxDepth}) reached for symbol: {SymbolName} at depth {CurrentDepth}",
                options.MaxDepth.Value, symbol.Name, currentDepth);
            return new List<ParsedEntity>();
        }

        try
        {
            // Оптимизация: предвычисляем примерное количество дочерних сущностей
            var estimatedChildCount = symbol is INamedTypeSymbol namedTypeSymbol ?
                namedTypeSymbol.GetMembers().Count(m => !m.IsImplicitlyDeclared || ShouldIncludeImplicitMember(m)) : 0;

            var childEntities = new List<ParsedEntity>(estimatedChildCount);
            var processedSymbols = new HashSet<string>(); // Защита от рекурсивных зависимостей
            var childEntityTasks = new List<Task<ParsedEntity>>();

            if (symbol is INamedTypeSymbol || symbol is INamespaceSymbol)
            {
                // Оптимизация: получаем все члены сразу и фильтруем их
                List<ISymbol> allMembers = symbol is INamedTypeSymbol typeSymbol
                    ? typeSymbol!.GetMembers().ToList()
                    : symbol is INamespaceSymbol namespaceSymbol
                        ? namespaceSymbol.GetMembers().OfType<ISymbol>().ToList()
                        : throw new NotImplementedException();

                // Extract all members recursively
                foreach (var member in allMembers)
                {
                    // Проверка на рекурсивную обработку
                    var memberKey = $"{member.Name}_{member.Kind}";
                    if (processedSymbols.Contains(memberKey))
                    {
                        logger.LogWarning("Detected potential recursive dependency for member: {MemberName}", member.Name);
                        continue;
                    }
                    processedSymbols.Add(memberKey);

                    // Skip inherited members that we don't want to duplicate, but keep important ones
                    if (member.IsOverride && !ShouldIncludeOverrideMember(member))
                        continue;

                    // Skip compiler-generated members
                    if (member.IsImplicitlyDeclared && !ShouldIncludeImplicitMember(member))
                        continue;

                    // Оптимизация: добавляем задачи вместо последовательного выполнения
                    // Extract methods (including constructors, destructors, operators)
                    if (member is IMethodSymbol)
                    {
                        childEntityTasks.Add(ExtractEntityAsync(member, semanticModel, currentDepth + 1, options));
                    }
                    // Extract properties
                    else if (member is IPropertySymbol)
                    {
                        childEntityTasks.Add(ExtractEntityAsync(member, semanticModel, currentDepth + 1, options));
                    }
                    // Extract fields
                    else if (member is IFieldSymbol)
                    {
                        childEntityTasks.Add(ExtractEntityAsync(member, semanticModel, currentDepth + 1, options));
                    }
                    // Extract events
                    else if (member is IEventSymbol)
                    {
                        childEntityTasks.Add(ExtractEntityAsync(member, semanticModel, currentDepth + 1, options));
                    }
                    // Extract nested types
                    else if (member is INamedTypeSymbol)
                    {
                        childEntityTasks.Add(ExtractEntityAsync(member, semanticModel, currentDepth + 1, options));
                    }
                }

                // Оптимизация: выполняем все задачи параллельно
                if (childEntityTasks.Count > 0)
                {
                    var childResults = await Task.WhenAll(childEntityTasks);
                    childEntities.AddRange(childResults);
                }
            }

            logger.LogInformation("Successfully extracted {ChildCount} child entities for symbol: {SymbolName}", childEntities.Count, symbol.Name);
            return childEntities;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error extracting child entities for symbol: {SymbolName}", symbol.Name);
            throw;
        }
    }

    /// <summary>
    /// Определяет, следует ли включать переопределенный член
    /// </summary>
    private static bool ShouldIncludeOverrideMember(ISymbol member)
    {
        return IsImportantOverrideMethod(member);
    }

    /// <summary>
    /// Определяет, следует ли включать неявно объявленный член
    /// </summary>
    private static bool ShouldIncludeImplicitMember(ISymbol member)
    {
        // Включаем важные неявно объявленные члены
        if (member is IMethodSymbol methodSymbol)
        {
            // Включаем конструкторы по умолчанию для записей
            if (methodSymbol.MethodKind == MethodKind.Constructor &&
                methodSymbol.ContainingType.IsRecord)
                return true;
        }

        return false;
    }

    private ParsedEntityType DetermineEntityType(ISymbol symbol) =>
        symbol switch
        {
            INamespaceSymbol => ParsedEntityType.Namespace,
            INamedTypeSymbol namedTypeSymbol => namedTypeSymbol.TypeKind switch
            {
                TypeKind.Class => ParsedEntityType.Class,
                TypeKind.Interface => ParsedEntityType.Interface,
                TypeKind.Enum => ParsedEntityType.Enum,
                TypeKind.Struct => ParsedEntityType.Struct,
                // TypeKind.Record => ParsedEntityType.Class, // Records are treated as classes
                // TypeKind.RecordStruct => ParsedEntityType.Struct, // Record structs are treated as structs
                TypeKind.Delegate => ParsedEntityType.Class, // Delegates are treated as classes
                _ => ParsedEntityType.Class
            },
            IMethodSymbol methodSymbol => methodSymbol.MethodKind switch
            {
                // MethodKind.Constructor => ParsedEntityType.Constructor,
                // MethodKind.Destructor => ParsedEntityType.Destructor,
                // MethodKind.Operator => ParsedEntityType.Operator,
                // MethodKind.Conversion => ParsedEntityType.Operator,
                MethodKind.Ordinary => ParsedEntityType.Method,
                // MethodKind.StaticConstructor => ParsedEntityType.Constructor,
                MethodKind.LocalFunction => ParsedEntityType.Method,
                _ => ParsedEntityType.Method
            },
            IPropertySymbol => ParsedEntityType.Property,
            IFieldSymbol fieldSymbol => fieldSymbol.ContainingType is INamedTypeSymbol { TypeKind: TypeKind.Enum }
                ? ParsedEntityType.Enum
                : ParsedEntityType.Property, // Enum members are treated as fields
            IEventSymbol => ParsedEntityType.UnityEvent, // Using existing enum value
            _ => ParsedEntityType.Class
        };

    private bool IsUnityEventField(ISymbol symbol)
    {
        if (symbol is not IFieldSymbol fieldSymbol)
            return false;

        var fieldType = fieldSymbol.Type.ToDisplayString();

        // Check for exact UnityEvent type names
        if (UnityEventNames.Contains(fieldType))
            return true;

        // Check for UnityEvent with generic parameters using regex
        if (UnityEventRegex.IsMatch(fieldType))
            return true;

        // Check if the type inherits from UnityEvent
        if (fieldSymbol.Type.AllInterfaces.Any(i => i.Name.Contains("UnityEvent")))
            return true;

        return false;
    }
}