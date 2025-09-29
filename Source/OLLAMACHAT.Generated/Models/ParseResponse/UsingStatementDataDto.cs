namespace OLLAMACHAT.Generated.Models.ParseResponse;

/// <summary>
/// </summary>
[DataContract]
public class UsingStatementDataDto : IEquatable<UsingStatementDataDto>
{
    /// <summary>
    /// Gets or Sets Source
    /// </summary>
    [DataMember(Name = "source")]
    public string Source { get; set; }

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

        return obj.GetType() == GetType() && Equals((UsingStatementDataDto)obj);
    }

    /// <summary>
    /// Returns true if UsingStatementData instances are equal
    /// </summary>
    /// <param name="other">Instance of UsingStatementData to be compared</param>
    /// <returns>Boolean</returns>
    public bool Equals(UsingStatementDataDto other)
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
            Source == other.Source ||
            Source != null &&
            Source.Equals(other.Source);
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
            if (Source != null)
            {
                hashCode = hashCode * 59 + Source.GetHashCode();
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
        sb.Append("class UsingStatementData {\n");
        sb.Append("  Source: ").Append(Source).Append("\n");
        sb.Append("}\n");
        return sb.ToString();
    }

    #region Operators

#pragma warning disable 1591

    public static bool operator ==(UsingStatementDataDto left, UsingStatementDataDto right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(UsingStatementDataDto left, UsingStatementDataDto right)
    {
        return !Equals(left, right);
    }

#pragma warning restore 1591

    #endregion Operators
}