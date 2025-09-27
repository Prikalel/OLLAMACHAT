namespace OLLAMACHAT.Generated.Models;

/// <summary>
/// </summary>
[DataContract]
public class ParseErrorDto : IEquatable<ParseErrorDto>
{
    /// <summary>
    /// Gets or Sets Severity
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SeverityEnumDto
    {
        /// <summary>
        /// Enum ErrorEnum for error
        /// </summary>
        [EnumMember(Value = "error")]
        ErrorEnum = 0,

        /// <summary>
        /// Enum WarningEnum for warning
        /// </summary>
        [EnumMember(Value = "warning")]
        WarningEnum = 1,

        /// <summary>
        /// Enum InfoEnum for info
        /// </summary>
        [EnumMember(Value = "info")]
        InfoEnum = 2
    }

    /// <summary>
    /// Gets or Sets Location
    /// </summary>
    [DataMember(Name = "location")]
    public LocationDto Location { get; set; }

    /// <summary>
    /// Gets or Sets Message
    /// </summary>
    [DataMember(Name = "message")]
    public string Message { get; set; }

    /// <summary>
    /// Gets or Sets Severity
    /// </summary>
    [DataMember(Name = "severity")]
    public SeverityEnumDto? Severity { get; set; }

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

        return obj.GetType() == GetType() && Equals((ParseErrorDto)obj);
    }

    /// <summary>
    /// Returns true if ParseError instances are equal
    /// </summary>
    /// <param name="other">Instance of ParseError to be compared</param>
    /// <returns>Boolean</returns>
    public bool Equals(ParseErrorDto other)
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
                Message == other.Message ||
                Message != null &&
                Message.Equals(other.Message)
            ) &&
            (
                Severity == other.Severity ||
                Severity != null &&
                Severity.Equals(other.Severity)
            ) &&
            (
                Location == other.Location ||
                Location != null &&
                Location.Equals(other.Location)
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
            if (Message != null)
            {
                hashCode = hashCode * 59 + Message.GetHashCode();
            }

            if (Severity != null)
            {
                hashCode = hashCode * 59 + Severity.GetHashCode();
            }

            if (Location != null)
            {
                hashCode = hashCode * 59 + Location.GetHashCode();
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
        sb.Append("class ParseError {\n");
        sb.Append("  Message: ").Append(Message).Append("\n");
        sb.Append("  Severity: ").Append(Severity).Append("\n");
        sb.Append("  Location: ").Append(Location).Append("\n");
        sb.Append("}\n");
        return sb.ToString();
    }

    #region Operators

#pragma warning disable 1591

    public static bool operator ==(ParseErrorDto left, ParseErrorDto right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ParseErrorDto left, ParseErrorDto right)
    {
        return !Equals(left, right);
    }

#pragma warning restore 1591

    #endregion Operators
}
