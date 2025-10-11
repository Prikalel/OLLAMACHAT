namespace OLLAMACHAT.Generated.Models.ParseResponse;

/// <summary>
/// Complete parsing result for a single file.
/// </summary>
[DataContract]
public class ParseResultDto
{
    /// <summary>
    /// Gets or Sets Language
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LanguageEnumDto
    {
        /// <summary>
        /// Enum CsharpEnum for csharp
        /// </summary>
        [EnumMember(Value = "csharp")]
        CsharpEnum = 0
    }

    /// <summary>
    /// SHA-256 hash of the file content for caching.
    /// </summary>
    /// <value>SHA-256 hash of the file content for caching.</value>
    [Required]
    [DataMember(Name = "contentHash")]
    public required string ContentHash { get; set; }

    /// <summary>
    /// Gets or Sets Entities
    /// </summary>
    [Required]
    [DataMember(Name = "entities")]
    public required List<ParsedEntityDto> Entities { get; set; }

    /// <summary>
    /// Gets or Sets Errors
    /// </summary>
    [DataMember(Name = "errors")]
    public List<ParseErrorDto>? Errors { get; set; }

    /// <summary>
    /// Gets or Sets FilePath
    /// </summary>
    [Required]
    [DataMember(Name = "filePath")]
    public required string FilePath { get; set; }

    /// <summary>
    /// Gets or Sets Language
    /// </summary>
    [Required]
    [DataMember(Name = "language")]
    public required LanguageEnumDto Language { get; set; }

    /// <summary>
    /// Time taken for parsing in milliseconds.
    /// </summary>
    /// <value>Time taken for parsing in milliseconds.</value>
    [Required]
    [DataMember(Name = "parseTimeMs")]
    public required int? ParseTimeMs { get; set; }

    /// <summary>
    /// Gets or Sets Relationships
    /// </summary>
    [DataMember(Name = "relationships")]
    public List<RelationshipDto>? Relationships { get; set; }

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
        sb.Append("class ParseResult {\n");
        sb.Append("  FilePath: ").Append(FilePath).Append("\n");
        sb.Append("  Language: ").Append(Language).Append("\n");
        sb.Append("  Entities: ").Append(Entities).Append("\n");
        sb.Append("  Relationships: ").Append(Relationships).Append("\n");
        sb.Append("  ContentHash: ").Append(ContentHash).Append("\n");
        sb.Append("  ParseTimeMs: ").Append(ParseTimeMs).Append("\n");
        sb.Append("  Errors: ").Append(Errors).Append("\n");
        sb.Append("}\n");
        return sb.ToString();
    }
}
