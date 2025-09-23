namespace VelikiyPrikalel.OLLAMACHAT.Application.Models;

/// <summary>
/// Представляет отношение между двумя сущностями.
/// </summary>
/// <param name="From">Имя исходной сущности.</param>
/// <param name="To">Имя целевой сущности.</param>
/// <param name="Type">Тип отношения.</param>
/// <param name="TargetFile">Относительный путь к целевому файлу, если отношение межфайловое.</param>
/// <param name="Location">Расположение отношения в коде.</param>
public record Relationship(
    string From,
    string To,
    RelationshipType Type,
    string? TargetFile,
    Location? Location
);