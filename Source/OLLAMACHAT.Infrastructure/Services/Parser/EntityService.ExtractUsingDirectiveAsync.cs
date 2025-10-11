using Attribute = VelikiyPrikalel.OLLAMACHAT.Application.Models.Attribute;
using Location = VelikiyPrikalel.OLLAMACHAT.Application.Models.Location;

namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Services.Parser;

/// <summary>
/// Часть класса EntityService, содержащая метод ExtractUsingDirectiveAsync
/// </summary>
public partial class EntityService
{
    private async Task<ParsedEntity?> ExtractUsingDirectiveAsync(UsingDirectiveSyntax usingDirective, SemanticModel semanticModel, bool extractUsingData)
    {
        try
        {
            string name = usingDirective.Name?.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(name))
            {
                logger.LogWarning("Using directive name is null or empty");
                return null;
            }

            // Проверка на некорректные символы в имени
            if (ContainsSpecialCharacters(name))
            {
                logger.LogWarning("Using directive contains special characters: {Name}", name);
            }

            Location location = mapperService.MapLocation(usingDirective.GetLocation());
            string? alias = usingDirective.Alias?.Name.ToString();

            // Extract additional information about the using directive
            List<Attribute> attributes = new();
            List<string> modifiers = new();

            // Check if it's a global using
            if (!usingDirective.GlobalKeyword.IsKind(SyntaxKind.None))
            {
                modifiers.Add("global");
            }

            // Check if it's a static using
            if (!usingDirective.StaticKeyword.IsKind(SyntaxKind.None))
            {
                modifiers.Add("static");
            }

            // Определяем реальный путь к файлам namespace
            string? namespacePath = extractUsingData ? await ResolveNamespacePathAsync(name) : null;

            ParsedEntity entity = new(
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

            Solution? solution = solutionLoaderService.CurrentSolution;
            string solutionFilePath = solutionSettings.Value.SolutionFilePath;
            string? solutionDirectory = Path.GetDirectoryName(solutionFilePath);

            if (string.IsNullOrEmpty(solutionDirectory))
            {
                logger.LogWarning("Solution directory is null, returning namespace name as is: {NamespaceName}", namespaceName);
                return null;
            }

            // Ищем все документы в solution, которые принадлежат к указанному namespace
            List<Document> namespaceDocuments = new();

            foreach (Project project in solution.Projects)
            {
                foreach (Document document in project.Documents)
                {
                    if (document.FilePath != null && document.FilePath.EndsWith(".cs"))
                    {
                        // Получаем семантическую модель для документа
                        SemanticModel? semanticModel = await document.GetSemanticModelAsync();
                        if (semanticModel != null)
                        {
                            // Получаем корневой узел синтаксического дерева
                            SyntaxNode? root = await semanticModel.SyntaxTree.GetRootAsync();
                            if (root != null)
                            {
                                // Ищем объявления namespace в документе
                                IEnumerable<BaseNamespaceDeclarationSyntax> namespaceDeclarations = root.DescendantNodes().OfType<BaseNamespaceDeclarationSyntax>();

                                foreach (BaseNamespaceDeclarationSyntax namespaceDeclaration in namespaceDeclarations)
                                {
                                    string? documentNamespace = semanticModel.GetDeclaredSymbol(namespaceDeclaration)?.GetFullName();
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
                string firstDocumentPath = namespaceDocuments.First().FilePath!;
                string? documentDirectory = Path.GetDirectoryName(firstDocumentPath);

                if (!string.IsNullOrEmpty(documentDirectory))
                {
                    // Делаем путь относительным относительно директории solution
                    string relativePath = Path.GetRelativePath(solutionDirectory, documentDirectory);

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
