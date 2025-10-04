namespace OLLAMACHAT.Generated.Models.ParseResponse;

/// <summary>
/// Represents a relationship between two entities.
/// </summary>
[DataContract]
public class RelationshipDto : IEquatable<RelationshipDto>
{
    /// <summary>
    /// Gets or Sets Type
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    [XmlType("OLLAMACHAT.Generated.Models.ParseResponse.RelationshipDto.TypeEnumDto")]
    public enum TypeEnumDto
    {
        /// <summary>
        /// Enum CallsEnum for calls
        /// </summary>
        [EnumMember(Value = "calls")]
        CallsEnum = 0,

        /// <summary>
        /// Enum IsBaseForEnum for isBaseFor
        /// </summary>
        [EnumMember(Value = "isBaseFor")]
        IsBaseForEnum = 1,

        /// <summary>
        /// Enum SubscribesToEnum for subscribesTo
        /// </summary>
        [EnumMember(Value = "subscribesTo")]
        SubscribesToEnum = 2,

        /// <summary>
        /// Enum ObservedByEnum for observedBy
        /// </summary>
        [EnumMember(Value = "observedBy")]
        ObservedByEnum = 3
    }

    /// <summary>
    /// Full name of the source entity.
    /// </summary>
    /// <value>Full name of the source entity.</value>
    [Required]
    [DataMember(Name = "fullNameFrom")]
    public string FullNameFrom { get; set; }

    /// <summary>
    /// Relative path to the target file if the relationship is cross-file.
    /// </summary>
    /// <value>Relative path to the target file if the relationship is cross-file.</value>
    [DataMember(Name = "targetDefinitionFilePath")]
    public string TargetDefinitionFilePath { get; set; }

    /// <summary>
    /// Full name of the target entity.
    /// </summary>
    /// <value>Full name of the target entity.</value>
    [Required]
    [DataMember(Name = "fullNameTo")]
    public string FullNameTo { get; set; }

    /// <summary>
    /// Gets or Sets Type
    /// </summary>
    [Required]
    [DataMember(Name = "type")]
    public TypeEnumDto? Type { get; set; }

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

        return obj.GetType() == GetType() && Equals((RelationshipDto)obj);
    }

    /// <summary>
    /// Returns true if Relationship instances are equal
    /// </summary>
    /// <param name="other">Instance of Relationship to be compared</param>
    /// <returns>Boolean</returns>
    public bool Equals(RelationshipDto other)
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
                FullNameFrom == other.FullNameFrom ||
                FullNameFrom != null &&
                FullNameFrom.Equals(other.FullNameFrom)
            ) &&
            (
                FullNameTo == other.FullNameTo ||
                FullNameTo != null &&
                FullNameTo.Equals(other.FullNameTo)
            ) &&
            (
                Type == other.Type ||
                Type != null &&
                Type.Equals(other.Type)
            ) &&
            (
                TargetDefinitionFilePath == other.TargetDefinitionFilePath ||
                TargetDefinitionFilePath != null &&
                TargetDefinitionFilePath.Equals(other.TargetDefinitionFilePath)
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
            if (FullNameFrom != null)
            {
                hashCode = hashCode * 59 + FullNameFrom.GetHashCode();
            }

            if (FullNameTo != null)
            {
                hashCode = hashCode * 59 + FullNameTo.GetHashCode();
            }

            if (Type != null)
            {
                hashCode = hashCode * 59 + Type.GetHashCode();
            }

            if (TargetDefinitionFilePath != null)
            {
                hashCode = hashCode * 59 + TargetDefinitionFilePath.GetHashCode();
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
        sb.Append("class Relationship {\n");
        sb.Append("  FullNameFrom: ").Append(FullNameFrom).Append("\n");
        sb.Append("  FullNameTo: ").Append(FullNameTo).Append("\n");
        sb.Append("  Type: ").Append(Type).Append("\n");
        sb.Append("  TargetDefinitionFilePath: ").Append(TargetDefinitionFilePath).Append("\n");
        sb.Append("}\n");
        return sb.ToString();
    }

    #region Operators

#pragma warning disable 1591

    public static bool operator ==(RelationshipDto left, RelationshipDto right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(RelationshipDto left, RelationshipDto right)
    {
        return !Equals(left, right);
    }

#pragma warning restore 1591

    #endregion Operators
}
