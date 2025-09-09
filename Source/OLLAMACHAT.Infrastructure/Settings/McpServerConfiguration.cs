namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Settings;

public class McpServerConfiguration
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Type { get; set; } = "SSE";
    public string? AuthToken { get; set; }
}