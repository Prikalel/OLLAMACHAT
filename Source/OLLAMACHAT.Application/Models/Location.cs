namespace VelikiyPrikalel.OLLAMACHAT.Application.Models;

/// <summary>
/// Расположение элемента.
/// </summary>
/// <param name="Start">Стартовая позиция.</param>
/// <param name="End">Конец.</param>
public record Location(Position Start, Position End);
