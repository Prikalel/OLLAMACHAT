namespace VelikiyPrikalel.OLLAMACHAT.Data.Parser.Enums;

/// <summary>
/// Типы связей между событиями и обработчиками.
/// </summary>
public enum EventHandlerRelationshipType
{
    /// <summary>
    /// Потенциальная связь, определенная по сигнатуре метода.
    /// </summary>
    Potential = 0,

    /// <summary>
    /// Подтвержденная связь, установленная через инспектор или код.
    /// </summary>
    Confirmed,

    /// <summary>
    /// Устаревшая связь.
    /// </summary>
    Obsolete
}
