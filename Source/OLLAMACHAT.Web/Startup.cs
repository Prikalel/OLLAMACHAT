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
            RegisterSwagger = true
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
            endpoints.MapHub<ChatHub>("/chatHub");
        });

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
        services.AddHostedService<SolutionInitializationHostedService>();
        services.RegisterInfrastructure(Configuration);

        if (ServicesRegistrationOptions.RegisterSwagger)
        {
            RegisterSwagger(services);
        }
        services.AddControllers()
            .AddApplicationPart(typeof(CParserApiController).Assembly);
        services.AddRazorPages().AddMvcOptions(options =>
        {
            options.InputFormatters.RemoveType<Microsoft.AspNetCore.Mvc.Formatters.SystemTextJsonInputFormatter>();
            options.OutputFormatters.RemoveType<Microsoft.AspNetCore.Mvc.Formatters.SystemTextJsonOutputFormatter>();
        }).AddNewtonsoftJson(opts =>
        {
            opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            opts.SerializerSettings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
        })
        .AddXmlSerializerFormatters();
        services.AddSignalR();

        services.AddMediator((MediatorOptions options) =>
            options.ServiceLifetime = ServiceLifetime.Scoped);

        services.AddMapster();

        // добавляем сгенерированный маппер
        services.AddScoped<IMapperInterface, MapperInterface>();
    }

    private static void RegisterSwagger(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            IncludeOllamaChatXmlDocs(c, nameof(Web));

            c.SupportNonNullableReferenceTypes();
            c.UseAllOfForInheritance();

            c.EnableAnnotations();

            c.CustomSchemaIds(type => type.FullName!.Replace('+', '.'));
            c.IncludeXmlComments($"{AppContext.BaseDirectory}{Path.DirectorySeparatorChar}OLLAMACHAT.Generated.xml");

            // Include DataAnnotation attributes on Controller Action parameters as Swagger validation rules (e.g required, pattern, ..)
            // Use [ValidateModelState] on Actions to actually validate it in C# as well!
            c.OperationFilter<GeneratePathParamsValidationFilter>();
        });
    }

    private static void IncludeOllamaChatXmlDocs(SwaggerGenOptions swaggerGenOptions, string projectName)
    {
        string xmlDocsPath = Path.Combine(AppContext.BaseDirectory, $"{nameof(VelikiyPrikalel)}.{nameof(OLLAMACHAT)}.{projectName}.xml");
        swaggerGenOptions.IncludeXmlComments(xmlDocsPath);
    }
}
