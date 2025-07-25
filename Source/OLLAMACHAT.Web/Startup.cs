namespace VelikiyPrikalel.OLLAMACHAT.Web;

/// <summary>
/// Класс конфигурации веб-приложения.
/// </summary>
public class Startup
{
    private const string ServiceName = "OLLAMACHAT";

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="configuration">Конфигурация.</param>
    /// <param name="env">Параметры окружения.</param>
    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        Configuration = configuration;
        Environment = env;
    }

    /// <summary>
    /// Конфигурация приложения.
    /// </summary>
    public IConfiguration Configuration { get; }

    /// <summary>
    /// Окружение.
    /// </summary>
    public IWebHostEnvironment Environment { get; }

    private ServicesRegistrationSettings ServicesRegistrationOptions =>
        new ()
        {
            RegisterSwagger = true,
            RegisterHangfireDashboard = true
        };

    /// <summary>
    /// Конфигурирует pipeline запросов.
    /// </summary>
    /// <param name="app">Строитель приложения.</param>
    /// <param name="env">Параметры окружения.</param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseDeveloperExceptionPage();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapRazorPages();

            endpoints.AddMinimalApis();
        });

        if (ServicesRegistrationOptions.RegisterHangfireDashboard)
        {
            app.UseHangfireDashboard();
        }

        if (ServicesRegistrationOptions.RegisterSwagger)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{ServiceName} API v1");
            });
        }
    }

    /// <summary>
    /// Конфигурирует все сервисы приложения.
    /// </summary>
    /// <param name="services">Коллекция сервисов.</param>
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHostedService<UserChatBackgroundService>();
        services.RegisterInfrastructure(Configuration);

        if (ServicesRegistrationOptions.RegisterSwagger)
        {
            RegisterSwagger(services);
        }
        services.AddControllers();
        services.AddRazorPages();

        services.AddMediator((MediatorOptions options) =>
            options.ServiceLifetime = ServiceLifetime.Scoped);
    }

    private static void RegisterSwagger(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            IncludeOllamaChatXmlDocs(options, nameof(Web));

            options.SupportNonNullableReferenceTypes();
            options.UseAllOfForInheritance();

            options.EnableAnnotations();

            //options.
        });
    }

    private static void IncludeOllamaChatXmlDocs(SwaggerGenOptions swaggerGenOptions, string projectName)
    {
        string xmlDocsPath = Path.Combine(AppContext.BaseDirectory, $"{nameof(VelikiyPrikalel)}.{nameof(OLLAMACHAT)}.{projectName}.xml");
        swaggerGenOptions.IncludeXmlComments(xmlDocsPath);
    }
}