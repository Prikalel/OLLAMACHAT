namespace VelikiyPrikalel.OLLAMACHAT.Data.Parser.Enums;

/// <summary>
/// Причины удаления связей между событиями и обработчиками.
/// </summary>
public enum DeletionReason
{
    /// <summary>
    /// Событие было удалено из кода.
    /// </summary>
    EventDeleted = 0,

    /// <summary>
    /// Обработчик был удален из кода.
    /// </summary>
    HandlerDeleted,

    /// <summary>
    /// Изменилась сигнатура метода или события.
    /// </summary>
    SignatureChanged,

    /// <summary>
    /// Класс был удален или переименован.
    /// </summary>
    ClassDeleted
}
