namespace VelikiyPrikalel.OLLAMACHAT.Application.Models;

/// <summary>
/// Представляет опции конфигурации для выборочного парсинга.
/// </summary>
/// <param name="ExtractFullExtractInheritance">Извлекать полную информацию о наследовании.</param>
/// <param name="MaxDepth">Максимальная глубина.</param>
/// <param name="ExtractUsingStatementData">Извлекать данные о using-директивах.</param>
public record ParserOptions(bool? ExtractFullExtractInheritance, int? MaxDepth, bool? ExtractUsingStatementData);