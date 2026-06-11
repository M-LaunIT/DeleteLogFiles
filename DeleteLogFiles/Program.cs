using DeleteLogFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "DeleteLogFiles";
});

builder.Services.Configure<FileLoggingOptions>(builder.Configuration.GetSection("FileLogging"));
builder.Logging.AddProvider(new FileLoggerProvider(
    Microsoft.Extensions.Options.Options.Create(
        builder.Configuration.GetSection("FileLogging").Get<FileLoggingOptions>() ?? new FileLoggingOptions())));

builder.Services.AddSingleton<CleanupService>();
builder.Services.AddHostedService<CleanupWorker>();

var host = builder.Build();
await host.RunAsync();
