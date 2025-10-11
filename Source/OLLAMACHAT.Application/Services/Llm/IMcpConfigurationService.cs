namespace VelikiyPrikalel.OLLAMACHAT.Application.Services.Llm;

/// <summary>
/// Интерфейс настроек mcp серверов.
/// </summary>
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

/// <summary>
/// Информация о mcp сервере.
/// </summary>
/// <param name="Name">Имя.</param>
/// <param name="Url">Путь (если sse).</param>
/// <param name="Type">Тип.</param>
/// <param name="AuthToken">Токен (при необходимости авторизации).</param>
public record McpServerInfo(string Name, string Url, string Type, string? AuthToken);
