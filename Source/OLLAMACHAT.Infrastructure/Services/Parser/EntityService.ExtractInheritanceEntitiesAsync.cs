namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Services.Parser;

/// <summary>
/// Часть класса EntityService, содержащая метод ExtractInheritanceEntitiesAsync
/// </summary>
public partial class EntityService
{
    /// <summary>
    /// Рекурсивно извлекает сущности базовых классов и интерфейсов
    /// </summary>
    /// <param name="typeSymbol">Символ типа для анализа</param>
    /// <param name="semanticModel">Семантическая модель</param>
    /// <param name="currentDepth">Текущая глубина извлечения</param>
    /// <param name="options">Опции парсера</param>
    /// <param name="processedEntities">Множество уже обработанных сущностей для избежания дублирования</param>
    /// <returns>Список извлеченных сущностей наследования</returns>
    private async Task<List<ParsedEntity>> ExtractInheritanceEntitiesAsync(
        INamedTypeSymbol typeSymbol,
        SemanticModel semanticModel,
        int currentDepth,
        ParserOptions? options = null,
        HashSet<string>? processedEntities = null)
    {
        var inheritanceEntities = new List<ParsedEntity>();

        // Инициализируем множество обработанных сущностей, если оно не передано
        processedEntities ??= new HashSet<string>();

        if (typeSymbol == null || semanticModel == null)
        {
            logger.LogWarning("TypeSymbol or SemanticModel is null in ExtractInheritanceEntitiesAsync");
            return inheritanceEntities;
        }

        logger.LogDebug("Starting extraction of inheritance entities for type: {TypeName}", typeSymbol.Name);

        // Проверка на превышение максимальной глубины
        if (options?.MaxDepth != null && currentDepth >= options.MaxDepth.Value)
        {
            logger.LogWarning("Maximum depth ({MaxDepth}) reached for inheritance extraction of type: {TypeName} at depth {CurrentDepth}",
                options.MaxDepth.Value, typeSymbol.Name, currentDepth);
            return inheritanceEntities;
        }

        try
        {
            // Извлекаем базовый класс (если он есть и не System.Object)
            if (typeSymbol.BaseType != null &&
                typeSymbol.BaseType.SpecialType != SpecialType.System_Object)
            {
                var baseTypeName = typeSymbol.BaseType.ToDisplayString();

                // Проверяем, не обрабатывали ли мы уже эту сущность
                if (!processedEntities.Contains(baseTypeName))
                {
                    processedEntities.Add(baseTypeName);
                    logger.LogDebug("Extracting base class: {BaseClassName}", baseTypeName);

                    // Рекурсивно извлекаем базовый класс
                    var baseEntity = await ExtractEntityAsync(typeSymbol.BaseType, semanticModel, currentDepth + 1, options);
                    inheritanceEntities.Add(baseEntity);

                    // Рекурсивно извлекаем наследование базового класса
                    var baseInheritanceEntities = await ExtractInheritanceEntitiesAsync(
                        typeSymbol.BaseType, semanticModel, currentDepth + 1, options, processedEntities);
                    inheritanceEntities.AddRange(baseInheritanceEntities);
                }
            }

            // Извлекаем прямые интерфейсы
            foreach (var interfaceSymbol in typeSymbol.Interfaces)
            {
                var interfaceName = interfaceSymbol.ToDisplayString();

                // Проверяем, не обрабатывали ли мы уже эту сущность
                if (!processedEntities.Contains(interfaceName))
                {
                    processedEntities.Add(interfaceName);
                    logger.LogDebug("Extracting interface: {InterfaceName}", interfaceName);

                    // Извлекаем интерфейс
                    var interfaceEntity = await ExtractEntityAsync(interfaceSymbol, semanticModel, currentDepth + 1, options);
                    inheritanceEntities.Add(interfaceEntity);

                    // Рекурсивно извлекаем наследование интерфейса
                    var interfaceInheritanceEntities = await ExtractInheritanceEntitiesAsync(
                        interfaceSymbol, semanticModel, currentDepth + 1, options, processedEntities);
                    inheritanceEntities.AddRange(interfaceInheritanceEntities);
                }
            }

            logger.LogDebug("Extracted {Count} inheritance entities for type: {TypeName}",
                inheritanceEntities.Count, typeSymbol.Name);
            return inheritanceEntities;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error extracting inheritance entities for type: {TypeName}", typeSymbol.Name);
            return inheritanceEntities;
        }
    }
}