namespace DeleteLogFiles;

public sealed class CleanupOptions
{
    public int IntervalMinutes { get; set; } = 30;

    public int DeleteAfterDays { get; set; } = 14;

    public bool IncludeSubdirectories { get; set; } = true;

    public bool DryRun { get; set; } = true;

    public string[] Directories { get; set; } = [];

    public string[] Extensions { get; set; } = [];
}
