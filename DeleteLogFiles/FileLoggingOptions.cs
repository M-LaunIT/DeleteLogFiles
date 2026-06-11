using Microsoft.Extensions.Logging;

namespace DeleteLogFiles;

public sealed class FileLoggingOptions
{
    public bool Enabled { get; set; } = true;

    public string Path { get; set; } = "Logs\\DeleteLogFiles.log";

    public LogLevel MinimumLevel { get; set; } = LogLevel.Information;
}
