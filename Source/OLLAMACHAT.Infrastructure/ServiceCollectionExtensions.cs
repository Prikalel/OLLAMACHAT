namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure;

/// <summary>
/// Extension методы <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Регистрирует Redis кэш в DI контейнере.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/>.</param>
    /// <param name="configuration"><see cref="IConfiguration"/>.</param>
    public static void AddRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RedisSettings>(options =>
            configuration.GetSection("Redis").Bind(options));

        services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            RedisSettings redisSettings = provider.GetRequiredService<IOptions<RedisSettings>>().Value;
            ConfigurationOptions configurationOptions = ConfigurationOptions.Parse(redisSettings.ConnectionString);
            configurationOptions.AbortOnConnectFail = false;
            configurationOptions.ConnectRetry = 3;

            try
            {
                return ConnectionMultiplexer.Connect(configurationOptions);
            }
            catch (Exception ex)
            {
                ILogger<ConnectionMultiplexer>? logger = provider.GetService<ILogger<ConnectionMultiplexer>>();
                logger?.LogError(ex, "Failed to connect to Redis at {ConnectionString}", redisSettings.ConnectionString);

                return null!;
            }
        });

        services.AddScoped<IDatabase>(provider =>
        {
            IConnectionMultiplexer connectionMultiplexer = provider.GetRequiredService<IConnectionMultiplexer>();
            return connectionMultiplexer.GetDatabase();
        });

        services.AddSingleton(typeof(ICacheRepository<>), typeof(CacheRepository<>));
    }

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

        services.Configure<SolutionSettings>(options =>
            configuration.GetSection("SolutionSettings").Bind(options));

        services.Configure<List<McpServerConfiguration>>(options =>
            configuration.GetSection("McpServers").Bind(options));

        // Register SolutionLoaderService as a singleton
        services.AddSingleton<ISolutionLoaderService, SolutionLoaderService>();

        services.Scan(scan => scan
            .FromAssemblyOf<LlmService>()
            .AddClasses(classes =>
                classes.InNamespaces(
                    typeof(LlmService).Namespace!,
                    typeof(RoslynParsingService).Namespace!,
                    typeof(Repository<>).Namespace!))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
    }
}
