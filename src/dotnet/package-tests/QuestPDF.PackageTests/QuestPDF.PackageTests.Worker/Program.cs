using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<PdfWorker>();
    })
    .Build();

await host.RunAsync();

public sealed class PdfWorker(IHostApplicationLifetime applicationLifetime) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        QuestPDF.PackageTests.Shared.TestRunner.Run();
        applicationLifetime.StopApplication();

        return Task.CompletedTask;
    }
}