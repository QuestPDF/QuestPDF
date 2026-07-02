using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using QuestPDF.Tests.Shared;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/health", () => Results.Ok("ready"));

app.MapGet("/generate-pdf", () =>
{
    var pdf = PdfSmokeTests.GenerateQpdfPdfBytes();
    PdfValidator.ValidateBytes(pdf, "HTTP response PDF");

    return Results.File(pdf, "application/pdf", "questpdf-integration-smoke.pdf");
});

app.MapGet("/generate-skia-pdf", () =>
{
    var pdf = PdfSmokeTests.GenerateSkiaPdfBytes();
    PdfValidator.ValidateBytes(pdf, "HTTP response Skia PDF");

    return Results.File(pdf, "application/pdf", "questpdf-integration-smoke-skia.pdf");
});

app.MapGet("/generate-xps", () =>
{
    var xps = PdfSmokeTests.GenerateXpsBytes();
    PdfValidator.ValidateXpsBytes(xps, "HTTP response XPS");

    return Results.File(xps, "application/vnd.ms-xpsdocument", "questpdf-integration-smoke.xps");
});

app.Run();
