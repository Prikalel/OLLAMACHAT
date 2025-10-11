namespace OLLAMACHAT.Generated.Models.ParseRequest;

/// <summary>
/// Request to parse a C# file.
/// </summary>
[DataContract]
public class ParserRequestDto
{
    /// <summary>
    /// Path to the C# source file to parse, relative to the repository root.
    /// </summary>
    /// <value>Path to the C# source file to parse, relative to the repository root.</value>
    [Required]
    [DataMember(Name = "filePath")]
    public required string FilePath { get; set; }

    /// <summary>
    /// Gets or Sets Options
    /// </summary>
    [DataMember(Name = "options")]
    public ParserOptionsDto? Options { get; set; }

    /// <summary>
    /// Absolute path to the repository root for resolving context.
    /// </summary>
    /// <value>Absolute path to the repository root for resolving context.</value>
    [Required]
    [DataMember(Name = "repoPath")]
    public required string RepoPath { get; set; }

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
        sb.Append("class ParserRequest {\n");
        sb.Append("  FilePath: ").Append(FilePath).Append("\n");
        sb.Append("  RepoPath: ").Append(RepoPath).Append("\n");
        sb.Append("  Options: ").Append(Options).Append("\n");
        sb.Append("}\n");
        return sb.ToString();
    }
}
