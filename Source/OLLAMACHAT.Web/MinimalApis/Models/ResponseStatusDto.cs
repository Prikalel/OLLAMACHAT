namespace VelikiyPrikalel.OLLAMACHAT.Web.MinimalApis.Models;

/// <summary>
/// Результат операции с чатом.
/// </summary>
/// <param name="Status">Статус. Если "ready" - можно забирать.</param>
/// <param name="Response">Результат.</param>
public record ResponseStatusDto(string Status, ResponseContentDto? Response = null);