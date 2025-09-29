namespace OLLAMACHAT.Generated.Models.ParseResponse;

/// <summary>
/// </summary>
[DataContract]
public class ModelParameterDto : IEquatable<ModelParameterDto>
{
    /// <summary>
    /// Gets or Sets DefaultValue
    /// </summary>
    [DataMember(Name = "defaultValue")]
    public string DefaultValue { get; set; }

    /// <summary>
    /// Gets or Sets Name
    /// </summary>
    [DataMember(Name = "name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or Sets Nullable
    /// </summary>
    [DataMember(Name = "nullable")]
    public bool? Nullable { get; set; }

    /// <summary>
    /// Gets or Sets FullTypeName
    /// </summary>
    [DataMember(Name = "fullTypeName")]
    public string FullTypeName { get; set; }

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

        return obj.GetType() == GetType() && Equals((ModelParameterDto)obj);
    }

    /// <summary>
    /// Returns true if ModelParameter instances are equal
    /// </summary>
    /// <param name="other">Instance of ModelParameter to be compared</param>
    /// <returns>Boolean</returns>
    public bool Equals(ModelParameterDto other)
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
                FullTypeName == other.FullTypeName ||
                FullTypeName != null &&
                FullTypeName.Equals(other.FullTypeName)
            ) &&
            (
                Nullable == other.Nullable ||
                Nullable != null &&
                Nullable.Equals(other.Nullable)
            ) &&
            (
                DefaultValue == other.DefaultValue ||
                DefaultValue != null &&
                DefaultValue.Equals(other.DefaultValue)
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

            if (FullTypeName != null)
            {
                hashCode = hashCode * 59 + FullTypeName.GetHashCode();
            }

            if (Nullable != null)
            {
                hashCode = hashCode * 59 + Nullable.GetHashCode();
            }

            if (DefaultValue != null)
            {
                hashCode = hashCode * 59 + DefaultValue.GetHashCode();
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
        sb.Append("class ModelParameter {\n");
        sb.Append("  Name: ").Append(Name).Append("\n");
        sb.Append("  FullTypeName: ").Append(FullTypeName).Append("\n");
        sb.Append("  Nullable: ").Append(Nullable).Append("\n");
        sb.Append("  DefaultValue: ").Append(DefaultValue).Append("\n");
        sb.Append("}\n");
        return sb.ToString();
    }

    #region Operators

#pragma warning disable 1591

    public static bool operator ==(ModelParameterDto left, ModelParameterDto right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ModelParameterDto left, ModelParameterDto right)
    {
        return !Equals(left, right);
    }

#pragma warning restore 1591

    #endregion Operators
}
