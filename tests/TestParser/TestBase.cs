using System.Reflection;
using Microsoft.Extensions.Options;

namespace TestParser;

/// <summary>
/// Базовый класс для тестов парсера сущностей
/// </summary>
public abstract class TestBase
{
    [ModuleInitializer]
    public static void Init() => VerifyNewtonsoftJson.Enable();

    protected FakeLogger<EntityService> EntityLogger { get; private set; } = null!;
    protected FakeLogger<MapperService> MapperLogger { get; private set; } = null!;
    protected FakeLogger<SolutionLoaderService> SolutionLoaderLogger { get; private set; } = null!;

    protected MapperService MapperService { get; private set; } = null!;
    protected EntityService EntityService { get; private set; } = null!;
    protected SolutionLoaderService SolutionLoaderService { get; private set; } = null!;

    /// <summary>
    /// Настройка тестового окружения перед каждым тестом
    /// </summary>
    public virtual void Setup()
    {
        // Создаем фейковые логгеры для тестов
        EntityLogger = new FakeLogger<EntityService>();
        MapperLogger = new FakeLogger<MapperService>();
        SolutionLoaderLogger = new FakeLogger<SolutionLoaderService>();

        // Создаем сервисы
        MapperService = new MapperService(MapperLogger);
        IOptions<SolutionSettings> solutionSettingsOptions = Microsoft.Extensions.Options.Options.Create(
            new SolutionSettings { SolutionFilePath = Directory.GetCurrentDirectory() });
        SolutionLoaderService = new SolutionLoaderService(solutionSettingsOptions, SolutionLoaderLogger);

        EntityService = new EntityService(
            MapperService,
            EntityLogger,
            solutionSettingsOptions,
            SolutionLoaderService
        );
    }

    /// <summary>
    /// Создает тестовый документ с указанным содержимым
    /// </summary>
    /// <param name="content">Содержимое C# файла</param>
    /// <param name="fileName">Имя файла (по умолчанию "TestFile.cs")</param>
    /// <returns>Документ для анализа</returns>
    protected async Task<Document> CreateTestDocumentAsync(string content, string fileName = "TestFile.cs")
    {
        // Создаем рабочее пространство
        AdhocWorkspace workspace = new AdhocWorkspace();
        Project? project = workspace.AddProject("TestProject", LanguageNames.CSharp);

        // Добавляем необходимые метаданные для корректной работы семантической модели
        IEnumerable<MetadataReference> references = GetMetadataReferences();
        project = project.AddMetadataReferences(references);

        // Создаем исходный текст
        SourceText sourceText = SourceText.From(content);
        DocumentId documentId = DocumentId.CreateNewId(project.Id);
        DocumentInfo documentInfo = DocumentInfo.Create(
            documentId,
            fileName,
            Array.Empty<string>(),
            SourceCodeKind.Regular,
            TextLoader.From(TextAndVersion.Create(sourceText, VersionStamp.Default, fileName)),
            fileName,
            false);

        Document? document = workspace.AddDocument(documentInfo);
        return document;
    }

    /// <summary>
    /// Получает стандартные метаданные для компиляции C#
    /// </summary>
    /// <returns>Список метаданных</returns>
    private static IEnumerable<MetadataReference> GetMetadataReferences()
    {
        List<MetadataReference> references = new()
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location)
        };

        // Добавляем ссылки на базовые сборки .NET
        Assembly coreAssembly = typeof(System.Runtime.GCSettings).Assembly;
        if (!references.Any(r => r.Display == coreAssembly.Location))
        {
            references.Add(MetadataReference.CreateFromFile(coreAssembly.Location));
        }

        return references;
    }

    /// <summary>
    /// Создает стандартные опции парсера для тестов
    /// </summary>
    /// <param name="extractFullInheritance">Извлекать полное наследование</param>
    /// <param name="maxDepth">Максимальная глубина</param>
    /// <param name="extractUsingStatements">Извлекать using директивы</param>
    /// <returns>Опции парсера</returns>
    protected ParserOptions CreateParserOptions(
        bool extractFullInheritance = true,
        int? maxDepth = null,
        bool extractUsingStatements = true)
    {
        return new ParserOptions(
            ExtractFullExtractInheritance: extractFullInheritance,
            MaxDepth: maxDepth,
            ExtractUsingStatementData: extractUsingStatements
        );
    }

    /// <summary>
    /// Проверяет, что логгер записал сообщения об ошибках
    /// </summary>
    /// <param name="logger">Логгер для проверки</param>
    /// <param name="expectedErrorMessage">Ожидаемое сообщение об ошибке (если null, проверяется наличие любых ошибок)</param>
    /// <returns>True, если найдены ошибки</returns>
    protected bool HasLogErrors<T>(FakeLogger<T> logger, string? expectedErrorMessage = null)
    {
        IReadOnlyList<LogEntry> logEntries = logger.LogEntries;
        List<LogEntry> errorEntries = logEntries.Where(r => r.Level >= LogLevel.Error).ToList();

        if (expectedErrorMessage != null)
        {
            return errorEntries.Any(r => r.Message.Contains(expectedErrorMessage));
        }

        return errorEntries.Any();
    }
}
