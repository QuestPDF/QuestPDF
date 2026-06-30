using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QuestPDF.Tests.Shared;

var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureServices(services => services.AddHostedService<PdfWorker>())
    .Build();

await host.RunAsync();

public sealed class PdfWorker : BackgroundService
{
    private readonly IHostApplicationLifetime applicationLifetime;
    private readonly ILogger<PdfWorker> logger;

    public PdfWorker(IHostApplicationLifetime applicationLifetime, ILogger<PdfWorker> logger)
    {
        this.applicationLifetime = applicationLifetime;
        this.logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var outputDirectory = Environment.GetEnvironmentVariable("QUESTPDF_TEST_OUTPUT") ?? AppContext.BaseDirectory;
        var pdfPath = PdfSmokeTests.GeneratePdfFile(outputDirectory, "questpdf-integration-worker.pdf");

        logger.LogInformation("Generated {PdfPath}", pdfPath);
        applicationLifetime.StopApplication();

        return Task.CompletedTask;
    }
}
