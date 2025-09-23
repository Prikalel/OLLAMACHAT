namespace VelikiyPrikalel.OLLAMACHAT.Application.Models;

/// <summary>
/// Представляет метаданные о проанализированном файле.
/// </summary>
/// <param name="Loc">Количество строк кода.</param>
/// <param name="ComplexityScore">Общая цикломатическая сложность.</param>
/// <param name="Namespace">Основное пространство имен файла.</param>
public record FileMetadata(int? Loc, int? ComplexityScore, string? Namespace);