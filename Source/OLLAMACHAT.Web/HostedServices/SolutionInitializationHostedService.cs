namespace VelikiyPrikalel.OLLAMACHAT.Web.HostedServices;

/// <summary>
/// Сервис управления загрузкой решения.
/// </summary>
public class SolutionInitializationHostedService(
    ISolutionLoaderService loader,
    IServiceProvider serviceProvider,
    ILogger<SolutionInitializationHostedService> logger) : IHostedService
{
    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            OllamaChatContext db = scope.ServiceProvider.GetRequiredService<OllamaChatContext>();
            await db.Database.MigrateAsync(cancellationToken);
        }

        await loader.LoadSolutionAsync();

        if (loader.IsSolutionLoaded)
        {
            await OnSolutionReloaded(loader.CurrentSolution!);

            loader.SolutionReloaded += async (obj, args) => await OnSolutionReloaded(args.Solution);
        }
        else
        {
            logger.LogError("Error loading solution");
        }
    }

    private async Task OnSolutionReloaded(Solution solution)
    {
        logger.LogInformation("Caches restarted...");

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IEntityService entityService = scope.ServiceProvider.GetRequiredService<IEntityService>();
            entityService.ClearCache();
            IRelationshipService relationshipService = scope.ServiceProvider.GetRequiredService<IRelationshipService>();
            await relationshipService.InitializeCaches(solution);
        }

        logger.LogInformation("Caches ended...");
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
