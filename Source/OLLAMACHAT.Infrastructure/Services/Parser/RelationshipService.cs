namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Services.Parser;

[UsedImplicitly]
public class RelationshipService(
    ILogger<RelationshipService> logger,
    ISolutionLoaderService solutionLoaderService) : IRelationshipService
{
    private static ConcurrentDictionary<string, INamedTypeSymbol>? allTypesCache;
    private static ConcurrentDictionary<string, string>? typeToFilePathCache;

    /// <inheritdoc />
    public async Task<IEnumerable<Relationship>> AnalyzeRelationshipsAsync(IEnumerable<ParsedEntity> entities, Document document)
    {
        logger.LogInformation("Analyzing relationships for document: {DocumentPath}", document.FilePath);

        try
        {
            var relationships = new List<Relationship>();
            var semanticModel = await document.GetSemanticModelAsync();

            if (semanticModel == null)
            {
                logger.LogWarning("Could not get semantic model for document: {DocumentPath}", document.FilePath);
                return relationships;
            }

            var syntaxTree = await document.GetSyntaxTreeAsync();
            if (syntaxTree == null)
            {
                logger.LogWarning("Could not get syntax tree for document: {DocumentPath}", document.FilePath);
                return relationships;
            }

            var root = await syntaxTree.GetRootAsync();

            var allEntities = FlattenAllTrees(entities).ToList();

            foreach (var entity in allEntities)
            {
                if (entity.Type == ParsedEntityType.Method)
                {
                    AnalyzeMethodCallRelationships(entity, semanticModel, root, relationships);
                }
                else if (entity.Type == ParsedEntityType.Class ||
                         entity.Type == ParsedEntityType.Interface ||
                         entity.Type == ParsedEntityType.Struct)
                {
                    AnalyzeInheritanceRelationships(entity, relationships);
                }
            }

            logger.LogInformation("Successfully analyzed {RelationshipCount} relationships for document: {DocumentPath}",
                relationships.Count, document.FilePath);
            return relationships;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error analyzing relationships for document: {DocumentPath}", document.FilePath);
            throw;
        }
    }

    /// <summary>
    /// Инициализирует кеши типов и путей к файлам для оптимизации поиска
    /// </summary>
    public static async Task InitializeCaches(Solution solution)
    {
        allTypesCache = new();
        typeToFilePathCache = new();

        foreach (var project in solution.Projects)
        {
            var compilation = await project.GetCompilationAsync();
            if (compilation == null)
                continue;

            var types = GetAllTypesInCompilation(compilation);
            foreach (var type in types)
            {
                var fullName = type.ToDisplayString();
                if (!allTypesCache.ContainsKey(fullName))
                {
                    allTypesCache[fullName] = type;

                    // Кешируем путь к файлу
                    var filePath = GetFilePathForTypeInternal(type, solution);
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        typeToFilePathCache[fullName] = filePath;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Рекурсивно обходит все дочерние сущности и возвращает плоский список всех сущностей
    /// </summary>
    /// <param name="roots">Корневые сущности</param>
    /// <returns>Плоский список всех сущностей, включая дочерние</returns>
    private static IEnumerable<ParsedEntity> FlattenAllTrees(IEnumerable<ParsedEntity> roots)
    {
        foreach (var root in roots)
        {
            yield return root;

            if (root.Children != null && root.Children.Any())
            {
                foreach (var child in FlattenAllTrees(root.Children))
                {
                    yield return child;
                }
            }
        }
    }

    /// <summary>
    /// Анализирует вызовы методов внутри метода
    /// </summary>
    private void AnalyzeMethodCallRelationships(
        ParsedEntity methodEntity,
        SemanticModel semanticModel,
        SyntaxNode root,
        List<Relationship> relationships)
    {
        // Ищем декларацию метода
        var methodDeclaration = FindMethodDeclaration(root, methodEntity);
        if (methodDeclaration == null)
            return;

        // Логирование для отладки проблемы с вложенными классами
        logger.LogDebug("Analyzing method: {MethodFullName}, Found declaration at line: {LineNumber}",
            methodEntity.FullName, methodDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line + 1);

        // Проверяем, является ли метод частью вложенного класса
        if (methodEntity.FullName?.Contains('.') == true && methodEntity.FullName.Split('.').Length > 2)
        {
            logger.LogDebug("Processing nested class method: {MethodFullName}", methodEntity.FullName);
        }

        // Анализируем все вызовы методов
        var invocationExpressions = methodDeclaration.DescendantNodes().OfType<InvocationExpressionSyntax>();

        foreach (var invocation in invocationExpressions)
        {
            ProcessMethodInvocation(invocation, methodEntity, semanticModel, relationships);
        }

        // Анализируем обращения к свойствам и полям
        var memberAccessExpressions = methodDeclaration.DescendantNodes()
            .OfType<MemberAccessExpressionSyntax>()
            .Where(ma => ma.Parent is not InvocationExpressionSyntax); // Исключаем уже обработанные вызовы методов

        foreach (var memberAccess in memberAccessExpressions)
        {
            ProcessMemberAccess(memberAccess, methodEntity, semanticModel, relationships);
        }
    }

    /// <summary>
    /// Находит синтаксическую декларацию метода
    /// </summary>
    private CSharpSyntaxNode? FindMethodDeclaration(SyntaxNode root, ParsedEntity methodEntity)
    {
        // Логирование для отладки проблемы с вложенными классами
        logger.LogDebug("Looking for method declaration: {MethodFullName}, SimpleName: {SimpleName}",
            methodEntity.FullName, methodEntity.SimpleName);

        // Проверяем, является ли метод частью вложенного класса
        var isNestedClassMethod = methodEntity.FullName?.Contains('.') == true &&
                                  methodEntity.FullName.Split('.').Length > 2;

        if (isNestedClassMethod)
        {
            // Для вложенных классов ищем декларацию с учетом иерархии
            return FindNestedClassMethodDeclaration(root, methodEntity);
        }

        // Поддержка обычных методов
        var methodDeclaration = root.DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .FirstOrDefault(m => IsMatchingMethod(m.Identifier.Text, methodEntity));

        if (methodDeclaration != null)
        {
            logger.LogDebug("Found method declaration: {MethodName} at line {LineNumber}",
                methodDeclaration.Identifier.Text,
                methodDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line + 1);
            return methodDeclaration;
        }

        // Поддержка конструкторов
        var constructorDeclaration = root.DescendantNodes()
            .OfType<ConstructorDeclarationSyntax>()
            .FirstOrDefault(c => IsMatchingMethod(c.Identifier.Text, methodEntity) ||
                                 methodEntity.SimpleName == ".ctor");

        if (constructorDeclaration != null)
        {
            logger.LogDebug("Found constructor declaration: {ConstructorName} at line {LineNumber}",
                constructorDeclaration.Identifier.Text,
                constructorDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line + 1);
            return constructorDeclaration;
        }

        // Поддержка акцессоров свойств
        var accessorDeclaration = root.DescendantNodes()
            .OfType<AccessorDeclarationSyntax>()
            .FirstOrDefault(a => IsMatchingAccessor(a, methodEntity));

        if (accessorDeclaration != null)
        {
            logger.LogDebug("Found accessor declaration: {AccessorKind} at line {LineNumber}",
                accessorDeclaration.Keyword.Text,
                accessorDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line + 1);
        }
        else
        {
            logger.LogWarning("Method declaration not found for: {MethodFullName}", methodEntity.FullName);
        }

        return accessorDeclaration;
    }

    /// <summary>
    /// Находит синтаксическую декларацию метода для вложенных классов с учетом иерархии
    /// </summary>
    private CSharpSyntaxNode? FindNestedClassMethodDeclaration(SyntaxNode root, ParsedEntity methodEntity)
    {
        if (methodEntity.FullName == null)
            return null;

        // Разбираем полное имя на компоненты
        var nameParts = methodEntity.FullName.Split('.');
        if (nameParts.Length < 2)
            return null;

        // Последний компонент - имя метода
        var methodName = nameParts[^1];
        // Предпоследний компонент - имя класса (может содержать параметры)
        var classNameWithParams = nameParts[^2];
        // Все остальное - иерархия вложенных классов
        var classHierarchy = nameParts[..^2].ToList();

        // Извлекаем имя класса без параметров
        var className = classNameWithParams.Split('(')[0];
        classHierarchy.Add(className);

        logger.LogDebug("Looking for nested class method: {MethodName} in class hierarchy: {ClassHierarchy}",
            methodName, string.Join(".", classHierarchy));

        // Ищем классы в иерархии
        var currentNodes = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
        ClassDeclarationSyntax? targetClass = null;

        foreach (var classInHierarchy in classHierarchy)
        {
            var foundClasses = currentNodes.Where(c => c.Identifier.Text == classInHierarchy).ToList();
            if (!foundClasses.Any())
            {
                logger.LogWarning("Class {ClassName} not found in hierarchy", classInHierarchy);
                return null;
            }

            if (foundClasses.Count > 1)
            {
                logger.LogWarning("Multiple classes {ClassName} found, using first one", classInHierarchy);
            }

            targetClass = foundClasses.First();
            // Для следующего уровня иерархии ищем вложенные классы
            currentNodes = targetClass.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
        }

        if (targetClass == null)
        {
            logger.LogWarning("Target class not found for method: {MethodFullName}", methodEntity.FullName);
            return null;
        }

        // Ищем метод внутри найденного класса
        if (methodEntity.SimpleName == ".ctor")
        {
            // Ищем конструктор
            var constructor = targetClass.DescendantNodes()
                .OfType<ConstructorDeclarationSyntax>()
                .FirstOrDefault(c => c.Identifier.Text == targetClass.Identifier.Text);

            if (constructor != null)
            {
                logger.LogDebug("Found nested constructor declaration: {ConstructorName} at line {LineNumber}",
                    constructor.Identifier.Text,
                    constructor.GetLocation().GetLineSpan().StartLinePosition.Line + 1);
                return constructor;
            }
        }
        else if (methodEntity.SimpleName.StartsWith("get_") || methodEntity.SimpleName.StartsWith("set_"))
        {
            // Ищем акцессор свойства
            var accessor = targetClass.DescendantNodes()
                .OfType<AccessorDeclarationSyntax>()
                .FirstOrDefault(a => IsMatchingAccessor(a, methodEntity));

            if (accessor != null)
            {
                logger.LogDebug("Found nested accessor declaration: {AccessorKind} at line {LineNumber}",
                    accessor.Keyword.Text,
                    accessor.GetLocation().GetLineSpan().StartLinePosition.Line + 1);
                return accessor;
            }
        }
        else
        {
            // Ищем обычный метод
            var method = targetClass.DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(m => IsMatchingMethod(m.Identifier.Text, methodEntity));

            if (method != null)
            {
                logger.LogDebug("Found nested method declaration: {MethodName} at line {LineNumber}",
                    method.Identifier.Text,
                    method.GetLocation().GetLineSpan().StartLinePosition.Line + 1);
                return method;
            }
        }

        logger.LogWarning("Nested method declaration not found for: {MethodFullName}", methodEntity.FullName);
        return null;
    }

    /// <summary>
    /// Проверяет соответствие имени метода сущности
    /// </summary>
    private bool IsMatchingMethod(string methodName, ParsedEntity methodEntity)
    {
        var isMatch = methodName == methodEntity.SimpleName ||
                      methodEntity.FullName?.EndsWith($".{methodName}") == true ||
                      methodEntity.FullName?.EndsWith($".{methodName}(") == true;

        // Логирование для отладки проблемы с вложенными классами
        if (methodEntity.FullName?.Contains('.') == true && methodEntity.FullName.Split('.').Length > 2)
        {
            logger.LogDebug("Matching nested class method: MethodName={MethodName}, SimpleName={SimpleName}, FullName={FullName}, IsMatch={IsMatch}",
                methodName, methodEntity.SimpleName, methodEntity.FullName, isMatch);
        }

        return isMatch;
    }

    /// <summary>
    /// Проверяет соответствие акцессора сущности
    /// </summary>
    private bool IsMatchingAccessor(AccessorDeclarationSyntax accessor, ParsedEntity methodEntity)
    {
        var accessorKind = accessor.Keyword.Text; // "get" или "set"
        var propertyName = (accessor.Parent?.Parent as PropertyDeclarationSyntax)?.Identifier.Text;

        if (propertyName == null)
            return false;

        var expectedName = $"{accessorKind}_{propertyName}";
        return methodEntity.SimpleName == expectedName ||
               methodEntity.FullName?.Contains($".{expectedName}") == true;
    }

    /// <summary>
    /// Обрабатывает вызов метода
    /// </summary>
    private void ProcessMethodInvocation(
        InvocationExpressionSyntax invocation,
        ParsedEntity methodEntity,
        SemanticModel semanticModel,
        List<Relationship> relationships)
    {
        var symbolInfo = semanticModel.GetSymbolInfo(invocation.Expression);
        var methodSymbols = new List<IMethodSymbol>();

        if (symbolInfo.Symbol is IMethodSymbol directSymbol)
        {
            methodSymbols.Add(directSymbol);
        }
        else if (symbolInfo.CandidateSymbols.Any())
        {
            methodSymbols.AddRange(symbolInfo.CandidateSymbols.OfType<IMethodSymbol>());
        }

        foreach (var methodSymbol in methodSymbols)
        {
            var containingType = methodSymbol.ContainingType;
            if (containingType == null)
                continue;

            var containingTypeFullName = containingType.ToDisplayString();
            var fullCalledMethodName = $"{containingTypeFullName}.{methodSymbol.Name}";

            // Логирование для отладки проблемы с вложенными классами
            logger.LogDebug("Processing method call from {FromMethod} to {ToMethod}",
                methodEntity.FullName, fullCalledMethodName);

            // Проверяем, является ли метод частью вложенного класса
            if (methodEntity.FullName?.Contains('.') == true && methodEntity.FullName.Split('.').Length > 2)
            {
                logger.LogDebug("Nested class method {FromMethod} is calling {ToMethod}",
                    methodEntity.FullName, fullCalledMethodName);

                // Дополнительное логирование для отслеживания проблемы
                var invocationText = invocation.ToString();
                if (invocationText.Length > 100)
                    invocationText = invocationText.Substring(0, 100) + "...";

                logger.LogDebug("Invocation expression: {InvocationText}", invocationText);
            }

            var targetFilePath = GetCachedFilePathForType(containingType);

            relationships.Add(new Relationship(
                FullNameFrom: methodEntity.FullName,
                FullNameTo: fullCalledMethodName,
                Type: RelationshipType.Calls,
                TargetDefinitionFilePath: targetFilePath
            ));
        }
    }

    /// <summary>
    /// Обрабатывает обращение к члену типа (свойству или полю)
    /// </summary>
    private void ProcessMemberAccess(
        MemberAccessExpressionSyntax memberAccess,
        ParsedEntity methodEntity,
        SemanticModel semanticModel,
        List<Relationship> relationships)
    {
        var symbolInfo = semanticModel.GetSymbolInfo(memberAccess);
        var symbol = symbolInfo.Symbol;

        if (symbol == null)
            return;

        // Обрабатываем только свойства и поля
        if (symbol is not (IPropertySymbol or IFieldSymbol))
            return;

        var containingType = symbol.ContainingType;
        if (containingType == null)
            return;

        var containingTypeFullName = containingType.ToDisplayString();
        var fullMemberName = $"{containingTypeFullName}.{symbol.Name}";

        var targetFilePath = GetCachedFilePathForType(containingType);

        relationships.Add(new Relationship(
            FullNameFrom: methodEntity.FullName,
            FullNameTo: fullMemberName,
            Type: RelationshipType.Calls,
            TargetDefinitionFilePath: targetFilePath
        ));
    }

    /// <summary>
    /// Получает путь к файлу из кеша или возвращает null
    /// </summary>
    private string? GetCachedFilePathForType(INamedTypeSymbol typeSymbol)
    {
        if (typeSymbol == null)
            return null;

        var fullName = typeSymbol.ToDisplayString();

        // Проверяем, является ли тип пользовательским
        if (typeToFilePathCache != null && typeToFilePathCache.TryGetValue(fullName, out var cachedPath))
        {
            return cachedPath;
        }

        return null;
    }

    /// <summary>
    /// Получает путь к файлу, в котором определен тип (внутренний метод для кеширования)
    /// </summary>
    private static string? GetFilePathForTypeInternal(INamedTypeSymbol typeSymbol, Solution solution)
    {
        if (typeSymbol == null)
            return null;

        var typeDeclarations = typeSymbol.DeclaringSyntaxReferences;
        if (typeDeclarations.Length == 0)
            return null;

        var syntaxNode = typeDeclarations[0].GetSyntax();
        var syntaxTree = syntaxNode.SyntaxTree;
        var filePath = syntaxTree.FilePath;

        if (string.IsNullOrEmpty(filePath))
            return null;

        // Возвращаем абсолютный путь
        return Path.GetFullPath(filePath);
    }

    /// <summary>
    /// Анализирует отношения наследования для сущности (класса, интерфейса или структуры)
    /// </summary>
    private void AnalyzeInheritanceRelationships(ParsedEntity entity, List<Relationship> relationships)
    {
        // Проверяем, что у сущности есть информация о наследовании
        if (entity.Inheritance == null)
            return;

        if (!solutionLoaderService.IsSolutionLoaded || solutionLoaderService.CurrentSolution == null)
            return;

        // Получаем символ текущей сущности из кеша
        if (allTypesCache == null || !allTypesCache.TryGetValue(entity.FullName!, out var currentEntitySymbol))
            return;

        // Ищем все типы, которые наследуют от текущей сущности
        foreach (var kvp in allTypesCache)
        {
            var typeSymbol = kvp.Value;
            var typeFullName = kvp.Key;

            // Пропускаем саму сущность
            if (SymbolEqualityComparer.Default.Equals(typeSymbol, currentEntitySymbol))
                continue;

            if (InheritsFrom(typeSymbol, currentEntitySymbol))
            {
                // Получаем путь к файлу из кеша
                var targetFilePath = typeToFilePathCache?.TryGetValue(typeFullName, out var path) == true
                    ? path
                    : null;

                relationships.Add(new Relationship(
                    FullNameFrom: entity.FullName,
                    FullNameTo: typeFullName,
                    Type: RelationshipType.IsBaseFor,
                    TargetDefinitionFilePath: targetFilePath
                ));
            }
        }
    }

    /// <summary>
    /// Получает все типы в компиляции
    /// </summary>
    private static IEnumerable<INamedTypeSymbol> GetAllTypesInCompilation(Compilation compilation)
    {
        var types = new List<INamedTypeSymbol>();
        GetAllTypesInNamespace(compilation.GlobalNamespace, types);
        return types;
    }

    /// <summary>
    /// Рекурсивно получает все типы в пространстве имен и его дочерних пространствах
    /// </summary>
    private static void GetAllTypesInNamespace(INamespaceSymbol namespaceSymbol, List<INamedTypeSymbol> types)
    {
        // Добавляем типы текущего пространства имен
        foreach (var typeMember in namespaceSymbol.GetTypeMembers())
        {
            types.Add(typeMember);

            // Рекурсивно добавляем вложенные типы
            AddNestedTypes(typeMember, types);
        }

        // Рекурсивно обрабатываем дочерние пространства имен
        foreach (var childNamespace in namespaceSymbol.GetNamespaceMembers())
        {
            GetAllTypesInNamespace(childNamespace, types);
        }
    }

    /// <summary>
    /// Рекурсивно добавляет вложенные типы
    /// </summary>
    private static void AddNestedTypes(INamedTypeSymbol typeSymbol, List<INamedTypeSymbol> types)
    {
        foreach (var nestedType in typeSymbol.GetTypeMembers())
        {
            types.Add(nestedType);
            AddNestedTypes(nestedType, types);
        }
    }

    /// <summary>
    /// Проверяет, наследует ли тип от указанного базового типа (включая косвенное наследование)
    /// </summary>
    private bool InheritsFrom(INamedTypeSymbol typeSymbol, INamedTypeSymbol baseTypeSymbol)
    {
        if (typeSymbol == null || baseTypeSymbol == null)
            return false;

        // Проверяем прямое и косвенное наследование от базового класса
        var baseType = typeSymbol.BaseType;
        while (baseType != null)
        {
            if (SymbolEqualityComparer.Default.Equals(baseType, baseTypeSymbol))
                return true;
            baseType = baseType.BaseType;
        }

        // Проверяем реализацию интерфейсов (прямую и косвенную)
        foreach (var interfaceType in typeSymbol.AllInterfaces)
        {
            if (SymbolEqualityComparer.Default.Equals(interfaceType, baseTypeSymbol))
                return true;
        }

        return false;
    }
}
