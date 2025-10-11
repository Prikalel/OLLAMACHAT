namespace TestParser;

/// <summary>
/// Простой фейковый логгер для тестов
/// </summary>
/// <typeparam name="T">Тип логгера</typeparam>
public class FakeLogger<T> : ILogger<T>
{
    private readonly List<LogEntry> logEntries = new();

    /// <summary>
    /// Получает все записи лога
    /// </summary>
    public IReadOnlyList<LogEntry> LogEntries => logEntries;

    /// <summary>
    /// Запись в лог
    /// </summary>
    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        return null!;
    }

    /// <summary>
    /// Проверка, включен ли уровень логирования
    /// </summary>
    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    /// <summary>
    /// Запись лога
    /// </summary>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        string message = formatter(state, exception);
        logEntries.Add(new LogEntry(logLevel, eventId, message, exception));
    }
}

/// <summary>
/// Запись в логе
/// </summary>
public record LogEntry(LogLevel Level, EventId EventId, string Message, Exception? Exception);
