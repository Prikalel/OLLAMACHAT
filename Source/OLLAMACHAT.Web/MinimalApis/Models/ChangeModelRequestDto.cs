namespace VelikiyPrikalel.OLLAMACHAT.Web.MinimalApis.Models;

/// <summary>
/// Поменять текущую модель чата.
/// </summary>
/// <param name="Model">Новая модель для обработки сообщений пользователя.</param>
public record ChangeModelRequestDto(string Model);
