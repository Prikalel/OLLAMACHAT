namespace OLLAMACHAT.Generated.Models.ParseResponse;

/// <summary>
/// </summary>
[DataContract]
public class ParsedEntityInheritanceDto
{
    /// <summary>
    /// Gets or Sets DirectBaseClasses
    /// </summary>
    [DataMember(Name = "directBaseClasses")]
    public required List<string> DirectBaseClasses { get; set; }

    /// <summary>
    /// Gets or Sets DirectInterfaces
    /// </summary>
    [DataMember(Name = "directInterfaces")]
    public required List<string> DirectInterfaces { get; set; }

    /// <summary>
    /// Gets or Sets AllBaseClasses
    /// </summary>
    [DataMember(Name = "allBaseClasses")]
    public List<string>? AllBaseClasses { get; set; }

    /// <summary>
    /// Gets or Sets AllInterfaces
    /// </summary>
    [DataMember(Name = "allInterfaces")]
    public List<string>? AllInterfaces { get; set; }

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
        sb.Append("class ParsedEntityInheritance {\n");
        sb.Append("  DirectBaseClasses: ").Append(DirectBaseClasses).Append("\n");
        sb.Append("  DirectInterfaces: ").Append(DirectInterfaces).Append("\n");
        sb.Append("  AllBaseClasses: ").Append(AllBaseClasses).Append("\n");
        sb.Append("  AllInterfaces: ").Append(AllInterfaces).Append("\n");
        sb.Append("}\n");
        return sb.ToString();
    }
}
