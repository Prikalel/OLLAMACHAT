namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Services;

/// <inheritdoc />
public class McpConfigurationService : IMcpConfigurationService
{
    private readonly List<McpServerConfiguration> _servers;
    private readonly IConfiguration _configuration;

    public McpConfigurationService(IConfiguration configuration)
    {
        _configuration = configuration;
        _servers = new List<McpServerConfiguration>();
        configuration
            .GetSection("McpServers")
            .Bind(_servers);
    }

    /// <inheritdoc />
    public IEnumerable<McpServerInfo> GetAllServers()
    {
        return _servers.Select(s => new McpServerInfo(s.Name, s.Url, s.Type, s.AuthToken));
    }

    /// <inheritdoc />
    public McpServerInfo? GetServerByName(string name)
    {
        McpServerConfiguration? server = _servers.FirstOrDefault(s => s.Name.Replace(' ', '_').Equals(name, StringComparison.OrdinalIgnoreCase));
        return server != null ? new McpServerInfo(server.Name, server.Url, server.Type, server.AuthToken) : null;
    }
}