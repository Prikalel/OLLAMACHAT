using Hangfire;
using Hangfire.SQLite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            // string connectionString = configuration.GetConnectionString("OLLAMACHAT"); // This line can be removed or commented out if not used
            c.UseSQLiteStorage("hangfire.db"); // Ensure this is the new line
        });

        services.AddHangfireServer();
    }
}