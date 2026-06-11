using Microsoft.Extensions.Logging;

namespace DeleteLogFiles;

internal sealed class FileLogger : ILogger
{
    private readonly string categoryName;
    private readonly FileLoggingOptions options;
    private readonly Action<string> writeLine;

    public FileLogger(string categoryName, FileLoggingOptions options, Action<string> writeLine)
    {
        this.categoryName = categoryName;
        this.options = options;
        this.writeLine = writeLine;
    }

    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return options.Enabled && logLevel >= options.MinimumLevel && logLevel != LogLevel.None;
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var message = formatter(state, exception);
        if (string.IsNullOrWhiteSpace(message) && exception is null)
        {
            return;
        }

        var line = $"{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff zzz} [{logLevel}] {categoryName}: {message}";
        writeLine(line);

        if (exception is not null)
        {
            writeLine(exception.ToString());
        }
    }
}
