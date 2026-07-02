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
        var skiaPdfFileName = Environment.GetEnvironmentVariable("QUESTPDF_TEST_SKIA_PDF_FILE") ?? "questpdf-integration-worker-skia.pdf";
        var qpdfPdfFileName = Environment.GetEnvironmentVariable("QUESTPDF_TEST_QPDF_PDF_FILE") ?? "questpdf-integration-worker-qpdf.pdf";
        var xpsFileName = Environment.GetEnvironmentVariable("QUESTPDF_TEST_XPS_FILE");
        var pdfOutput = PdfSmokeTests.GeneratePdfFiles(outputDirectory, skiaPdfFileName, qpdfPdfFileName);

        logger.LogInformation("Generated {PdfPath}", pdfOutput.SkiaPdfPath);
        logger.LogInformation("Generated {PdfPath}", pdfOutput.QpdfPdfPath);

        if (!string.IsNullOrWhiteSpace(xpsFileName))
        {
            var xpsPath = PdfSmokeTests.GenerateXpsFile(outputDirectory, xpsFileName);
            logger.LogInformation("Generated {XpsPath}", xpsPath);
        }

        applicationLifetime.StopApplication();

        return Task.CompletedTask;
    }
}
