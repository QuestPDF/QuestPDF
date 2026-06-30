using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using QuestPDF.Tests.Shared;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/health", () => Results.Ok("ready"));

app.MapGet("/generate-pdf", () =>
{
    var pdf = PdfSmokeTests.GeneratePdfBytes();
    PdfValidator.ValidateBytes(pdf, "HTTP response PDF");

    return Results.File(pdf, "application/pdf", "questpdf-integration-smoke.pdf");
});

app.Run();
