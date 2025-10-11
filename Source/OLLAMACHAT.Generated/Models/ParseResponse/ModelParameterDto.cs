namespace OLLAMACHAT.Generated.Models.ParseResponse;

/// <summary>
/// </summary>
[DataContract]
public class ModelParameterDto
{
    /// <summary>
    /// Gets or Sets DefaultValue
    /// </summary>
    [DataMember(Name = "defaultValue")]
    public string? DefaultValue { get; set; }

    /// <summary>
    /// Gets or Sets Name
    /// </summary>
    [DataMember(Name = "name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or Sets Nullable
    /// </summary>
    [DataMember(Name = "nullable")]
    public bool? Nullable { get; set; }

    /// <summary>
    /// Gets or Sets FullTypeName
    /// </summary>
    [DataMember(Name = "fullTypeName")]
    public string? FullTypeName { get; set; }

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
        StringBuilder sb = new();
        sb.Append("class ModelParameter {\n");
        sb.Append("  Name: ").Append(Name).Append("\n");
        sb.Append("  FullTypeName: ").Append(FullTypeName).Append("\n");
        sb.Append("  Nullable: ").Append(Nullable).Append("\n");
        sb.Append("  DefaultValue: ").Append(DefaultValue).Append("\n");
        sb.Append("}\n");
        return sb.ToString();
    }
}
