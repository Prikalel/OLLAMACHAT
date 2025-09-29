namespace OLLAMACHAT.Generated.Models.ParseResponse;

/// <summary>
/// </summary>
[DataContract]
public class AttributeDto : IEquatable<AttributeDto>
{
    /// <summary>
    /// Gets or Sets Arguments
    /// </summary>
    [DataMember(Name = "arguments")]
    public List<string> Arguments { get; set; }

    /// <summary>
    /// Gets or Sets Name
    /// </summary>
    [DataMember(Name = "name")]
    public string Name { get; set; }

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

        return obj.GetType() == GetType() && Equals((AttributeDto)obj);
    }

    /// <summary>
    /// Returns true if Attribute instances are equal
    /// </summary>
    /// <param name="other">Instance of Attribute to be compared</param>
    /// <returns>Boolean</returns>
    public bool Equals(AttributeDto other)
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
                Name == other.Name ||
                Name != null &&
                Name.Equals(other.Name)
            ) &&
            (
                Arguments == other.Arguments ||
                Arguments != null &&
                Arguments.SequenceEqual(other.Arguments)
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
            if (Name != null)
            {
                hashCode = hashCode * 59 + Name.GetHashCode();
            }

            if (Arguments != null)
            {
                hashCode = hashCode * 59 + Arguments.GetHashCode();
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
        sb.Append("class Attribute {\n");
        sb.Append("  Name: ").Append(Name).Append("\n");
        sb.Append("  Arguments: ").Append(Arguments).Append("\n");
        sb.Append("}\n");
        return sb.ToString();
    }

    #region Operators

#pragma warning disable 1591

    public static bool operator ==(AttributeDto left, AttributeDto right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(AttributeDto left, AttributeDto right)
    {
        return !Equals(left, right);
    }

#pragma warning restore 1591

    #endregion Operators
}
