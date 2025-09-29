namespace VelikiyPrikalel.OLLAMACHAT.Application.Models;

/// <summary>
/// Представляет одну проанализированную сущность кода (класс, метод и т.д.).
/// </summary>
/// <param name="SimpleName">Имя сущности.</param>
/// <param name="FullName">Полное имя сущности включая пространство имен.</param>
/// <param name="Type">Тип сущности.</param>
/// <param name="Location">Расположение сущности в коде.</param>
/// <param name="Children">Дочерние сущности.</param>
/// <param name="Modifiers">Модификаторы сущности.</param>
/// <param name="Attributes">Атрибуты сущности.</param>
/// <param name="Inheritance">Информация о наследовании.</param>
/// <param name="ReturnType">Тип возвращаемого значения.</param>
/// <param name="Parameters">Параметры.</param>
/// <param name="UsingStatementData">Данные об импорте.</param>
public record ParsedEntity(
    string SimpleName,
    string? FullName,
    ParsedEntityType Type,
    Location Location,
    List<ParsedEntity>? Children,
    List<string>? Modifiers,
    List<Attribute>? Attributes,
    ParsedEntityInheritance? Inheritance,
    string? ReturnType,
    List<ModelParameter>? Parameters,
    UsingStatementData? UsingStatementData
);