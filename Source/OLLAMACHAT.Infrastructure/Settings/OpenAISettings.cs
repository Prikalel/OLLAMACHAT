namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Settings;

/// <summary>
/// Настройки подключения к большой языковой модели.
/// </summary>
public class OpenAISettings
{
    /// <summary>
    /// Ключ.
    /// </summary>
    public string ApiKey { get; set; } = "";

    /// <summary>
    /// Список поддерживаемых моделей.
    /// </summary>
    public string[] Models { get; set; } = [];

    /// <summary>
    /// Путь.
    /// </summary>
    public string ApiBase { get; set; } = "https://openrouter.ai/api/v1";

    /// <summary>
    /// Системный промпт.
    /// </summary>
    public string? SystemChatMessage { get; set; } = "ты умный ассистент";

    /// <summary>
    /// Выключить использование mcp серверов и проч.
    /// </summary>
    public bool EnableTools { get; set; } = false;
}
