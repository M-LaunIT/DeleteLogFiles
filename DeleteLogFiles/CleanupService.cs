using Microsoft.Extensions.Logging;

namespace DeleteLogFiles;

public sealed class CleanupService
{
    private readonly ILogger<CleanupService> logger;

    public CleanupService(ILogger<CleanupService> logger)
    {
        this.logger = logger;
    }

    public CleanupResult Run(CleanupOptions options, CancellationToken cancellationToken)
    {
        Validate(options);

        var result = new CleanupResult();
        var cutoffDate = DateTimeOffset.Now.AddDays(-options.DeleteAfterDays);
        var extensions = NormalizeExtensions(options.Extensions);
        var searchOption = options.IncludeSubdirectories
            ? SearchOption.AllDirectories
            : SearchOption.TopDirectoryOnly;

        foreach (var configuredDirectory in options.Directories)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(configuredDirectory))
            {
                result.FilesSkipped++;
                logger.LogWarning("Empty directory entry skipped.");
                continue;
            }

            var directory = configuredDirectory.Trim();
            if (!Directory.Exists(directory))
            {
                result.FilesSkipped++;
                logger.LogWarning("Directory does not exist or is not accessible: {Directory}", directory);
                continue;
            }

            result.DirectoriesChecked++;
            ProcessDirectory(directory, searchOption, extensions, cutoffDate, options.DryRun, result, cancellationToken);
        }

        return result;
    }

    private void ProcessDirectory(
        string directory,
        SearchOption searchOption,
        HashSet<string> extensions,
        DateTimeOffset cutoffDate,
        bool dryRun,
        CleanupResult result,
        CancellationToken cancellationToken)
    {
        IEnumerable<string> files;

        try
        {
            files = Directory.EnumerateFiles(directory, "*", searchOption);
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or IOException)
        {
            result.FilesSkipped++;
            logger.LogWarning(ex, "Directory could not be enumerated: {Directory}", directory);
            return;
        }

        foreach (var filePath in files)
        {
            cancellationToken.ThrowIfCancellationRequested();
            result.FilesChecked++;

            try
            {
                var file = new FileInfo(filePath);
                if (!extensions.Contains(file.Extension) || file.LastWriteTimeUtc > cutoffDate.UtcDateTime)
                {
                    continue;
                }

                if (dryRun)
                {
                    result.FilesWouldDelete++;
                    result.BytesWouldFree += file.Length;
                    logger.LogInformation("Dry run: {File} would be deleted ({Size} bytes).", file.FullName, file.Length);
                    continue;
                }

                var size = file.Length;
                file.Delete();
                result.FilesDeleted++;
                result.BytesFreed += size;
                logger.LogInformation("Deleted {File} ({Size} bytes).", file.FullName, size);
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException or IOException)
            {
                result.FilesSkipped++;
                logger.LogWarning(ex, "File could not be processed: {File}", filePath);
            }
        }
    }

    private static void Validate(CleanupOptions options)
    {
        if (options.IntervalMinutes < 1)
        {
            throw new InvalidOperationException("IntervalMinutes must be at least 1.");
        }

        if (options.DeleteAfterDays < 1)
        {
            throw new InvalidOperationException("DeleteAfterDays must be at least 1.");
        }

        if (options.Directories.Length == 0)
        {
            throw new InvalidOperationException("At least one directory must be configured.");
        }

        if (options.Extensions.Length == 0)
        {
            throw new InvalidOperationException("At least one file extension must be configured.");
        }
    }

    private static HashSet<string> NormalizeExtensions(IEnumerable<string> extensions)
    {
        return extensions
            .Where(extension => !string.IsNullOrWhiteSpace(extension))
            .Select(extension => extension.Trim())
            .Select(extension => extension.StartsWith('.') ? extension : "." + extension)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }
}
