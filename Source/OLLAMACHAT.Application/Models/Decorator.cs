namespace VelikiyPrikalel.OLLAMACHAT.Application.Models;

/// <summary>
/// Представляет декоратор в коде.
/// </summary>
/// <param name="Name">Имя декоратора.</param>
/// <param name="Arguments">Аргументы декоратора.</param>
public record Decorator(string? Name, List<string>? Arguments);
