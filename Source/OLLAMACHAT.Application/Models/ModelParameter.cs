namespace VelikiyPrikalel.OLLAMACHAT.Application.Models;

/// <summary>
/// Представляет параметр модели.
/// </summary>
/// <param name="Name">Имя параметра.</param>
/// <param name="FullTypeName">Полное имя типа параметра.</param>
/// <param name="Nullable">Флаг, указывающий, является ли параметр nullable.</param>
/// <param name="DefaultValue">Значение параметра по умолчанию.</param>
public record ModelParameter(string? Name, string? FullTypeName, bool? Nullable, string? DefaultValue);