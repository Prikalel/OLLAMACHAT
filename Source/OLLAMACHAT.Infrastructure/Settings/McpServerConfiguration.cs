namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Settings;

/// <summary>
/// Конфигурация mcp серверов.
/// </summary>
public class McpServerConfiguration
{
    /// <summary>
    /// Имя (должно быть без пробелов.)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Путь (если sse).
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Тип.
    /// </summary>
    public string Type { get; set; } = "SSE";

    /// <summary>
    /// Токен авторизации.
    /// </summary>
    public string? AuthToken { get; set; }
}
