namespace VelikiyPrikalel.OLLAMACHAT.Application.Models;

/// <summary>
/// Представляет опции конфигурации для выборочного парсинга.
/// </summary>
/// <param name="ExtractReferences">Извлекать ссылки.</param>
/// <param name="ExtractInheritance">Извлекать информацию о наследовании.</param>
/// <param name="DetectPatterns">Обнаруживать паттерны.</param>
/// <param name="MaxDepth">Максимальная глубина.</param>
public record ParserOptions(bool? ExtractReferences, bool? ExtractInheritance, bool? DetectPatterns, int? MaxDepth);