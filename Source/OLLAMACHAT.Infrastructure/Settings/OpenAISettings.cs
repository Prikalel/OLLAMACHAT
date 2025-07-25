namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Settings;

public class OpenAISettings
{
    public string ApiKey { get; set; } = "";
    public string[] Models { get; set; } = [];
    public string ApiBase { get; set; } = "https://openrouter.ai/api/v1";
    public string? SystemChatMessage { get; set; } = "ты умный ассистент";
    public bool EnableTools { get; set; } = false;
}
