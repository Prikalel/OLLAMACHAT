namespace OLLAMACHAT.Generated.Models;

/// <summary>
/// </summary>
[DataContract]
public class ResolveImportRequestDto
{
    /// <summary>
    /// The path of the file containing the import.
    /// </summary>
    /// <value>The path of the file containing the import.</value>
    [Required]
    [DataMember(Name = "filePath")]
    public required string FilePath { get; set; }

    /// <summary>
    /// The import/using path to resolve.
    /// </summary>
    /// <value>The import/using path to resolve.</value>
    [Required]
    [DataMember(Name = "importPath")]
    public required string ImportPath { get; set; }

    /// <summary>
    /// Absolute path to the repository root.
    /// </summary>
    /// <value>Absolute path to the repository root.</value>
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
        StringBuilder sb = new();
        sb.Append("class ResolveImportRequest {\n");
        sb.Append("  ImportPath: ").Append(ImportPath).Append("\n");
        sb.Append("  FilePath: ").Append(FilePath).Append("\n");
        sb.Append("  RepoPath: ").Append(RepoPath).Append("\n");
        sb.Append("}\n");
        return sb.ToString();
    }
}
