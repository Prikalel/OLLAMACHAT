namespace OLLAMACHAT.Generated.Models.ParseResponse;

/// <summary>
/// Represents a single parsed code entity (class, method, etc.).
/// </summary>
[DataContract]
public class ParsedEntityDto
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
        [EnumMember(Value = "usingStatement")]
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
    public required List<ParsedEntityDto> Children { get; set; }

    /// <summary>
    /// Gets or Sets Attributes
    /// </summary>
    [DataMember(Name = "attributes")]
    public List<AttributeDto>? Attributes { get; set; }

    /// <summary>
    /// Gets or Sets UsingStatementData
    /// </summary>
    [DataMember(Name = "usingData")]
    public UsingStatementDataDto? UsingStatementData { get; set; }

    /// <summary>
    /// Gets or Sets Inheritance
    /// </summary>
    [DataMember(Name = "inheritance")]
    public ParsedEntityInheritanceDto? Inheritance { get; set; }

    /// <summary>
    /// Gets or Sets Location
    /// </summary>
    [Required]
    [DataMember(Name = "location")]
    public required LocationDto Location { get; set; }

    /// <summary>
    /// Модификаторы - private/static etc.
    /// </summary>
    [DataMember(Name = "modifiers")]
    public List<string>? Modifiers { get; set; }

    /// <summary>
    /// Gets or Sets SimpleName
    /// </summary>
    [Required]
    [DataMember(Name = "simpleName")]
    public required string SimpleName { get; set; }

    /// <summary>
    /// Gets or Sets Parameters
    /// </summary>
    [DataMember(Name = "parameters")]
    public List<ModelParameterDto>? Parameters { get; set; }

    /// <summary>
    /// Gets or Sets ReturnType
    /// </summary>
    [DataMember(Name = "returnType")]
    public string? ReturnType { get; set; }

    /// <summary>
    /// Gets or Sets FullName
    /// </summary>
    [DataMember(Name = "fullName")]
    public string? FullName { get; set; }

    /// <summary>
    /// Gets or Sets Type
    /// </summary>
    [Required]
    [DataMember(Name = "type")]
    public TypeEnumDto? Type { get; set; }

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
}
