using System.IO;
using System.Runtime.InteropServices;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.PackageTests.Shared;

public static class TestRunner
{
    public const string OutputFolder = "TestOutput";
    
    public static void Run()
    {
        QuestPDF.Settings.License = LicenseType.Community;
        QuestPDF.Settings.UseEnvironmentFonts = false;
        QuestPDF.Settings.CheckIfAllTextGlyphsAreAvailable = true;
        
        Directory.CreateDirectory(OutputFolder);
        
        var skiaPdfOutput = Path.Combine(OutputFolder, "skia.pdf");
        var skiaXpsOutput = Path.Combine(OutputFolder, "skia.xps");
        var pdfToMerge = "to-merge.pdf"; 
        var qpdfOutput = Path.Combine(OutputFolder, "qpdf.pdf");

        CreateMainDocument().GeneratePdf(skiaPdfOutput);
        CreateDocumentToMerge().GeneratePdf(pdfToMerge);
        
        DocumentOperation
            .LoadFile(skiaPdfOutput)
            .MergeFile(pdfToMerge)
            .AddAttachment(new DocumentOperation.DocumentAttachment()
            {
                FilePath = "Resources/books.xml"
            })
            .Save(qpdfOutput);
        
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            CreateMainDocument().GeneratePdf(skiaXpsOutput);
    }

    private static IDocument CreateMainDocument()
    {
        return Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Margin(50);
                    page.Size(PageSizes.A5);
                    page.DefaultTextStyle(x => x.FontSize(24));
  
                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(10);
                            column.Item().Text("Lorem ipsum dolor sit amet");
                            column.Item().Text("مرحبا بالعالم").FontFamily("Noto Sans Arabic");
                            column.Item().Width(50).Image("Resources/questpdf-logo.png");
                        });
                });
            });
    }
    
    private static IDocument CreateDocumentToMerge()
    {
        return Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Content().Text("Document to merge");
                });
            });
    }
}