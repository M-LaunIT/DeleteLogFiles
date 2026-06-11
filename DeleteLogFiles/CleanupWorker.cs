using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DeleteLogFiles;

public sealed class CleanupWorker : BackgroundService
{
    private readonly CleanupService cleanupService;
    private readonly IConfiguration configuration;
    private readonly ILogger<CleanupWorker> logger;
    private readonly SemaphoreSlim runLock = new(1, 1);

    public CleanupWorker(
        CleanupService cleanupService,
        IConfiguration configuration,
        ILogger<CleanupWorker> logger)
    {
        this.cleanupService = cleanupService;
        this.configuration = configuration;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var options = LoadOptions();
            await RunOnce(options, stoppingToken);

            var delay = TimeSpan.FromMinutes(Math.Max(1, options.IntervalMinutes));
            logger.LogInformation("Next cleanup run scheduled in {Delay}.", delay);
            await Task.Delay(delay, stoppingToken);
        }
    }

    private async Task RunOnce(CleanupOptions options, CancellationToken stoppingToken)
    {
        if (!await runLock.WaitAsync(0, stoppingToken))
        {
            logger.LogWarning("Cleanup run skipped because a previous run is still active.");
            return;
        }

        try
        {
            logger.LogInformation(
                "Cleanup run started. DryRun={DryRun}, DeleteAfterDays={DeleteAfterDays}, IncludeSubdirectories={IncludeSubdirectories}",
                options.DryRun,
                options.DeleteAfterDays,
                options.IncludeSubdirectories);

            var result = cleanupService.Run(options, stoppingToken);

            logger.LogInformation(
                "Cleanup run finished. Directories={Directories}, FilesChecked={FilesChecked}, Deleted={Deleted}, WouldDelete={WouldDelete}, Skipped={Skipped}, Freed={FreedMb:0.###} MB, WouldFree={WouldFreeMb:0.###} MB",
                result.DirectoriesChecked,
                result.FilesChecked,
                result.FilesDeleted,
                result.FilesWouldDelete,
                result.FilesSkipped,
                result.BytesFreed / 1024d / 1024d,
                result.BytesWouldFree / 1024d / 1024d);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Cleanup service is stopping.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Cleanup run failed.");
        }
        finally
        {
            runLock.Release();
        }
    }

    private CleanupOptions LoadOptions()
    {
        var section = configuration.GetSection("Cleanup");

        return new CleanupOptions
        {
            IntervalMinutes = section.GetValue("IntervalMinutes", 30),
            DeleteAfterDays = section.GetValue("DeleteAfterDays", 14),
            IncludeSubdirectories = section.GetValue("IncludeSubdirectories", true),
            DryRun = section.GetValue("DryRun", true),
            Directories = section.GetSection("Directories").Get<string[]>() ?? [],
            Extensions = section.GetSection("Extensions").Get<string[]>() ?? []
        };
    }
}
