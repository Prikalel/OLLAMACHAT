namespace VelikiyPrikalel.OLLAMACHAT.Application.Models;

/// <summary>
/// Представляет информацию о наследовании сущности.
/// </summary>
/// <param name="BaseClasses">Базовые классы.</param>
/// <param name="Interfaces">Реализуемые интерфейсы.</param>
public record ParsedEntityInheritance(List<string>? BaseClasses, List<string>? Interfaces);