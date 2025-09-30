namespace VelikiyPrikalel.OLLAMACHAT.Application.Models;

/// <summary>
/// Позиция в файле.
/// </summary>
/// <param name="Line">1-based line number.</param>
/// <param name="Column">0-based column position.</param>
/// <param name="Index">0-based character index from the start of the file.</param>
public record Position(int? Line, int? Column, int? Index);
