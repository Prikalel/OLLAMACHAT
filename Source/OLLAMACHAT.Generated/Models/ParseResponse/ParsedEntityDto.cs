namespace OLLAMACHAT.Generated.Models.ParseResponse;

/// <summary>
/// Represents a single parsed code entity (class, method, etc.).
/// </summary>
[DataContract]
public class ParsedEntityDto : IEquatable<ParsedEntityDto>
{
    /// <summary>
    /// Gets or Sets Type
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    [XmlType("OLLAMACHAT.Generated.Models.ParseResponse.ParsedEntityDto.TypeEnumDto")]
    public enum TypeEnumDto
    {
        /// <summary>
        /// Enum ClassEnum for class
        /// </summary>
        [EnumMember(Value = "class")]
        ClassEnum = 0,

        /// <summary>
        /// Enum MethodEnum for method
        /// </summary>
        [EnumMember(Value = "method")]
        MethodEnum = 1,

        /// <summary>
        /// Enum InterfaceEnum for interface
        /// </summary>
        [EnumMember(Value = "interface")]
        InterfaceEnum = 2,

        /// <summary>
        /// Enum UsingStatementEnum for import
        /// </summary>
        [EnumMember(Value = "import")]
        UsingStatementEnum = 3,

        /// <summary>
        /// Enum PropertyEnum for property
        /// </summary>
        [EnumMember(Value = "property")]
        PropertyEnum = 4,

        /// <summary>
        /// Enum EnumEnum for enum
        /// </summary>
        [EnumMember(Value = "enum")]
        EnumEnum = 5,

        /// <summary>
        /// Enum StructEnum for struct
        /// </summary>
        [EnumMember(Value = "struct")]
        StructEnum = 6,

        /// <summary>
        /// Enum NamespaceEnum for namespace
        /// </summary>
        [EnumMember(Value = "namespace")]
        NamespaceEnum = 7,

        /// <summary>
        /// Enum UnityEventEnum for unityEvent
        /// </summary>
        [EnumMember(Value = "unityEvent")]
        UnityEventEnum = 8
    }

    /// <summary>
    /// Gets or Sets Children
    /// </summary>
    [DataMember(Name = "children")]
    public List<ParsedEntityDto> Children { get; set; }

    /// <summary>
    /// Gets or Sets Attributes
    /// </summary>
    [DataMember(Name = "attributes")]
    public List<AttributeDto> Attributes { get; set; }

    /// <summary>
    /// Gets or Sets UsingStatementData
    /// </summary>
    [DataMember(Name = "importData")]
    public UsingStatementDataDto UsingStatementData { get; set; }

    /// <summary>
    /// Gets or Sets Inheritance
    /// </summary>
    [DataMember(Name = "inheritance")]
    public ParsedEntityInheritanceDto Inheritance { get; set; }

    /// <summary>
    /// Gets or Sets Location
    /// </summary>
    [Required]
    [DataMember(Name = "location")]
    public LocationDto Location { get; set; }

    /// <summary>
    /// Модификаторы - private/static etc.
    /// </summary>
    [DataMember(Name = "modifiers")]
    public List<string> Modifiers { get; set; }

    /// <summary>
    /// Gets or Sets SimpleName
    /// </summary>
    [Required]
    [DataMember(Name = "simpleName")]
    public string SimpleName { get; set; }

    /// <summary>
    /// Gets or Sets Parameters
    /// </summary>
    [DataMember(Name = "parameters")]
    public List<ModelParameterDto> Parameters { get; set; }

    /// <summary>
    /// Gets or Sets ReturnType
    /// </summary>
    [DataMember(Name = "returnType")]
    public string ReturnType { get; set; }

    /// <summary>
    /// Gets or Sets FullName
    /// </summary>
    [DataMember(Name = "fullName")]
    public string FullName { get; set; }

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

        return obj.GetType() == GetType() && Equals((ParsedEntityDto)obj);
    }

    /// <summary>
    /// Returns true if ParsedEntity instances are equal
    /// </summary>
    /// <param name="other">Instance of ParsedEntity to be compared</param>
    /// <returns>Boolean</returns>
    public bool Equals(ParsedEntityDto other)
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
                SimpleName == other.SimpleName ||
                SimpleName != null &&
                SimpleName.Equals(other.SimpleName)
            ) &&
            (
                FullName == other.FullName ||
                FullName != null &&
                FullName.Equals(other.FullName)
            ) &&
            (
                Type == other.Type ||
                Type != null &&
                Type.Equals(other.Type)
            ) &&
            (
                Location == other.Location ||
                Location != null &&
                Location.Equals(other.Location)
            ) &&
            (
                Children == other.Children ||
                Children != null &&
                Children.SequenceEqual(other.Children)
            ) &&
            (
                Modifiers == other.Modifiers ||
                Modifiers != null &&
                Modifiers.SequenceEqual(other.Modifiers)
            ) &&
            (
                Attributes == other.Attributes ||
                Attributes != null &&
                Attributes.SequenceEqual(other.Attributes)
            ) &&
            (
                Inheritance == other.Inheritance ||
                Inheritance != null &&
                Inheritance.Equals(other.Inheritance)
            ) &&
            (
                ReturnType == other.ReturnType ||
                ReturnType != null &&
                ReturnType.Equals(other.ReturnType)
            ) &&
            (
                Parameters == other.Parameters ||
                Parameters != null &&
                Parameters.SequenceEqual(other.Parameters)
            ) &&
            (
                UsingStatementData == other.UsingStatementData ||
                UsingStatementData != null &&
                UsingStatementData.Equals(other.UsingStatementData)
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
            if (SimpleName != null)
            {
                hashCode = hashCode * 59 + SimpleName.GetHashCode();
            }

            if (FullName != null)
            {
                hashCode = hashCode * 59 + FullName.GetHashCode();
            }

            if (Type != null)
            {
                hashCode = hashCode * 59 + Type.GetHashCode();
            }

            if (Location != null)
            {
                hashCode = hashCode * 59 + Location.GetHashCode();
            }

            if (Children != null)
            {
                hashCode = hashCode * 59 + Children.GetHashCode();
            }

            if (Modifiers != null)
            {
                hashCode = hashCode * 59 + Modifiers.GetHashCode();
            }

            if (Attributes != null)
            {
                hashCode = hashCode * 59 + Attributes.GetHashCode();
            }

            if (Inheritance != null)
            {
                hashCode = hashCode * 59 + Inheritance.GetHashCode();
            }

            if (ReturnType != null)
            {
                hashCode = hashCode * 59 + ReturnType.GetHashCode();
            }

            if (Parameters != null)
            {
                hashCode = hashCode * 59 + Parameters.GetHashCode();
            }

            if (UsingStatementData != null)
            {
                hashCode = hashCode * 59 + UsingStatementData.GetHashCode();
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
        sb.Append("class ParsedEntity {\n");
        sb.Append("  SimpleName: ").Append(SimpleName).Append("\n");
        sb.Append("  FullName: ").Append(FullName).Append("\n");
        sb.Append("  Type: ").Append(Type).Append("\n");
        sb.Append("  Location: ").Append(Location).Append("\n");
        sb.Append("  Children: ").Append(Children).Append("\n");
        sb.Append("  Modifiers: ").Append(Modifiers).Append("\n");
        sb.Append("  Attributes: ").Append(Attributes).Append("\n");
        sb.Append("  Inheritance: ").Append(Inheritance).Append("\n");
        sb.Append("  ReturnType: ").Append(ReturnType).Append("\n");
        sb.Append("  Parameters: ").Append(Parameters).Append("\n");
        sb.Append("  UsingStatementData: ").Append(UsingStatementData).Append("\n");
        sb.Append("}\n");
        return sb.ToString();
    }

    #region Operators

#pragma warning disable 1591

    public static bool operator ==(ParsedEntityDto left, ParsedEntityDto right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ParsedEntityDto left, ParsedEntityDto right)
    {
        return !Equals(left, right);
    }

#pragma warning restore 1591

    #endregion Operators
}
