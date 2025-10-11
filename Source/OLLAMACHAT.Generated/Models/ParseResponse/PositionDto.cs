namespace OLLAMACHAT.Generated.Models.ParseResponse;

/// <summary>
/// </summary>
[DataContract]
public class PositionDto
{
    /// <summary>
    /// 0-based column position.
    /// </summary>
    /// <value>0-based column position.</value>
    [DataMember(Name = "column")]
    public int? Column { get; set; }

    /// <summary>
    /// 0-based character index from the start of the file.
    /// </summary>
    /// <value>0-based character index from the start of the file.</value>
    [DataMember(Name = "index")]
    public int? Index { get; set; }

    /// <summary>
    /// 1-based line number.
    /// </summary>
    /// <value>1-based line number.</value>
    [DataMember(Name = "line")]
    public int? Line { get; set; }

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
        sb.Append("class Position {\n");
        sb.Append("  Line: ").Append(Line).Append("\n");
        sb.Append("  Column: ").Append(Column).Append("\n");
        sb.Append("  Index: ").Append(Index).Append("\n");
        sb.Append("}\n");
        return sb.ToString();
    }
}
