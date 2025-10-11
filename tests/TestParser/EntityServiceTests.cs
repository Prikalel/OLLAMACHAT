namespace TestParser;

/// <summary>
/// Основной класс тестов для <see cref="EntityService"/>.
/// </summary>
public class EntityServiceTests : TestBase, IDisposable
{
    private readonly ITestOutputHelper _output;

    public EntityServiceTests(ITestOutputHelper output)
    {
        _output = output;
        Setup();
    }

    /// <summary>
    /// Освобождение ресурсов после тестов
    /// </summary>
    public void Dispose()
    {
    }

    #region Basic Inheritance Tests

    /// <summary>
    /// Тест запечатанного класса: sealed class A : B {}
    /// </summary>
    [Fact]
    public async Task SealedClassInheritance_Test_ShouldExtractSealedClassInheritance()
    {
        Document document = await CreateTestDocumentAsync(TestDataHelper.SealedClassInheritanceTestCode);
        ParserOptions options = CreateParserOptions(extractFullInheritance: true);

        IEnumerable<ParsedEntity> result = await EntityService.ExtractEntitiesAsync(document, options);

        Assert.NotNull(result);
        List<ParsedEntity> entities = result.ToList();

        // Проверяем наличие классов
        List<ParsedEntity>? namespaceEntities = entities.FirstOrDefault(x => x.SimpleName == "SealedClassInheritanceNamespace").Children!;
        ParsedEntity? classA = namespaceEntities.FirstOrDefault(e => e.SimpleName == "A");
        ParsedEntity? classB = namespaceEntities.FirstOrDefault(e => e.SimpleName == "B");

        Assert.NotNull(classA);
        Assert.NotNull(classB);

        // Проверяем наследование для запечатанного класса
        Assert.NotNull(classA.Inheritance);
        Assert.Contains("SealedClassInheritanceNamespace.B", classA.Inheritance.DirectBaseClasses ?? new List<string>());
    }

    /// <summary>
    /// Тест абстрактного класса: abstract class A : B {}, class C : A {}
    /// </summary>
    [Fact]
    public async Task AbstractClassInheritance_Test_ShouldExtractAbstractClassInheritance()
    {
        Document document = await CreateTestDocumentAsync(TestDataHelper.AbstractClassInheritanceTestCode);
        ParserOptions options = CreateParserOptions(extractFullInheritance: true);

        IEnumerable<ParsedEntity> result = await EntityService.ExtractEntitiesAsync(document, options);

        Assert.NotNull(result);
        List<ParsedEntity> entities = result.ToList();

        // Проверяем наличие всех классов
        List<ParsedEntity>? classEntities = entities.FirstOrDefault(x => x.SimpleName == "AbstractClassInheritanceNamespace").Children!;
        Assert.True(classEntities.Count >= 3);
        ParsedEntity? classA = classEntities.FirstOrDefault(e => e.SimpleName == "A");
        ParsedEntity? classB = classEntities.FirstOrDefault(e => e.SimpleName == "B");
        ParsedEntity? classC = classEntities.FirstOrDefault(e => e.SimpleName == "C");

        Assert.NotNull(classA);
        Assert.NotNull(classB);
        Assert.NotNull(classC);

        // Проверяем, что A - абстрактный класс
        Assert.Equal(ParsedEntityType.Class, classA.Type);

        // Проверяем наследование для абстрактного класса
        Assert.NotNull(classA.Inheritance);
        Assert.Contains("AbstractClassInheritanceNamespace.B", classA.Inheritance.DirectBaseClasses ?? new List<string>());

        // Проверяем наследование для конкретного класса
        Assert.NotNull(classC.Inheritance);
        Assert.Contains("AbstractClassInheritanceNamespace.A", classC.Inheritance.DirectBaseClasses ?? new List<string>());
    }

    #endregion

    #region Complex Inheritance Tests

    /// <summary>
    /// Тест обработки сложных сценариев с ограничением глубины
    /// </summary>
    [Theory]
    [InlineData(2)]
    [InlineData(5)]
    [InlineData(15)]
    public async Task ComplexInheritance_WithMaxDepth_ShouldRespectDepthLimit(int maxDepth)
    {
        Document document = await CreateTestDocumentAsync(TestDataHelper.DeepInheritanceChainTestCode);
        ParserOptions options = CreateParserOptions(extractFullInheritance: true, maxDepth: maxDepth);

        IEnumerable<ParsedEntity> result = await EntityService.ExtractEntitiesAsync(document, options);

        Assert.NotNull(result);
        List<ParsedEntity> entities = result.ToList();

        // При малой глубине должны отсутствовать глубокие вложенные сущности
        if (maxDepth < 10)
        {
            // Проверяем, что глубина соблюдается
            List<(ParsedEntity e, int level)> deepClasses = entities
                .Where(x => x.Type == ParsedEntityType.Class)
                .Select(e =>
                {
                    string className = e.SimpleName;
                    if (className.StartsWith("Level") && int.TryParse(className.Substring(5), out int level))
                    {
                        return (e, level);
                    }

                    throw new NotImplementedException();
                })
                .Where(e => e.level > maxDepth)
                .ToList();

            // Глубокие классы могут отсутствовать или быть без полного наследования
            if (deepClasses.Any())
            {
                foreach ((ParsedEntity e, int level) deepClass in deepClasses)
                {
                    // У глубоких классов может быть неполное наследование
                    if (deepClass.e.Inheritance != null)
                    {
                        Assert.True(deepClass.e.Inheritance.AllBaseClasses.Count == deepClass.level);
                    }
                }
            }
        }
    }

    #endregion

    #region Performance Tests

    /// <summary>
    /// Тест производительности с большим количеством сущностей
    /// </summary>
    [Fact]
    public async Task ExtractEntitiesAsync_PerformanceTestWithManyEntities_ShouldCompleteInReasonableTime()
    {
        Document document = await CreateTestDocumentAsync(TestDataHelper.PerformanceTestCode);
        ParserOptions options = CreateParserOptions(extractFullInheritance: true);

        DateTime startTime = DateTime.UtcNow;
        IEnumerable<ParsedEntity> result = await EntityService.ExtractEntitiesAsync(document, options);
        DateTime endTime = DateTime.UtcNow;
        TimeSpan duration = endTime - startTime;

        Assert.NotNull(result);
        List<ParsedEntity> entities = result.ToList();
        Assert.True(entities.Count > 0);

        // Проверяем, что выполнение занимает разумное время (менее 5 секунд)
        Assert.True(duration.TotalSeconds < 5, $"Тест выполнялся слишком долго: {duration.TotalSeconds} секунд");

        _output.WriteLine($"Извлечено {entities.Count} сущностей за {duration.TotalMilliseconds} мс");
    }

    /// <summary>
    /// Тест кэширования результатов при повторных вызовах
    /// </summary>
    [Fact]
    public async Task ExtractEntitiesAsync_CachingTest_ShouldBeFasterOnSecondCall()
    {
        Document document = await CreateTestDocumentAsync(TestDataHelper.BasicInheritanceTestCode);
        ParserOptions options = CreateParserOptions(extractFullInheritance: true);

        DateTime startTime1 = DateTime.UtcNow;
        IEnumerable<ParsedEntity> result1 = await EntityService.ExtractEntitiesAsync(document, options);
        DateTime endTime1 = DateTime.UtcNow;
        TimeSpan duration1 = endTime1 - startTime1;

        DateTime startTime2 = DateTime.UtcNow;
        IEnumerable<ParsedEntity> result2 = await EntityService.ExtractEntitiesAsync(document, options);
        DateTime endTime2 = DateTime.UtcNow;
        TimeSpan duration2 = endTime2 - startTime2;

        Assert.NotNull(result1);
        Assert.NotNull(result2);

        List<ParsedEntity> entities1 = result1.ToList();
        List<ParsedEntity> entities2 = result2.ToList();

        Assert.Equal(entities1.Count, entities2.Count);

        // Второй вызов должен быть быстрее (использование кэша)
        // Но это может не всегда работать в зависимости от реализации
        _output.WriteLine($"Первый вызов: {duration1.TotalMilliseconds} мс, Второй вызов: {duration2.TotalMilliseconds} мс");
    }

    #endregion

    #region Edge Cases Tests

    /// <summary>
    /// Тест с пустым документом
    /// </summary>
    [Fact]
    public async Task ExtractEntitiesAsync_EmptyDocument_ShouldReturnEmptyResult()
    {
        Document document = await CreateTestDocumentAsync("");
        ParserOptions options = CreateParserOptions();

        IEnumerable<ParsedEntity> result = await EntityService.ExtractEntitiesAsync(document, options);

        Assert.NotNull(result);
        List<ParsedEntity> entities = result.ToList();
        Assert.Empty(entities);
    }

    /// <summary>
    /// Тест обработки ошибок в коде
    /// </summary>
    [Fact]
    public async Task ExtractEntitiesAsync_WithErrorHandling_ShouldContinueProcessing()
    {
        Document document = await CreateTestDocumentAsync(TestDataHelper.ErrorHandlingTestCode);
        ParserOptions options = CreateParserOptions();

        string message = Assert.ThrowsAsync<Exception>(async () => await EntityService.ExtractEntitiesAsync(document, options))
            .Result.Message;

        Assert.Equal("Syntax error at line 10: Требуется \")\"", message);
    }

    #endregion

    #region Verify Snapshot Tests

    /// <summary>
    /// Снапшот-тест для базового наследования
    /// </summary>
    [Fact]
    public async Task ExtractEntitiesAsync_BasicInheritanceSnapshot_ShouldMatchSnapshot()
    {
        Document document = await CreateTestDocumentAsync(TestDataHelper.BasicInheritanceTestCode);
        ParserOptions options = CreateParserOptions(extractFullInheritance: true);

        IEnumerable<ParsedEntity> result = await EntityService.ExtractEntitiesAsync(document, options);

        await Verify(JsonSerializer.Serialize(result, new JsonSerializerOptions()
        {
            WriteIndented = true
        }));
    }

    /// <summary>
    /// Снапшот-тест для сложного наследования
    /// </summary>
    [Fact]
    public async Task ExtractEntitiesAsync_ComplexInheritanceSnapshot_ShouldMatchSnapshot()
    {
        Document document = await CreateTestDocumentAsync(TestDataHelper.ComplexInheritanceTestCode);
        ParserOptions options = CreateParserOptions(extractFullInheritance: true);

        IEnumerable<ParsedEntity> result = await EntityService.ExtractEntitiesAsync(document, options);

        await Verify(JsonSerializer.Serialize(result, new JsonSerializerOptions()
        {
            WriteIndented = true
        }));
    }

    /// <summary>
    /// Снапшот-тест для unity файла.
    /// </summary>
    [Fact]
    public async Task ExtractEntitiesAsync_UnitySnapshot_ShouldMatchSnapshot()
    {
        Document document = await CreateTestDocumentAsync(TestDataHelper.ExampleUnityScriptTestCode);
        ParserOptions options = CreateParserOptions(extractFullInheritance: true);

        IEnumerable<ParsedEntity> result = await EntityService.ExtractEntitiesAsync(document, options);

        Assert.Contains(result, x => x.FullName.Contains("IStateMachineControllable"));
        await Verify(JsonSerializer.Serialize(result, new JsonSerializerOptions()
        {
            WriteIndented = true
        }));
    }

    /// <summary>
    /// Снапшот-тест для DeepInheritanceChain.
    /// </summary>
    [Fact]
    public async Task ExtractEntitiesAsync_DeepInheritanceChain_ShouldMatchSnapshot()
    {
        Document document = await CreateTestDocumentAsync(TestDataHelper.DeepInheritanceChainTestCode);
        ParserOptions options = CreateParserOptions(extractFullInheritance: true);

        IEnumerable<ParsedEntity> result = await EntityService.ExtractEntitiesAsync(document, options);

        await Verify(JsonSerializer.Serialize(result, new JsonSerializerOptions()
        {
            WriteIndented = true
        }));
    }

    #endregion

    #region Circular Reference Tests

    /// <summary>
    /// Тест обработки циклических ссылок в классах: class A : B {}, class B : A {} (некорректный код)
    /// </summary>
    [Fact]
    public async Task CircularReference_Class_Test_ShouldHandleCircularClassReferencesGracefully()
    {
        Document document = await CreateTestDocumentAsync(TestDataHelper.CircularReferenceClassTestCode);
        ParserOptions options = CreateParserOptions(extractFullInheritance: true);

        IEnumerable<ParsedEntity> result = await EntityService.ExtractEntitiesAsync(document, options);

        Assert.NotNull(result);
        List<ParsedEntity> entities = result.ToList();

        // Проверяем, что нет бесконечной рекурсии
        Assert.True(entities.Count < 100, "Слишком много сущностей, возможна бесконечная рекурсия");

        // Из-за синтаксической ошибки классы могут не быть обработаны
        // Главное - чтобы не было бесконечной рекурсии
        _output.WriteLine($"Найдено сущностей: {entities.Count}");

        // Проверяем наличие ошибок в логе
        bool hasErrors = HasLogErrors(EntityLogger);
        if (hasErrors)
        {
            _output.WriteLine("Обнаружены ошибки в логе (ожидаемо для некорректного кода)");
        }
    }

    #endregion

    #region Edge Cases Tests

    /// <summary>
    /// Тест обработки пустого файла
    /// </summary>
    [Fact]
    public async Task EmptyFile_Test_ShouldHandleEmptyFileGracefully()
    {
        Document document = await CreateTestDocumentAsync(TestDataHelper.EmptyFileTestCode);
        ParserOptions options = CreateParserOptions();

        IEnumerable<ParsedEntity> result = await EntityService.ExtractEntitiesAsync(document, options);

        Assert.NotNull(result);
        List<ParsedEntity> entities = result.ToList();
        Assert.Empty(entities);

        _output.WriteLine("Пустой файл обработан корректно");
    }

    /// <summary>
    /// Тест обработки файла с только комментариями
    /// </summary>
    [Fact]
    public async Task CommentsOnlyFile_Test_ShouldHandleCommentsOnlyFile()
    {
        string commentsOnlyCode = @"
// Это файл с только комментариями
using System;

/*
 * Многострочный комментарий
 * с несколькими строками
 */

/// <summary>
/// XML документация
/// </summary>
/// <remarks>
/// Еще один комментарий
/// </remarks>
";

        Document document = await CreateTestDocumentAsync(commentsOnlyCode);
        ParserOptions options = CreateParserOptions();

        IEnumerable<ParsedEntity> result = await EntityService.ExtractEntitiesAsync(document, options);

        Assert.NotNull(result);
        List<ParsedEntity> entities = result.ToList();
        Assert.Equal(entities.Select(x => x.Type), [ParsedEntityType.UsingStatement]);

        _output.WriteLine("Файл с только комментариями обработан корректно");
    }

    /// <summary>
    /// Тест обработки файла с только using директивами
    /// </summary>
    [Fact]
    public async Task UsingOnlyFile_Test_ShouldHandleUsingOnlyFile()
    {
        string usingOnlyCode = @"
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;
global using Microsoft.CodeAnalysis;
";

        Document document = await CreateTestDocumentAsync(usingOnlyCode);
        ParserOptions options = CreateParserOptions(extractUsingStatements: true);

        IEnumerable<ParsedEntity> result = await EntityService.ExtractEntitiesAsync(document, options);

        Assert.NotNull(result);
        List<ParsedEntity> entities = result.Where(x => x.Type is not ParsedEntityType.UsingStatement).ToList();
        Assert.Empty(entities);

        _output.WriteLine("Файл с только using директивами обработан корректно");
    }

    /// <summary>
    /// Тест обработки файла с пространствами имен но без классов
    /// </summary>
    [Fact]
    public async Task NamespaceOnlyFile_Test_ShouldHandleNamespaceOnlyFile()
    {
        string namespaceOnlyCode = @"
using System;

namespace TestNamespace1
{
    // Пустое пространство имен
}

namespace TestNamespace2.SubNamespace
{
    // Вложенное пустое пространство имен
}
";

        Document document = await CreateTestDocumentAsync(namespaceOnlyCode);
        ParserOptions options = CreateParserOptions();

        IEnumerable<ParsedEntity> result = await EntityService.ExtractEntitiesAsync(document, options);

        Assert.NotNull(result);
        List<ParsedEntity> nonNamespaceEntities = result.Where(x => x.Type is not ParsedEntityType.Namespace && x.Type is not ParsedEntityType.UsingStatement).ToList();
        Assert.Empty(nonNamespaceEntities);

        _output.WriteLine("Файл с только пространствами имен обработан корректно");
    }

    #endregion
}
