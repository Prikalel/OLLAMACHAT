namespace VelikiyPrikalel.OLLAMACHAT.Web.HostedServices;

/// <summary>
/// Сервис управления загрузкой решения.
/// </summary>
public class SolutionInitializationHostedService(
    IServiceProvider serviceProvider,
    ILogger<SolutionInitializationHostedService> logger) : IHostedService
{
    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using IServiceScope scope = serviceProvider.CreateScope();

        OllamaChatContext db = scope.ServiceProvider.GetRequiredService<OllamaChatContext>();
        await db.Database.MigrateAsync(cancellationToken);

        ISolutionLoaderService loader = scope.ServiceProvider.GetRequiredService<ISolutionLoaderService>();
        await loader.LoadSolutionAsync();

        if (loader.IsSolutionLoaded)
        {
            await RelationshipService.InitializeCaches(loader.CurrentSolution!);

            loader.SolutionReloaded += OnSolutionReloaded;
            logger.LogInformation("Done registering subscribers");
        }
        else
        {
            logger.LogError("Error loading solution");
        }
    }

    private async void OnSolutionReloaded(object? sender, SolutionReloadedEventArgs args)
    {
        using IServiceScope scope = serviceProvider.CreateScope();

        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        EntityService.ClearCache();
        await RelationshipService.InitializeCaches(args.Solution);

        logger.LogInformation("Starting UnityEvent data population...");
        PopulateUnityEventData.Response result = await mediator.Send(new PopulateUnityEventData.Command());
        logger.LogInformation(
            "UnityEvent data population completed. Processed {ProcessedFiles} files, found {UnityEvents} UnityEvents, {Handlers} handlers, created {Relationships} relationships",
            result.ProcessedFiles, result.UnityEvents, result.Handlers, result.Relationships);
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
