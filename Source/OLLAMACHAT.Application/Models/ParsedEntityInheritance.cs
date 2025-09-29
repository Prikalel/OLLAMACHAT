namespace VelikiyPrikalel.OLLAMACHAT.Application.Models;

/// <summary>
/// Представляет информацию о наследовании сущности.
/// </summary>
/// <param name="DirectBaseClasses">Прямые базовые классы.</param>
/// <param name="DirectInterfaces">Прямые реализуемые интерфейсы.</param>
/// <param name="AllBaseClasses">Все базовые классы.</param>
/// <param name="AllInterfaces">Все реализуемые интерфейсы.</param>
public record ParsedEntityInheritance(
    List<string>? DirectBaseClasses,
    List<string>? DirectInterfaces,
    List<string>? AllBaseClasses,
    List<string>? AllInterfaces
);