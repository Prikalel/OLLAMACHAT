namespace VelikiyPrikalel.OLLAMACHAT.Application.Mediator.UnityParser;

/// <summary>
/// Результат сравнения изменений между существующими и новыми данными.
/// </summary>
/// <param name="NewItems">Новые элементы, которых не было в существующих данных.</param>
/// <param name="ModifiedItems">Измененные элементы, которые есть в обоих наборах, но с разными свойствами.</param>
/// <param name="DeletedItems">Удаленные элементы, которые есть в существующих данных, но отсутствуют в новых.</param>
/// <param name="UnchangedItems">Неизмененные элементы, которые идентичны в обоих наборах.</param>
/// <param name="TotalNewCount">Общее количество новых элементов.</param>
/// <param name="TotalModifiedCount">Общее количество измененных элементов.</param>
/// <param name="TotalDeletedCount">Общее количество удаленных элементов.</param>
/// <param name="TotalUnchangedCount">Общее количество неизмененных элементов.</param>
/// <param name="ExecutionTime">Время выполнения сравнения.</param>
public record ChangeDetectionResult<T>(
    IReadOnlyList<T> NewItems,
    IReadOnlyList<ChangeDetectionResult<T>.ModifiedItem> ModifiedItems,
    IReadOnlyList<T> DeletedItems,
    IReadOnlyList<T> UnchangedItems,
    int TotalNewCount,
    int TotalModifiedCount,
    int TotalDeletedCount,
    int TotalUnchangedCount,
    TimeSpan ExecutionTime)
{
    /// <summary>
    /// Представляет измененный элемент с информацией о том, что изменилось.
    /// </summary>
    /// <param name="ExistingItem">Существующий элемент.</param>
    /// <param name="NewItem">Новый элемент.</param>
    /// <param name="ChangedProperties">Список измененных свойств.</param>
    public record ModifiedItem(
        T ExistingItem,
        T NewItem,
        IReadOnlyList<string> ChangedProperties);
}
