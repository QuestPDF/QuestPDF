using QuestPDF.Tests.Shared;

var outputDirectory = args.Length > 0 ? args[0] : AppContext.BaseDirectory;
var pdfPath = PdfSmokeTests.GeneratePdfFile(outputDirectory);

Console.WriteLine(pdfPath);
return 0;
