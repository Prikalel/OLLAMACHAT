namespace OLLAMACHAT.Generated.Models.ParseResponse;

/// <summary>
/// </summary>
[DataContract]
public class LocationDto
{
    /// <summary>
    /// Gets or Sets Start
    /// </summary>
    [Required]
    [DataMember(Name = "start")]
    public required PositionDto Start { get; set; }

    /// <summary>
    /// Gets or Sets End
    /// </summary>
    [Required]
    [DataMember(Name = "end")]
    public required PositionDto End { get; set; }

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
        sb.Append("class Location {\n");
        sb.Append("  Start: ").Append(Start).Append("\n");
        sb.Append("  End: ").Append(End).Append("\n");
        sb.Append("}\n");
        return sb.ToString();
    }
}
