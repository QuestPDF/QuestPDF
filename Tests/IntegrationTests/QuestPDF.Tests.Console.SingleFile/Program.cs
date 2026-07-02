using QuestPDF.Tests.Shared;

var outputDirectory = Environment.CurrentDirectory;

PdfSmokeTests.GeneratePdfFiles(outputDirectory);

if (PdfSmokeTests.ShouldGenerateXps())
    PdfSmokeTests.GenerateXpsFile(outputDirectory);

return 0;
