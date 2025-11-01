namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Repositories.Parser;

/// <summary>
/// Реализация репозитория кеша с использованием Redis.
/// </summary>
/// <typeparam name="T">Тип кешируемых объектов.</typeparam>
public class CacheRepository<T> : ICacheRepository<T>
    where T : class
{
    private readonly IDatabase database;
    private readonly string keyPrefix;
    private readonly ILogger<CacheRepository<T>> logger;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="CacheRepository{T}"/>.
    /// </summary>
    /// <param name="database">База данных Redis.</param>
    /// <param name="logger">Логгер..</param>
    public CacheRepository(IDatabase database, ILogger<CacheRepository<T>> logger)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database));
        keyPrefix = typeof(T).Name + ":";
        this.logger = logger;
    }

    /// <inheritdoc />
    public void Clear()
    {
        try
        {
            IServer server = database.Multiplexer.GetServer(database.Multiplexer.GetEndPoints().First());
            IEnumerable<RedisKey> keys = server.Keys(database.Database, keyPrefix + "*");

            foreach (RedisKey key in keys)
            {
                database.KeyDelete(key);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error clearing cache for type {typeof(T).Name}: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public bool TryGetValue(string key, out T? value)
    {
        value = null;

        try
        {
            string fullKey = keyPrefix + key;
            RedisValue redisValue = database.StringGet(fullKey);

            if (redisValue.HasValue)
            {
                string json = redisValue.ToString();
                value = JsonSerializer.Deserialize<T>(json);
                return value != null;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error getting value {key} for type {typeof(T).Name}: {ex.Message}");
        }

        return false;
    }

    /// <inheritdoc />
    public bool TryAdd(string key, T value)
    {
        if (value == null)
        {
            return false;
        }

        try
        {
            string fullKey = keyPrefix + key;
            string json = JsonSerializer.Serialize(value);
            return database.StringSet(fullKey, json);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error adding value to cache for key {key}: {ex.Message}");
            return false;
        }
    }

    /// <inheritdoc />
    public IEnumerable<KeyValuePair<string, T>> GetAll()
    {
        List<KeyValuePair<string, T>> result = new();

        try
        {
            IServer server = database.Multiplexer.GetServer(database.Multiplexer.GetEndPoints().First());
            IEnumerable<RedisKey> keys = server.Keys(database.Database, keyPrefix + "*");

            foreach (RedisKey redisKey in keys)
            {
                string key = redisKey.ToString().Substring(keyPrefix.Length);
                RedisValue redisValue = database.StringGet(redisKey);

                if (redisValue.HasValue)
                {
                    string json = redisValue.ToString();
                    T? value = JsonSerializer.Deserialize<T>(json);

                    if (value != null)
                    {
                        result.Add(new(key, value));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error getting all values from cache for type {typeof(T).Name}: {ex.Message}");
        }

        return result;
    }
}
