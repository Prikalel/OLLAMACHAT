using Microsoft.CodeAnalysis;

namespace VelikiyPrikalel.OLLAMACHAT.Application;

/// <summary>
/// Service for loading and monitoring .NET solution files.
/// </summary>
public interface ISolutionLoaderService
{
    /// <summary>
    /// Gets the currently loaded solution.
    /// </summary>
    Solution? CurrentSolution { get; }

    /// <summary>
    /// Gets a value indicating whether a solution is currently loaded.
    /// </summary>
    bool IsSolutionLoaded { get; }

    /// <summary>
    /// Loads the solution file specified in the configuration.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task LoadSolutionAsync();

    /// <summary>
    /// Reloads the solution file.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ReloadSolutionAsync();

    /// <summary>
    /// Gets all projects in the solution.
    /// </summary>
    /// <returns>A collection of projects in the solution.</returns>
    IEnumerable<Project> GetProjects();

    /// <summary>
    /// Gets a project by its name.
    /// </summary>
    /// <param name="projectName">The name of the project to retrieve.</param>
    /// <returns>The project if found; otherwise, null.</returns>
    Project? GetProjectByName(string projectName);

    /// <summary>
    /// Gets all documents in a project.
    /// </summary>
    /// <param name="projectName">The name of the project.</param>
    /// <returns>A collection of documents in the project.</returns>
    IEnumerable<Document> GetProjectDocuments(string projectName);

    /// <summary>
    /// Event raised when the solution is reloaded.
    /// </summary>
    event EventHandler<SolutionReloadedEventArgs>? SolutionReloaded;
}

/// <summary>
/// Event arguments for solution reloaded event.
/// </summary>
public class SolutionReloadedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the reloaded solution.
    /// </summary>
    public Solution Solution { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SolutionReloadedEventArgs"/> class.
    /// </summary>
    /// <param name="solution">The reloaded solution.</param>
    public SolutionReloadedEventArgs(Solution solution)
    {
        Solution = solution ?? throw new ArgumentNullException(nameof(solution));
    }
}