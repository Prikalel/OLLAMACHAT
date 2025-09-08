using Microsoft.EntityFrameworkCore;
using VelikiyPrikalel.OLLAMACHAT.Infrastructure.Settings;

namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure;

/// <summary>
/// Extension методы <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Регистрирует конфиги инфраструктуры.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/>.</param>
    /// <param name="configuration">><see cref="IConfiguration"/>.</param>
    public static void RegisterInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OllamaChatContext>(opt =>
        {
            opt.UseSqlite(configuration.GetConnectionString("OLLAMACHAT"));
        });
        services.Configure<OpenAISettings>(opt =>
        {
            var r = configuration.GetRequiredSection("OpenAISettings");
            opt.ApiKey = r["ApiKey"];
            opt.ApiBase = r["ApiBase"];
            opt.EnableTools = false;
            opt.SystemChatMessage = r["SystemChatMessage"];
            opt.Models = r.GetSection("Models").GetChildren().Select(x => x.Value).OfType<string>().ToArray();
        });
        services.Scan(scan => scan
            .FromAssemblyOf<LlmService>()
            .AddClasses(classes =>
                classes.InNamespaces(
                    typeof(LlmService).Namespace,
                    typeof(Repository<>).Namespace))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
    }
}