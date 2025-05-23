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
        services.RegisterHangfire(configuration);
        services.AddDbContext<OllamaChatContext>(opt =>
        {
            opt.UseNpgsql(configuration.GetConnectionString("OLLAMACHAT"));
        });
        services.Configure<LlmSettings>(
            configuration.GetSection(nameof(LlmSettings)));
        services.Scan(scan => scan
            .FromAssemblyOf<LlmService>()
            .AddClasses(classes =>
                classes.InNamespaces(typeof(LlmService).Namespace))
            .AsImplementedInterfaces()
            .WithSingletonLifetime());
    }
}