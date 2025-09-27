namespace OLLAMACHAT.Generated.Models;

/// <summary>
/// </summary>
[DataContract]
public class ResolveImportRequestDto : IEquatable<ResolveImportRequestDto>
{
    /// <summary>
    /// The path of the file containing the import.
    /// </summary>
    /// <value>The path of the file containing the import.</value>
    [Required]
    [DataMember(Name = "filePath")]
    public string FilePath { get; set; }

    /// <summary>
    /// The import/using path to resolve.
    /// </summary>
    /// <value>The import/using path to resolve.</value>
    [Required]
    [DataMember(Name = "importPath")]
    public string ImportPath { get; set; }

    /// <summary>
    /// Absolute path to the repository root.
    /// </summary>
    /// <value>Absolute path to the repository root.</value>
    [Required]
    [DataMember(Name = "repoPath")]
    public string RepoPath { get; set; }

    /// <summary>
    /// Returns true if objects are equal
    /// </summary>
    /// <param name="obj">Object to be compared</param>
    /// <returns>Boolean</returns>
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        return obj.GetType() == GetType() && Equals((ResolveImportRequestDto)obj);
    }

    /// <summary>
    /// Returns true if ResolveImportRequest instances are equal
    /// </summary>
    /// <param name="other">Instance of ResolveImportRequest to be compared</param>
    /// <returns>Boolean</returns>
    public bool Equals(ResolveImportRequestDto other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return
            (
                ImportPath == other.ImportPath ||
                ImportPath != null &&
                ImportPath.Equals(other.ImportPath)
            ) &&
            (
                FilePath == other.FilePath ||
                FilePath != null &&
                FilePath.Equals(other.FilePath)
            ) &&
            (
                RepoPath == other.RepoPath ||
                RepoPath != null &&
                RepoPath.Equals(other.RepoPath)
            );
    }

    /// <summary>
    /// Gets the hash code
    /// </summary>
    /// <returns>Hash code</returns>
    public override int GetHashCode()
    {
        unchecked // Overflow is fine, just wrap
        {
            var hashCode = 41;
            // Suitable nullity checks etc, of course :)
            if (ImportPath != null)
            {
                hashCode = hashCode * 59 + ImportPath.GetHashCode();
            }

            if (FilePath != null)
            {
                hashCode = hashCode * 59 + FilePath.GetHashCode();
            }

            if (RepoPath != null)
            {
                hashCode = hashCode * 59 + RepoPath.GetHashCode();
            }

            return hashCode;
        }
    }

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
        var sb = new StringBuilder();
        sb.Append("class ResolveImportRequest {\n");
        sb.Append("  ImportPath: ").Append(ImportPath).Append("\n");
        sb.Append("  FilePath: ").Append(FilePath).Append("\n");
        sb.Append("  RepoPath: ").Append(RepoPath).Append("\n");
        sb.Append("}\n");
        return sb.ToString();
    }

    #region Operators

#pragma warning disable 1591

    public static bool operator ==(ResolveImportRequestDto left, ResolveImportRequestDto right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ResolveImportRequestDto left, ResolveImportRequestDto right)
    {
        return !Equals(left, right);
    }

#pragma warning restore 1591

    #endregion Operators
}
