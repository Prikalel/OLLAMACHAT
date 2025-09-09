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
        services.Configure<OpenAISettings>(options =>
            configuration.GetSection("OpenAISettings").Bind(options));

        services.Configure<List<McpServerConfiguration>>(options =>
            configuration.GetSection("McpServers").Bind(options));
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