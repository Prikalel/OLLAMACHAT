namespace VelikiyPrikalel.OLLAMACHAT.Application.Repositories.Parser;

/// <summary>
/// Репозиторий кеша.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ICacheRepository<T>
    where T : class
{
    /// <summary>
    /// Очистить.
    /// </summary>
    void Clear();

    /// <summary>
    /// Попытаться получить значение.
    /// </summary>
    /// <param name="key">Ключ.</param>
    /// <param name="value">Значение.</param>
    /// <returns>True если удалось получить значение.</returns>
    bool TryGetValue(string key, out T? value);

    /// <summary>
    /// Попытаться добавить.
    /// </summary>
    /// <param name="key">Ключ.</param>
    /// <param name="value">Значение.</param>
    /// <returns>True если удалось добавить.</returns>
    bool TryAdd(string key, T value);

    /// <summary>
    /// Итерация по всем элементам.
    /// </summary>
    /// <returns>Массив ключ-значение.</returns>
    IEnumerable<KeyValuePair<string, T>> GetAll();
}
