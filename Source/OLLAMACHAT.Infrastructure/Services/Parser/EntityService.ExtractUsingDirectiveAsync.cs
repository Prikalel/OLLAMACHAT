using Attribute = VelikiyPrikalel.OLLAMACHAT.Application.Models.Attribute;

namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Services.Parser;

/// <summary>
/// Часть класса EntityService, содержащая метод ExtractUsingDirectiveAsync
/// </summary>
public partial class EntityService
{
    private async Task<ParsedEntity?> ExtractUsingDirectiveAsync(UsingDirectiveSyntax usingDirective, SemanticModel semanticModel, bool extractUsingData)
    {
        // Проверка на null директиву
        if (usingDirective == null)
        {
            logger.LogWarning("Using directive is null");
            return null;
        }

        // Проверка на null семантическую модель
        if (semanticModel == null)
        {
            logger.LogWarning("Semantic model is null for using directive");
            return null;
        }

        try
        {
            var name = usingDirective.Name?.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(name))
            {
                logger.LogWarning("Using directive name is null or empty");
                return null;
            }

            // Проверка на очень длинные имена
            if (name.Length > MaxUsingNameLength)
            {
                logger.LogWarning("Very long using directive name ({Length} characters): {Name}", name.Length, name);
            }

            // Проверка на некорректные символы в имени
            if (ContainsSpecialCharacters(name))
            {
                logger.LogWarning("Using directive contains special characters: {Name}", name);
            }

            var location = mapperService.MapLocation(usingDirective.GetLocation());
            var alias = usingDirective.Alias?.Name.ToString();

            // Extract additional information about the using directive
            var attributes = new List<Attribute>();
            var modifiers = new List<string>();

            // Check if it's a global using
            if (usingDirective.GlobalKeyword.Kind() != SyntaxKind.None)
            {
                modifiers.Add("global");
                attributes.Add(new Attribute("UsingType", new List<string> { "Global" }));
            }
            else
            {
                attributes.Add(new Attribute("UsingType", new List<string> { "Local" }));
            }

            // Check if it's a static using
            if (usingDirective.StaticKeyword.Kind() != SyntaxKind.None)
            {
                modifiers.Add("static");
                attributes.Add(new Attribute("UsingType", new List<string> { "Static" }));
            }

            // Extract alias information
            if (!string.IsNullOrEmpty(alias))
            {
                attributes.Add(new Attribute("Alias", new List<string> { alias }));
            }

            // Try to get symbol information for more details
            if (usingDirective.Name != null)
            {
                var symbolInfo = semanticModel.GetSymbolInfo(usingDirective.Name);
                if (symbolInfo.Symbol != null)
                {
                    attributes.Add(new Attribute("TargetSymbol", new List<string> { symbolInfo.Symbol.ToDisplayString() }));
                }
            }

            // Определяем реальный путь к файлам namespace
            var namespacePath = extractUsingData ? await ResolveNamespacePathAsync(name) : null;

            var entity = new ParsedEntity(
                SimpleName: name,
                FullName: name,
                Type: ParsedEntityType.UsingStatement,
                Location: location,
                Children: null,
                Modifiers: modifiers.Any() ? modifiers : null,
                Attributes: attributes.Any() ? attributes : null,
                Inheritance: null,
                ReturnType: null,
                Parameters: null,
                UsingStatementData: extractUsingData && namespacePath != null ? new UsingStatementData(namespacePath) : null
            );

            logger.LogDebug("Extracted using directive: {Name} with modifiers: {Modifiers}, resolved path: {Path}", name, string.Join(", ", modifiers), namespacePath);
            return entity;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error extracting using directive");
            return null;
        }
    }

    /// <summary>
    /// Определяет реальный путь к файлам для указанного namespace
    /// </summary>
    /// <param name="namespaceName">Имя namespace</param>
    /// <returns>Путь к директории с файлами namespace или исходное имя namespace, если путь не удалось определить</returns>
    private async Task<string?> ResolveNamespacePathAsync(string namespaceName)
    {
        try
        {
            if (string.IsNullOrEmpty(namespaceName))
            {
                return null;
            }

            // Проверяем, что solution загружен
            if (!solutionLoaderService.IsSolutionLoaded || solutionLoaderService.CurrentSolution == null)
            {
                logger.LogWarning("Solution is not loaded, returning namespace name as is: {NamespaceName}", namespaceName);
                return null;
            }

            var solution = solutionLoaderService.CurrentSolution;
            var solutionFilePath = solutionSettings.Value.SolutionFilePath;
            var solutionDirectory = Path.GetDirectoryName(solutionFilePath);

            if (string.IsNullOrEmpty(solutionDirectory))
            {
                logger.LogWarning("Solution directory is null, returning namespace name as is: {NamespaceName}", namespaceName);
                return null;
            }

            // Ищем все документы в solution, которые принадлежат к указанному namespace
            var namespaceDocuments = new List<Document>();

            foreach (var project in solution.Projects)
            {
                foreach (var document in project.Documents)
                {
                    if (document.FilePath != null && document.FilePath.EndsWith(".cs"))
                    {
                        // Получаем семантическую модель для документа
                        var semanticModel = await document.GetSemanticModelAsync();
                        if (semanticModel != null)
                        {
                            // Получаем корневой узел синтаксического дерева
                            var root = await semanticModel.SyntaxTree.GetRootAsync();
                            if (root != null)
                            {
                                // Ищем объявления namespace в документе
                                var namespaceDeclarations = root.DescendantNodes().OfType<BaseNamespaceDeclarationSyntax>();

                                foreach (var namespaceDeclaration in namespaceDeclarations)
                                {
                                    var documentNamespace = semanticModel.GetDeclaredSymbol(namespaceDeclaration)?.ToDisplayString();
                                    if (documentNamespace == namespaceName)
                                    {
                                        namespaceDocuments.Add(document);
                                        break; // Нашли нужный namespace в этом документе
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (namespaceDocuments.Any())
            {
                // Берем путь к первому найденному документу
                var firstDocumentPath = namespaceDocuments.First().FilePath!;
                var documentDirectory = Path.GetDirectoryName(firstDocumentPath);

                if (!string.IsNullOrEmpty(documentDirectory))
                {
                    // Делаем путь относительным относительно директории solution
                    var relativePath = Path.GetRelativePath(solutionDirectory, documentDirectory);

                    // Убедимся, что путь заканчивается на разделитель директории
                    if (!relativePath.EndsWith(Path.DirectorySeparatorChar.ToString()) &&
                        !relativePath.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
                    {
                        relativePath += Path.DirectorySeparatorChar;
                    }

                    logger.LogDebug("Resolved namespace '{NamespaceName}' to path: {RelativePath}", namespaceName, relativePath);
                    return relativePath;
                }
            }

            logger.LogWarning("Could not resolve path for namespace: {NamespaceName}", namespaceName);
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error resolving namespace path for: {NamespaceName}", namespaceName);
            return null;
        }
    }
}