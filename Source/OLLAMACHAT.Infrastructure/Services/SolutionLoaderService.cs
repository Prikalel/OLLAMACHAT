using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Services;

/// <inheritdoc />
public class SolutionLoaderService : ISolutionLoaderService, IDisposable
{
    private readonly ILogger<SolutionLoaderService> logger;
    private readonly IOptions<SolutionSettings> solutionSettings;
    private readonly SemaphoreSlim semaphore = new(1, 1);
    private readonly object debounceLock = new object();
    private MSBuildWorkspace? workspace;
    private Solution? currentSolution;
    private FileSystemWatcher? fileWatcher;
    private CancellationTokenSource? debounceCts;
    private DateTime lastFileChangeTime = DateTime.MinValue;
    private readonly TimeSpan fileChangeCooldown = TimeSpan.FromMilliseconds(100);
    private bool disposed;

    /// <inheritdoc />
    public Solution? CurrentSolution => currentSolution;

    /// <inheritdoc />
    public bool IsSolutionLoaded => currentSolution != null;

    /// <inheritdoc />
    public event EventHandler<SolutionReloadedEventArgs>? SolutionReloaded;

    /// <summary>
    /// Initializes a new instance of the <see cref="SolutionLoaderService"/> class.
    /// </summary>
    /// <param name="solutionSettings">The solution settings.</param>
    /// <param name="logger">The logger.</param>
    public SolutionLoaderService(IOptions<SolutionSettings> solutionSettings, ILogger<SolutionLoaderService> logger)
    {
        this.solutionSettings = solutionSettings ?? throw new ArgumentNullException(nameof(solutionSettings));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task LoadSolutionAsync()
    {
        if (disposed) throw new ObjectDisposedException(nameof(SolutionLoaderService));

        await semaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            if (IsSolutionLoaded)
            {
                logger.LogInformation("Solution is already loaded.");
                return;
            }

            string solutionPath = solutionSettings.Value.SolutionFilePath;
            if (string.IsNullOrWhiteSpace(solutionPath))
            {
                throw new InvalidOperationException("Solution file path is not configured.");
            }

            if (!File.Exists(solutionPath))
            {
                throw new FileNotFoundException($"Solution file not found: {solutionPath}");
            }

            logger.LogInformation("Loading solution from {SolutionPath}", solutionPath);

            workspace = MSBuildWorkspace.Create();
            workspace.WorkspaceFailed += OnWorkspaceFailed;

            currentSolution = await workspace.OpenSolutionAsync(solutionPath).ConfigureAwait(false);
            logger.LogInformation("Solution loaded successfully with {ProjectCount} projects", currentSolution.Projects.Count());

            SetupFileWatcher(Path.GetDirectoryName(solutionPath));
            OnSolutionReloaded(currentSolution);
        }
        finally
        {
            semaphore.Release();
        }
    }

    /// <inheritdoc />
    public async Task ReloadSolutionAsync()
    {
        if (disposed) throw new ObjectDisposedException(nameof(SolutionLoaderService));

        await semaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            if (!IsSolutionLoaded)
            {
                logger.LogWarning("No solution is currently loaded. Loading solution instead.");
                await LoadSolutionAsync().ConfigureAwait(false);
                return;
            }

            string solutionPath = solutionSettings.Value.SolutionFilePath;
            if (string.IsNullOrWhiteSpace(solutionPath))
            {
                throw new InvalidOperationException("Solution file path is not configured.");
            }

            logger.LogInformation("Reloading solution from {SolutionPath}", solutionPath);

            if (workspace != null)
            {
                currentSolution = await workspace.OpenSolutionAsync(solutionPath).ConfigureAwait(false);
                logger.LogInformation("Solution reloaded successfully with {ProjectCount} projects", currentSolution.Projects.Count());
                OnSolutionReloaded(currentSolution);
            }
            else
            {
                // If workspace is null, we need to create a new one
                await LoadSolutionAsync().ConfigureAwait(false);
            }
        }
        finally
        {
            semaphore.Release();
        }
    }

    /// <inheritdoc />
    public IEnumerable<Project> GetProjects()
    {
        if (disposed) throw new ObjectDisposedException(nameof(SolutionLoaderService));

        if (!IsSolutionLoaded || currentSolution == null)
        {
            logger.LogWarning("No solution is loaded. Cannot get projects.");
            return Enumerable.Empty<Project>();
        }

        return currentSolution.Projects;
    }

    /// <inheritdoc />
    public Project? GetProjectByName(string projectName)
    {
        if (disposed) throw new ObjectDisposedException(nameof(SolutionLoaderService));

        if (!IsSolutionLoaded || currentSolution == null)
        {
            logger.LogWarning("No solution is loaded. Cannot get project by name.");
            return null;
        }

        return currentSolution.Projects.FirstOrDefault(p => p.Name.Equals(projectName, StringComparison.OrdinalIgnoreCase));
    }

    /// <inheritdoc />
    public IEnumerable<Document> GetProjectDocuments(string projectName)
    {
        if (disposed) throw new ObjectDisposedException(nameof(SolutionLoaderService));

        var project = GetProjectByName(projectName);
        return project?.Documents ?? Enumerable.Empty<Document>();
    }

    private void SetupFileWatcher(string? directoryPath)
    {
        if (string.IsNullOrWhiteSpace(directoryPath))
        {
            logger.LogWarning("Cannot setup file watcher: directory path is null or empty.");
            return;
        }

        try
        {
            fileWatcher?.Dispose();
            fileWatcher = new FileSystemWatcher(directoryPath)
            {
                IncludeSubdirectories = true,
                EnableRaisingEvents = true,
                Filter = "*.cs"
            };

            fileWatcher.Changed += OnFileChanged;
            fileWatcher.Created += OnFileChanged;
            fileWatcher.Deleted += OnFileChanged;
            fileWatcher.Renamed += OnFileRenamed;

            logger.LogInformation("File watcher setup for directory: {DirectoryPath}", directoryPath);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to setup file watcher for directory: {DirectoryPath}", directoryPath);
        }
    }

    private async void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        var now = DateTime.Now;
        if (now - lastFileChangeTime < fileChangeCooldown)
            return;

        lastFileChangeTime = now;

        try
        {
            logger.LogInformation("Detected file change: {ChangeType} - {FullPath}", e.ChangeType, e.FullPath);

            // Run in background thread to avoid blocking UI thread
            await DebouncedReloadAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling file change event");
        }
    }

    private async void OnFileRenamed(object sender, RenamedEventArgs e)
    {
        var now = DateTime.Now;
        if (now - lastFileChangeTime < fileChangeCooldown)
            return;

        lastFileChangeTime = now;

        try
        {
            logger.LogInformation("Detected file rename: {OldPath} -> {NewPath}", e.OldFullPath, e.FullPath);

            // Run in background thread to avoid blocking UI thread
            await Task.Run(async () => await DebouncedReloadAsync().ConfigureAwait(false));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling file rename event");
        }
    }

    private async Task DebouncedReloadAsync()
    {
        CancellationTokenSource? localCts;

        lock (debounceLock)
        {
            debounceCts?.Cancel();
            debounceCts = new CancellationTokenSource();
            localCts = debounceCts;
        }

        try
        {
            // Wait for 1 second before reloading (debounce period)
            await Task.Delay(1000, localCts.Token).ConfigureAwait(false);

            // Only reload if not cancelled and solution is loaded
            if (!localCts.Token.IsCancellationRequested && IsSolutionLoaded)
            {
                // Retry logic for temporarily locked files
                await ReloadWithRetryAsync(localCts.Token).ConfigureAwait(false);
            }
        }
        catch (TaskCanceledException)
        {
            // Ignore cancellation - this is normal during debouncing
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to reload solution after file change");
        }
    }

    private async Task ReloadWithRetryAsync(CancellationToken cancellationToken)
    {
        const int maxRetries = 3;
        const int retryDelay = 500;

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                await ReloadSolutionAsync().ConfigureAwait(false);
                return;
            }
            catch (IOException ex) when (attempt < maxRetries)
            {
                logger.LogWarning(ex, "Attempt {Attempt} failed due to IO error, retrying...", attempt);
                await Task.Delay(retryDelay, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    private void OnWorkspaceFailed(object sender, WorkspaceDiagnosticEventArgs e)
    {
        logger.LogWarning("Workspace diagnostic: {Kind} - {Message}", e.Diagnostic.Kind, e.Diagnostic.Message);
    }

    private void OnSolutionReloaded(Solution solution)
    {
        SolutionReloaded?.Invoke(this, new SolutionReloadedEventArgs(solution));
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="SolutionLoaderService"/> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                // Unsubscribe from file watcher events
                if (fileWatcher != null)
                {
                    fileWatcher.Changed -= OnFileChanged;
                    fileWatcher.Created -= OnFileChanged;
                    fileWatcher.Deleted -= OnFileChanged;
                    fileWatcher.Renamed -= OnFileRenamed;
                    fileWatcher.Dispose();
                }

                // Unsubscribe from workspace events
                if (workspace != null)
                {
                    workspace.WorkspaceFailed -= OnWorkspaceFailed;
                    workspace.Dispose();
                }

                // Cancel and dispose debounce token source
                debounceCts?.Cancel();
                debounceCts?.Dispose();

                // Dispose semaphore
                semaphore.Dispose();
            }

            disposed = true;
        }
    }
}