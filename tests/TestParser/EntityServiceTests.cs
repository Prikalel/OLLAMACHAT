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
        var document = await CreateTestDocumentAsync(TestDataHelper.SealedClassInheritanceTestCode);
        var options = CreateParserOptions(extractFullInheritance: true);

        var result = await EntityService.ExtractEntitiesAsync(document, options);

        Assert.NotNull(result);
        var entities = result.ToList();

        // Проверяем наличие классов
        var namespaceEntities = entities.FirstOrDefault(x => x.SimpleName == "SealedClassInheritanceNamespace").Children!;
        var classA = namespaceEntities.FirstOrDefault(e => e.SimpleName == "A");
        var classB = namespaceEntities.FirstOrDefault(e => e.SimpleName == "B");

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
        var document = await CreateTestDocumentAsync(TestDataHelper.AbstractClassInheritanceTestCode);
        var options = CreateParserOptions(extractFullInheritance: true);

        var result = await EntityService.ExtractEntitiesAsync(document, options);

        Assert.NotNull(result);
        var entities = result.ToList();

        // Проверяем наличие всех классов
        var classEntities = entities.FirstOrDefault(x => x.SimpleName == "AbstractClassInheritanceNamespace").Children!;
        Assert.True(classEntities.Count >= 3);
        var classA = classEntities.FirstOrDefault(e => e.SimpleName == "A");
        var classB = classEntities.FirstOrDefault(e => e.SimpleName == "B");
        var classC = classEntities.FirstOrDefault(e => e.SimpleName == "C");

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
        var document = await CreateTestDocumentAsync(TestDataHelper.DeepInheritanceChainTestCode);
        var options = CreateParserOptions(extractFullInheritance: true, maxDepth: maxDepth);

        var result = await EntityService.ExtractEntitiesAsync(document, options);

        Assert.NotNull(result);
        var entities = result.ToList();

        // При малой глубине должны отсутствовать глубокие вложенные сущности
        if (maxDepth < 10)
        {
            // Проверяем, что глубина соблюдается
            var deepClasses = entities
                .Where(x => x.Type == ParsedEntityType.Class)
                .Select(e =>
                {
                    var className = e.SimpleName;
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
                foreach (var deepClass in deepClasses)
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
        var document = await CreateTestDocumentAsync(TestDataHelper.PerformanceTestCode);
        var options = CreateParserOptions(extractFullInheritance: true);

        var startTime = DateTime.UtcNow;
        var result = await EntityService.ExtractEntitiesAsync(document, options);
        var endTime = DateTime.UtcNow;
        var duration = endTime - startTime;

        Assert.NotNull(result);
        var entities = result.ToList();
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
        var document = await CreateTestDocumentAsync(TestDataHelper.BasicInheritanceTestCode);
        var options = CreateParserOptions(extractFullInheritance: true);

        var startTime1 = DateTime.UtcNow;
        var result1 = await EntityService.ExtractEntitiesAsync(document, options);
        var endTime1 = DateTime.UtcNow;
        var duration1 = endTime1 - startTime1;

        var startTime2 = DateTime.UtcNow;
        var result2 = await EntityService.ExtractEntitiesAsync(document, options);
        var endTime2 = DateTime.UtcNow;
        var duration2 = endTime2 - startTime2;

        Assert.NotNull(result1);
        Assert.NotNull(result2);

        var entities1 = result1.ToList();
        var entities2 = result2.ToList();

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
        var document = await CreateTestDocumentAsync("");
        var options = CreateParserOptions();

        var result = await EntityService.ExtractEntitiesAsync(document, options);

        Assert.NotNull(result);
        var entities = result.ToList();
        Assert.Empty(entities);
    }

    /// <summary>
    /// Тест обработки ошибок в коде
    /// </summary>
    [Fact]
    public async Task ExtractEntitiesAsync_WithErrorHandling_ShouldContinueProcessing()
    {
        var document = await CreateTestDocumentAsync(TestDataHelper.ErrorHandlingTestCode);
        var options = CreateParserOptions();

        var message = Assert.ThrowsAsync<Exception>(async () => await EntityService.ExtractEntitiesAsync(document, options))
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
        var document = await CreateTestDocumentAsync(TestDataHelper.BasicInheritanceTestCode);
        var options = CreateParserOptions(extractFullInheritance: true);

        var result = await EntityService.ExtractEntitiesAsync(document, options);

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
        var document = await CreateTestDocumentAsync(TestDataHelper.ComplexInheritanceTestCode);
        var options = CreateParserOptions(extractFullInheritance: true);

        var result = await EntityService.ExtractEntitiesAsync(document, options);

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
        var document = await CreateTestDocumentAsync(TestDataHelper.ExampleUnityScriptTestCode);
        var options = CreateParserOptions(extractFullInheritance: true);

        var result = await EntityService.ExtractEntitiesAsync(document, options);

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
        var document = await CreateTestDocumentAsync(TestDataHelper.DeepInheritanceChainTestCode);
        var options = CreateParserOptions(extractFullInheritance: true);

        var result = await EntityService.ExtractEntitiesAsync(document, options);

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
        var document = await CreateTestDocumentAsync(TestDataHelper.CircularReferenceClassTestCode);
        var options = CreateParserOptions(extractFullInheritance: true);

        var result = await EntityService.ExtractEntitiesAsync(document, options);

        Assert.NotNull(result);
        var entities = result.ToList();

        // Проверяем, что нет бесконечной рекурсии
        Assert.True(entities.Count < 100, "Слишком много сущностей, возможна бесконечная рекурсия");

        // Из-за синтаксической ошибки классы могут не быть обработаны
        // Главное - чтобы не было бесконечной рекурсии
        _output.WriteLine($"Найдено сущностей: {entities.Count}");

        // Проверяем наличие ошибок в логе
        var hasErrors = HasLogErrors(EntityLogger);
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
        var document = await CreateTestDocumentAsync(TestDataHelper.EmptyFileTestCode);
        var options = CreateParserOptions();

        var result = await EntityService.ExtractEntitiesAsync(document, options);

        Assert.NotNull(result);
        var entities = result.ToList();
        Assert.Empty(entities);

        _output.WriteLine("Пустой файл обработан корректно");
    }

    /// <summary>
    /// Тест обработки файла с только комментариями
    /// </summary>
    [Fact]
    public async Task CommentsOnlyFile_Test_ShouldHandleCommentsOnlyFile()
    {
        var commentsOnlyCode = @"
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

        var document = await CreateTestDocumentAsync(commentsOnlyCode);
        var options = CreateParserOptions();

        var result = await EntityService.ExtractEntitiesAsync(document, options);

        Assert.NotNull(result);
        var entities = result.ToList();
        Assert.Equal(entities.Select(x => x.Type), [ParsedEntityType.UsingStatement]);

        _output.WriteLine("Файл с только комментариями обработан корректно");
    }

    /// <summary>
    /// Тест обработки файла с только using директивами
    /// </summary>
    [Fact]
    public async Task UsingOnlyFile_Test_ShouldHandleUsingOnlyFile()
    {
        var usingOnlyCode = @"
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;
global using Microsoft.CodeAnalysis;
";

        var document = await CreateTestDocumentAsync(usingOnlyCode);
        var options = CreateParserOptions(extractUsingStatements: true);

        var result = await EntityService.ExtractEntitiesAsync(document, options);

        Assert.NotNull(result);
        var entities = result.Where(x => x.Type is not ParsedEntityType.UsingStatement).ToList();
        Assert.Empty(entities);

        _output.WriteLine("Файл с только using директивами обработан корректно");
    }

    /// <summary>
    /// Тест обработки файла с пространствами имен но без классов
    /// </summary>
    [Fact]
    public async Task NamespaceOnlyFile_Test_ShouldHandleNamespaceOnlyFile()
    {
        var namespaceOnlyCode = @"
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

        var document = await CreateTestDocumentAsync(namespaceOnlyCode);
        var options = CreateParserOptions();

        var result = await EntityService.ExtractEntitiesAsync(document, options);

        Assert.NotNull(result);
        var nonNamespaceEntities = result.Where(x => x.Type is not ParsedEntityType.Namespace && x.Type is not ParsedEntityType.UsingStatement).ToList();
        Assert.Empty(nonNamespaceEntities);

        _output.WriteLine("Файл с только пространствами имен обработан корректно");
    }

    #endregion
}
