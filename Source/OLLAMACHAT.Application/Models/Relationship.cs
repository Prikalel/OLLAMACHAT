namespace VelikiyPrikalel.OLLAMACHAT.Application.Models;

/// <summary>
/// Представляет отношение между двумя сущностями.
/// </summary>
/// <param name="FullNameFrom">Полное имя исходной сущности.</param>
/// <param name="FullNameTo">Полное имя целевой сущности.</param>
/// <param name="Type">Тип отношения.</param>
/// <param name="TargetDefinitionFilePath">Относительный путь к целевому файлу, если отношение межфайловое.</param>
public record Relationship(
    string FullNameFrom,
    string FullNameTo,
    RelationshipType Type,
    string? TargetDefinitionFilePath
)
{
    /// <summary>
    /// ctor.
    /// </summary>
    /// <param name="orig"><see cref="SimpleRelationship"/>.</param>
    public Relationship(SimpleRelationship orig)
        : this(orig.FullNameFrom, orig.FullNameTo, (RelationshipType)orig.Type, orig.TargetDefinitionFilePath)
    {
    }
}

/// <summary>
/// Представляет отношение между двумя сущностями.
/// </summary>
/// <param name="FullNameFrom">Полное имя исходной сущности.</param>
/// <param name="FullNameTo">Полное имя целевой сущности.</param>
/// <param name="Type">Тип отношения <see cref="SimpleRelationshipType"/>.</param>
/// <param name="TargetDefinitionFilePath">Относительный путь к целевому файлу, если отношение межфайловое.</param>
public record SimpleRelationship(
    string FullNameFrom,
    string FullNameTo,
    SimpleRelationshipType Type,
    string? TargetDefinitionFilePath
);
