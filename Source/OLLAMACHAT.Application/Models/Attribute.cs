namespace VelikiyPrikalel.OLLAMACHAT.Application.Models;

/// <summary>
/// Представляет атрибут в коде.
/// </summary>
/// <param name="Name">Имя атрибута.</param>
/// <param name="Arguments">Аргументы атрибута.</param>
public record Attribute(string? Name, List<string>? Arguments);