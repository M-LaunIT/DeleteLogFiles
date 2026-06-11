using System.Text.Json.Serialization;

namespace DeleteLogFiles.Configurator;

internal sealed class AppConfiguration
{
    public CleanupSettings Cleanup { get; set; } = new();

    public FileLoggingSettings FileLogging { get; set; } = new();

    public LoggingSettings Logging { get; set; } = new();
}

internal sealed class CleanupSettings
{
    public int IntervalMinutes { get; set; } = 30;

    public int DeleteAfterDays { get; set; } = 14;

    public bool IncludeSubdirectories { get; set; } = true;

    public bool DryRun { get; set; } = true;

    public List<string> Directories { get; set; } = [];

    public List<string> Extensions { get; set; } = [];
}

internal sealed class LoggingSettings
{
    public LogLevelSettings LogLevel { get; set; } = new();
}

internal sealed class LogLevelSettings
{
    public string Default { get; set; } = "Information";

    [JsonPropertyName("Microsoft.Hosting.Lifetime")]
    public string MicrosoftHostingLifetime { get; set; } = "Information";
}

internal sealed class FileLoggingSettings
{
    public bool Enabled { get; set; } = true;

    public string Path { get; set; } = "Logs\\DeleteLogFiles.log";

    public string MinimumLevel { get; set; } = "Information";
}
