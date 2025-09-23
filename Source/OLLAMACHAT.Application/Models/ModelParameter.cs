namespace VelikiyPrikalel.OLLAMACHAT.Application.Models;

/// <summary>
/// Представляет параметр модели.
/// </summary>
/// <param name="Name">Имя параметра.</param>
/// <param name="Type">Тип параметра.</param>
/// <param name="Optional">Флаг, указывающий, является ли параметр необязательным.</param>
/// <param name="DefaultValue">Значение параметра по умолчанию.</param>
public record ModelParameter(string? Name, string? Type, bool? Optional, string? DefaultValue);