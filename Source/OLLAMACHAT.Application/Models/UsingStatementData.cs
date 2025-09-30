namespace VelikiyPrikalel.OLLAMACHAT.Application.Models;

/// <summary>
/// Представляет данные о using-директиве сущности.
/// </summary>
/// <param name="Source">Источник using-директивы (полный путь до файла куда указывает директива using).</param>
public record UsingStatementData(string? Source);