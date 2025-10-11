using System.Text.Json.Serialization;

namespace OLLAMACHAT.Generated.Models.ParseRequest;

/// <summary>
/// Configuration options for selective parsing.
/// </summary>
[DataContract]
public class ParserOptionsDto
{
    /// <summary>
    /// Gets or Sets ExtractFullExtractInheritance
    /// </summary>
    [DataMember(Name = "extractFullInheritance")]
    [JsonPropertyName("extractFullInheritance")]
    public bool? ExtractFullExtractInheritance { get; set; }

    /// <summary>
    /// Gets or Sets ExtractUsingStatementData
    /// </summary>
    [DataMember(Name = "extractUsingStatementData")]
    public bool? ExtractUsingStatementData { get; set; }

    /// <summary>
    /// Gets or Sets MaxDepth
    /// </summary>
    [DataMember(Name = "maxDepth")]
    public int? MaxDepth { get; set; }

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
        sb.Append("class ParserOptions {\n");
        sb.Append("  ExtractFullExtractInheritance: ").Append(ExtractFullExtractInheritance).Append("\n");
        sb.Append("  ExtractUsingStatementData: ").Append(ExtractUsingStatementData).Append("\n");
        sb.Append("  MaxDepth: ").Append(MaxDepth).Append("\n");
        sb.Append("}\n");
        return sb.ToString();
    }
}
