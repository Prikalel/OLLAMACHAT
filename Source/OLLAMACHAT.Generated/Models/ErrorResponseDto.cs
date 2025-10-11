namespace OLLAMACHAT.Generated.Models;

/// <summary>
/// </summary>
[DataContract]
public class ErrorResponseDto
{
    /// <summary>
    /// Gets or Sets Code
    /// </summary>
    [Required]
    [DataMember(Name = "code")]
    public required string Code { get; set; }

    /// <summary>
    /// Gets or Sets Details
    /// </summary>
    [DataMember(Name = "details")]
    public string? Details { get; set; }

    /// <summary>
    /// Gets or Sets Message
    /// </summary>
    [Required]
    [DataMember(Name = "message")]
    public required string Message { get; set; }

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
        sb.Append("class ErrorResponse {\n");
        sb.Append("  Code: ").Append(Code).Append("\n");
        sb.Append("  Message: ").Append(Message).Append("\n");
        sb.Append("  Details: ").Append(Details).Append("\n");
        sb.Append("}\n");
        return sb.ToString();
    }
}
