namespace VelikiyPrikalel.OLLAMACHAT.Web;

/// <summary>
/// Входная точка запуска веб сервиса.
/// </summary>
public class Program
{
    /// <summary>
    /// Собирает хост веб приложения.
    /// </summary>
    /// <param name="args">Аргументы командной строки.</param>
    /// <returns>Подготовленный строитель веб приложения.</returns>
    public static IHostBuilder CreateWebHostBuilder(string[] args)
    {
        return Host
            .CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(builder =>
            {
                builder
                    .UseIISIntegration()
                    .ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        IWebHostEnvironment env = hostingContext.HostingEnvironment;
                        config
                            .AddJsonFile("appsettings.json", false, true)
                            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
                            .AddJsonFile("/config/appsettings.json", true, true)
                            .AddEnvironmentVariables("OLLAMACHAT_Backend_");
                    })
                    .UseStartup<Startup>();
            })
            .ConfigureLogging((context, logging) =>
            {
                logging.ClearProviders();
                logging.AddConfiguration(context.Configuration);
                logging.SetMinimumLevel(LogLevel.Trace);
            })
            .UseNLog();
    }

    /// <summary>
    /// Входная точка программы.
    /// </summary>
    /// <param name="args">Аргументы командной строки.</param>
    /// <returns><see cref="Task"/>.</returns>
    public static async Task Main(string[] args)
    {
        Logger logger = NLogBuilder
            .ConfigureNLog("nlog.config")
            .GetCurrentClassLogger();

        try
        {
            logger.Debug("Starting up webhost.");
            IHost build = CreateWebHostBuilder(args)
                .Build();

            using (IServiceScope scope = build.Services.CreateScope())
            {
                OllamaChatContext db = scope.ServiceProvider.GetRequiredService<OllamaChatContext>();
                await db.Database.MigrateAsync();
            }

            await build
                .RunAsync();
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Stopped service due to exception");
            throw;
        }
        finally
        {
            LogManager.Shutdown();
        }
    }
}