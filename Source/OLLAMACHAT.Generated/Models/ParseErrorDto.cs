namespace OLLAMACHAT.Generated.Models;

/// <summary>
/// </summary>
[DataContract]
public class ParseErrorDto
{
    /// <summary>
    /// Gets or Sets Severity
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SeverityEnumDto
    {
        /// <summary>
        /// Enum ErrorEnum for error
        /// </summary>
        [EnumMember(Value = "error")]
        ErrorEnum = 0,

        /// <summary>
        /// Enum WarningEnum for warning
        /// </summary>
        [EnumMember(Value = "warning")]
        WarningEnum = 1,

        /// <summary>
        /// Enum InfoEnum for info
        /// </summary>
        [EnumMember(Value = "info")]
        InfoEnum = 2
    }

    /// <summary>
    /// Gets or Sets Location
    /// </summary>
    [DataMember(Name = "location")]
    public LocationDto? Location { get; set; }

    /// <summary>
    /// Gets or Sets Message
    /// </summary>
    [DataMember(Name = "message")]
    public string? Message { get; set; }

    /// <summary>
    /// Gets or Sets Severity
    /// </summary>
    [DataMember(Name = "severity")]
    public SeverityEnumDto? Severity { get; set; }

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
        sb.Append("class ParseError {\n");
        sb.Append("  Message: ").Append(Message).Append("\n");
        sb.Append("  Severity: ").Append(Severity).Append("\n");
        sb.Append("  Location: ").Append(Location).Append("\n");
        sb.Append("}\n");
        return sb.ToString();
    }
}
