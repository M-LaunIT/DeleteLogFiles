using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DeleteLogFiles;

public sealed class FileLoggerProvider : ILoggerProvider
{
    private readonly FileLoggingOptions options;
    private readonly object lockObject = new();
    private bool disposed;

    public FileLoggerProvider(IOptions<FileLoggingOptions> options)
    {
        this.options = options.Value;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new FileLogger(categoryName, options, WriteLine);
    }

    public void Dispose()
    {
        disposed = true;
    }

    private void WriteLine(string line)
    {
        if (disposed || !options.Enabled)
        {
            return;
        }

        var logPath = ResolvePath(options.Path);
        var directory = System.IO.Path.GetDirectoryName(logPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        lock (lockObject)
        {
            File.AppendAllText(logPath, line + Environment.NewLine);
        }
    }

    private static string ResolvePath(string configuredPath)
    {
        if (string.IsNullOrWhiteSpace(configuredPath))
        {
            configuredPath = "Logs\\DeleteLogFiles.log";
        }

        return System.IO.Path.IsPathRooted(configuredPath)
            ? configuredPath
            : System.IO.Path.Combine(AppContext.BaseDirectory, configuredPath);
    }
}
