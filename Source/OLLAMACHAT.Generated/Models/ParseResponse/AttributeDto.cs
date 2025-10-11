namespace OLLAMACHAT.Generated.Models.ParseResponse;

/// <summary>
/// </summary>
[DataContract]
public class AttributeDto
{
    /// <summary>
    /// Gets or Sets Arguments
    /// </summary>
    [DataMember(Name = "arguments")]
    public List<string>? Arguments { get; set; }

    /// <summary>
    /// Gets or Sets Name
    /// </summary>
    [DataMember(Name = "name")]
    public string? Name { get; set; }

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
        sb.Append("class Attribute {\n");
        sb.Append("  Name: ").Append(Name).Append("\n");
        sb.Append("  Arguments: ").Append(Arguments).Append("\n");
        sb.Append("}\n");
        return sb.ToString();
    }
}
