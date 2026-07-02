using QuestPDF.Tests.Shared;

var outputDirectory = args.Length > 0 ? args[0] : AppContext.BaseDirectory;
var skiaPdfFileName = args.Length > 1 ? args[1] : "questpdf-integration-smoke-skia.pdf";
var qpdfPdfFileName = args.Length > 2 ? args[2] : "questpdf-integration-smoke-qpdf.pdf";
var xpsFileName = args.Length > 3 ? args[3] : null;

var pdfOutput = PdfSmokeTests.GeneratePdfFiles(outputDirectory, skiaPdfFileName, qpdfPdfFileName);

Console.WriteLine(pdfOutput.SkiaPdfPath);
Console.WriteLine(pdfOutput.QpdfPdfPath);

if (xpsFileName != null)
{
    var xpsPath = PdfSmokeTests.GenerateXpsFile(outputDirectory, xpsFileName);
    Console.WriteLine(xpsPath);
}

return 0;
