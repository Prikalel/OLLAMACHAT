namespace TestParser;

/// <summary>
/// Реализация репозитория кеша с использованием in-memory ConcurrentDictionary.
/// </summary>
/// <typeparam name="T">Тип кешируемых объектов.</typeparam>
public class FakeCacheRepository<T> : ICacheRepository<T>
    where T : class
{
    private readonly ConcurrentDictionary<string, T> cache;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="CacheRepository{T}"/>.
    /// </summary>
    public FakeCacheRepository()
    {
        cache = new ConcurrentDictionary<string, T>();
    }

    /// <inheritdoc />
    public void Clear()
    {
        try
        {
            cache.Clear();
        }
        catch (Exception ex)
        {
        }
    }

    /// <inheritdoc />
    public bool TryGetValue(string key, out T? value)
    {
        if (string.IsNullOrEmpty(key))
        {
            value = null;
            return false;
        }

        try
        {
            bool result = cache.TryGetValue(key, out value);
            return result;
        }
        catch (Exception ex)
        {
            value = null;
            return false;
        }
    }

    /// <inheritdoc />
    public bool TryAdd(string key, T value)
    {
        if (string.IsNullOrEmpty(key) || value == null)
        {
            return false;
        }

        try
        {
            bool result = cache.TryAdd(key, value);
            return result;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    /// <inheritdoc />
    public IEnumerable<KeyValuePair<string, T>> GetAll()
    {
        try
        {
            return cache.ToList();
        }
        catch (Exception ex)
        {
            return Enumerable.Empty<KeyValuePair<string, T>>();
        }
    }
}
