namespace OLLAMACHAT.Generated.Models.ParseResponse;

/// <summary>
/// </summary>
[DataContract]
public class ParsedEntityInheritanceDto : IEquatable<ParsedEntityInheritanceDto>
{
    /// <summary>
    /// Gets or Sets DirectBaseClasses
    /// </summary>
    [DataMember(Name = "directBaseClasses")]
    public List<string> DirectBaseClasses { get; set; }

    /// <summary>
    /// Gets or Sets DirectInterfaces
    /// </summary>
    [DataMember(Name = "directInterfaces")]
    public List<string> DirectInterfaces { get; set; }

    /// <summary>
    /// Gets or Sets AllBaseClasses
    /// </summary>
    [DataMember(Name = "allBaseClasses")]
    public List<string>? AllBaseClasses { get; set; }

    /// <summary>
    /// Gets or Sets AllInterfaces
    /// </summary>
    [DataMember(Name = "allInterfaces")]
    public List<string>? AllInterfaces { get; set; }

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

        return obj.GetType() == GetType() && Equals((ParsedEntityInheritanceDto)obj);
    }

    /// <summary>
    /// Returns true if ParsedEntityInheritance instances are equal
    /// </summary>
    /// <param name="other">Instance of ParsedEntityInheritance to be compared</param>
    /// <returns>Boolean</returns>
    public bool Equals(ParsedEntityInheritanceDto other)
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
                DirectBaseClasses == other.DirectBaseClasses ||
                DirectBaseClasses != null &&
                DirectBaseClasses.SequenceEqual(other.DirectBaseClasses)
            ) &&
            (
                DirectInterfaces == other.DirectInterfaces ||
                DirectInterfaces != null &&
                DirectInterfaces.SequenceEqual(other.DirectInterfaces)
            ) &&
            (
                AllBaseClasses == other.AllBaseClasses ||
                AllBaseClasses != null &&
                AllBaseClasses.SequenceEqual(other.AllBaseClasses)
            ) &&
            (
                AllInterfaces == other.AllInterfaces ||
                AllInterfaces != null &&
                AllInterfaces.SequenceEqual(other.AllInterfaces)
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
            if (DirectBaseClasses != null)
            {
                hashCode = hashCode * 59 + DirectBaseClasses.GetHashCode();
            }

            if (DirectInterfaces != null)
            {
                hashCode = hashCode * 59 + DirectInterfaces.GetHashCode();
            }

            if (AllBaseClasses != null)
            {
                hashCode = hashCode * 59 + AllBaseClasses.GetHashCode();
            }

            if (AllInterfaces != null)
            {
                hashCode = hashCode * 59 + AllInterfaces.GetHashCode();
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
        sb.Append("class ParsedEntityInheritance {\n");
        sb.Append("  DirectBaseClasses: ").Append(DirectBaseClasses).Append("\n");
        sb.Append("  DirectInterfaces: ").Append(DirectInterfaces).Append("\n");
        sb.Append("  AllBaseClasses: ").Append(AllBaseClasses).Append("\n");
        sb.Append("  AllInterfaces: ").Append(AllInterfaces).Append("\n");
        sb.Append("}\n");
        return sb.ToString();
    }

    #region Operators

#pragma warning disable 1591

    public static bool operator ==(ParsedEntityInheritanceDto left, ParsedEntityInheritanceDto right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ParsedEntityInheritanceDto left, ParsedEntityInheritanceDto right)
    {
        return !Equals(left, right);
    }

#pragma warning restore 1591

    #endregion Operators
}
