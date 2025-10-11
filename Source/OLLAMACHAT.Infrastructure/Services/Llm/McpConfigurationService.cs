namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Services.Llm;

/// <inheritdoc />
public class McpConfigurationService : IMcpConfigurationService
{
    private readonly List<McpServerConfiguration> servers;
    private readonly IConfiguration configuration;

    /// <summary>
    /// ctor.
    /// </summary>
    /// <param name="configuration"><see cref="IConfiguration"/>.</param>
    public McpConfigurationService(IConfiguration configuration)
    {
        this.configuration = configuration;
        servers = new List<McpServerConfiguration>();
        configuration
            .GetSection("McpServers")
            .Bind(servers);
    }

    /// <inheritdoc />
    public IEnumerable<McpServerInfo> GetAllServers()
    {
        return servers.Select(s => new McpServerInfo(s.Name, s.Url, s.Type, s.AuthToken));
    }

    /// <inheritdoc />
    public McpServerInfo? GetServerByName(string name)
    {
        McpServerConfiguration? server = servers.FirstOrDefault(s => s.Name.Replace(' ', '_').Equals(name, StringComparison.OrdinalIgnoreCase));
        return server != null ? new McpServerInfo(server.Name, server.Url, server.Type, server.AuthToken) : null;
    }
}
