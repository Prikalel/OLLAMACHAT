namespace VelikiyPrikalel.OLLAMACHAT.Web.Settings;

/// <summary>
/// Настройка регистрации сервисов.
/// </summary>
public class ServicesRegistrationSettings
{
    /// <summary>
    /// Использовать SwaggerUi.
    /// </summary>
    public bool RegisterSwagger { get; set; } = false;
    
    /// <summary>
    /// Использовать hangfire dashboard.
    /// </summary>
    public bool RegisterHangfireDashboard { get; set; } = false;
}
