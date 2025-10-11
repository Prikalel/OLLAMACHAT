namespace OLLAMACHAT.Generated.Models.ParseResponse;

/// <summary>
/// Represents a relationship between two entities.
/// </summary>
[DataContract]
public class RelationshipDto
{
    /// <summary>
    /// Gets or Sets Type
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    [XmlType("OLLAMACHAT.Generated.Models.ParseResponse.RelationshipDto.TypeEnumDto")]
    public enum TypeEnumDto
    {
        /// <summary>
        /// Enum CallsEnum for calls
        /// </summary>
        [EnumMember(Value = "calls")]
        CallsEnum = 0,

        /// <summary>
        /// Enum IsBaseForEnum for isBaseFor
        /// </summary>
        [EnumMember(Value = "isBaseFor")]
        IsBaseForEnum = 1,

        /// <summary>
        /// Enum SubscribesToEnum for subscribesTo
        /// </summary>
        [EnumMember(Value = "subscribesTo")]
        SubscribesToEnum = 2,

        /// <summary>
        /// Enum ObservedByEnum for observedBy
        /// </summary>
        [EnumMember(Value = "observedBy")]
        ObservedByEnum = 3
    }

    /// <summary>
    /// Full name of the source entity.
    /// </summary>
    /// <value>Full name of the source entity.</value>
    [Required]
    [DataMember(Name = "fullNameFrom")]
    public required string FullNameFrom { get; set; }

    /// <summary>
    /// Relative path to the target file if the relationship is cross-file.
    /// </summary>
    /// <value>Relative path to the target file if the relationship is cross-file.</value>
    [DataMember(Name = "targetDefinitionFilePath")]
    public string? TargetDefinitionFilePath { get; set; }

    /// <summary>
    /// Full name of the target entity.
    /// </summary>
    /// <value>Full name of the target entity.</value>
    [Required]
    [DataMember(Name = "fullNameTo")]
    public required string FullNameTo { get; set; }

    /// <summary>
    /// Gets or Sets Type
    /// </summary>
    [Required]
    [DataMember(Name = "type")]
    public required TypeEnumDto Type { get; set; }

    /// <summary>
    /// Returns the JSON string presentation of the object
    /// </summary>
    /// <returns>JSON string presentation of the object</returns>
    public string ToJson() => JsonConvert.SerializeObject(this, Formatting.Indented);

    /// <summary>
    /// Returns the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("class Relationship {\n");
        sb.Append("  FullNameFrom: ").Append(FullNameFrom).Append("\n");
        sb.Append("  FullNameTo: ").Append(FullNameTo).Append("\n");
        sb.Append("  Type: ").Append(Type).Append("\n");
        sb.Append("  TargetDefinitionFilePath: ").Append(TargetDefinitionFilePath).Append("\n");
        sb.Append("}\n");
        return sb.ToString();
    }
}
