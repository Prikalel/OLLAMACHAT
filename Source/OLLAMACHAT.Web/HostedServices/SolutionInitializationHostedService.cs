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
            PopulateUnityEventData.Response result = await PopulateUnityEventData();
            logger.LogInformation("Done registering subscribers. Populated database in {s}s", result.ExecutionTime.TotalMilliseconds / 1000);
        }
        else
        {
            logger.LogError("Error loading solution");
        }
    }

    private async void OnSolutionReloaded(object? sender, SolutionReloadedEventArgs args)
    {
        EntityService.ClearCache();
        await RelationshipService.InitializeCaches(args.Solution);

        logger.LogInformation("Starting UnityEvent data population...");

        PopulateUnityEventData.Response result = await PopulateUnityEventData();
        logger.LogInformation(
            "UnityEvent data population completed. Processed {ProcessedFiles} files, found {UnityEvents} UnityEvents, {Handlers} handlers, created {Relationships} relationships",
            result.ProcessedFiles, result.UnityEvents, result.Handlers, result.Relationships);
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task<PopulateUnityEventData.Response> PopulateUnityEventData()
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        PopulateUnityEventData.Response result = await mediator.Send(new PopulateUnityEventData.Command());
        return result;
    }
}
