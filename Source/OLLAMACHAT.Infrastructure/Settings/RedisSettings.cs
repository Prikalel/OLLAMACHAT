namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Settings;

/// <summary>
/// Настройки подключения к Redis.
/// </summary>
public class RedisSettings
{
    /// <summary>
    /// Строка подключения к Redis.
    /// </summary>
    public string ConnectionString { get; set; } = "localhost:6379";
}
