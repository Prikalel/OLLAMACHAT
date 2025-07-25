namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Hangfire;

/// <summary>
///  Extension методы <see cref="IServiceCollection"/>.
/// </summary>
internal static class HangfireServiceCollectionExtensions
{
    /// <summary>
    /// Регистрирует сервисы для Hangfire.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/>.</param>
    /// <param name="configuration"><see cref="IConfiguration"/>.</param>
    internal static void RegisterHangfire(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(c =>
        {
            c.UseInMemoryStorage();
        });

        services.AddHangfireServer(options => options.WorkerCount = 1);
    }
}
