using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuestPDF.Tests.Shared;

var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureServices(services => services.AddHostedService<PdfWorker>())
    .Build();

await host.RunAsync();

public sealed class PdfWorker : BackgroundService
{
    private readonly IHostApplicationLifetime applicationLifetime;

    public PdfWorker(IHostApplicationLifetime applicationLifetime)
    {
        this.applicationLifetime = applicationLifetime;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        PdfSmokeTests.GenerateAllSupportedFiles(Environment.CurrentDirectory);
        applicationLifetime.StopApplication();

        return Task.CompletedTask;
    }
}
