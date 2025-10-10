namespace VelikiyPrikalel.OLLAMACHAT.Data.Parser.Enums;

/// <summary>
/// Источники связей между событиями и обработчиками.
/// </summary>
public enum Source
{
    /// <summary>
    /// Установлена через парсер кода.
    /// </summary>
    Parser = 0,

    /// <summary>
    /// Установлена через Unity Inspector.
    /// </summary>
    Inspector,

    /// <summary>
    /// Установлена программно в коде.
    /// </summary>
    Code,

    /// <summary>
    /// Установлена вручную через UI.
    /// </summary>
    Manual
}
