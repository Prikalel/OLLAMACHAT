namespace VelikiyPrikalel.OLLAMACHAT.Application.Models;

/// <summary>
/// Представляет одну проанализированную сущность кода (класс, метод и т.д.).
/// </summary>
/// <param name="Name">Имя сущности.</param>
/// <param name="Type">Тип сущности.</param>
/// <param name="Location">Расположение сущности в коде.</param>
/// <param name="Children">Дочерние сущности.</param>
/// <param name="Modifiers">Модификаторы сущности.</param>
/// <param name="Decorators">Декораторы сущности.</param>
/// <param name="Inheritance">Информация о наследовании.</param>
/// <param name="ReturnType">Тип возвращаемого значения.</param>
/// <param name="Parameters">Параметры.</param>
/// <param name="ImportData">Данные об импорте.</param>
public record ParsedEntity(
    string Name,
    ParsedEntityType Type,
    Location Location,
    List<ParsedEntity>? Children,
    List<string>? Modifiers,
    List<Decorator>? Decorators,
    ParsedEntityInheritance? Inheritance,
    string? ReturnType,
    List<ModelParameter>? Parameters,
    ParsedEntityImportData? ImportData
);