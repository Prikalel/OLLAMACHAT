namespace OLLAMACHAT.Generated.Models.ParseRequest;

/// <summary>
/// Configuration options for selective parsing.
/// </summary>
[DataContract]
public class ParserOptionsDto : IEquatable<ParserOptionsDto>
{
    /// <summary>
    /// Gets or Sets ExtractInheritance
    /// </summary>
    [DataMember(Name = "extractInheritance")]
    public bool? ExtractInheritance { get; set; }

    /// <summary>
    /// Gets or Sets ExtractReferences
    /// </summary>
    [DataMember(Name = "extractReferences")]
    public bool? ExtractReferences { get; set; }

    /// <summary>
    /// Gets or Sets MaxDepth
    /// </summary>
    [DataMember(Name = "maxDepth")]
    public int? MaxDepth { get; set; }

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

        return obj.GetType() == GetType() && Equals((ParserOptionsDto)obj);
    }

    /// <summary>
    /// Returns true if ParserOptions instances are equal
    /// </summary>
    /// <param name="other">Instance of ParserOptions to be compared</param>
    /// <returns>Boolean</returns>
    public bool Equals(ParserOptionsDto other)
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
                ExtractReferences == other.ExtractReferences ||
                ExtractReferences != null &&
                ExtractReferences.Equals(other.ExtractReferences)
            ) &&
            (
                ExtractInheritance == other.ExtractInheritance ||
                ExtractInheritance != null &&
                ExtractInheritance.Equals(other.ExtractInheritance)
            ) &&
            (
                MaxDepth == other.MaxDepth ||
                MaxDepth != null &&
                MaxDepth.Equals(other.MaxDepth)
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
            if (ExtractReferences != null)
            {
                hashCode = hashCode * 59 + ExtractReferences.GetHashCode();
            }

            if (ExtractInheritance != null)
            {
                hashCode = hashCode * 59 + ExtractInheritance.GetHashCode();
            }

            if (MaxDepth != null)
            {
                hashCode = hashCode * 59 + MaxDepth.GetHashCode();
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
        sb.Append("class ParserOptions {\n");
        sb.Append("  ExtractReferences: ").Append(ExtractReferences).Append("\n");
        sb.Append("  ExtractInheritance: ").Append(ExtractInheritance).Append("\n");
        sb.Append("  MaxDepth: ").Append(MaxDepth).Append("\n");
        sb.Append("}\n");
        return sb.ToString();
    }

    #region Operators

#pragma warning disable 1591

    public static bool operator ==(ParserOptionsDto left, ParserOptionsDto right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ParserOptionsDto left, ParserOptionsDto right)
    {
        return !Equals(left, right);
    }

#pragma warning restore 1591

    #endregion Operators
}
