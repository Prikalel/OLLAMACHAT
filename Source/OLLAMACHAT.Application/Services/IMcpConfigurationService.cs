namespace VelikiyPrikalel.OLLAMACHAT.Application.Services;

public interface IMcpConfigurationService
{
    /// <summary>
    /// Получает все конфигурации MCP серверов.
    /// </summary>
    /// <returns>Коллекция информации о MCP серверах.</returns>
    IEnumerable<McpServerInfo> GetAllServers();

    /// <summary>
    /// Получает конфигурацию MCP сервера по имени.
    /// </summary>
    /// <param name="name">Имя сервера.</param>
    /// <returns>Информация о MCP сервере, или null, если не найдено.</returns>
    McpServerInfo? GetServerByName(string name);
}

public record McpServerInfo(string Name, string Url, string Type, string? AuthToken);