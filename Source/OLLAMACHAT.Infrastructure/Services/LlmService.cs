namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Services;

/// <inheritdoc />
public class LlmService : ILlmService
{
    private readonly ILogger<LlmService> logger;
    private readonly OllamaApiClient client;

    /// <summary>
    /// ctor.
    /// </summary>
    /// <param name="options">Опции.</param>
    /// <param name="logger">Логгер..</param>
    public LlmService(IOptions<LlmSettings> options, ILogger<LlmService> logger)
    {
        this.logger = logger;
        client = new(options.Value.OllamaServer ?? throw new ArgumentNullException(nameof(options.Value.OllamaServer)));
    }

    /// <inheritdoc />
    public async Task<bool> IsServerAlive() => await client.IsRunningAsync();

    /// <inheritdoc />
    public async Task<IEnumerable<string>> ListLocalModelsAsync()
    {
        IEnumerable<Model> models = await client.ListLocalModelsAsync();
        logger.LogInformation("Found {Count} models on server", models.Count());
        return models.Select(x => x.Name);
    }
}
