using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.ConformanceTests;

public class ImageTests
{
    [Test]
    [Ignore("For manual testing purposes only")]
    public void GenerateAndShow()
    {
        GetDocumentUnderTest()
            .WithSettings(new DocumentSettings
            {
                PDFUA_Conformance = PDFUA_Conformance.PDFUA_1
            })
            .GeneratePdfAndShow();
    }

    [OneTimeSetUp]
    public void Setup()
    {
        // PDF/A-1a and PDF/A-1b require ICC profile version 2
        // prepare an image with ICC profile version 2
        var sourceImagePath = Path.Combine("Resources", "photo.jpeg");
        var targetImagePath = Path.Combine("Resources", "photo-icc2.jpeg");

        ImageHelpers.ConvertImageIccColorSpaceProfileToVersion2(sourceImagePath, targetImagePath);
    }
    
    [Test, TestCaseSource(typeof(TestHelpers), nameof(TestHelpers.PDFA_ConformanceLevels))]
    public void Test_PDFA(PDFA_Conformance conformance)
    {
        var useImageWithIcc2 = conformance is PDFA_Conformance.PDFA_1A or PDFA_Conformance.PDFA_1B;
        
        GetDocumentUnderTest(useImageWithIcc2)
            .WithSettings(new DocumentSettings
            {
                PDFA_Conformance = conformance
            })
            .TestConformance();
    }
    
    [Test, TestCaseSource(typeof(TestHelpers), nameof(TestHelpers.PDFUA_ConformanceLevels))]
    public void Test_PDFUA(PDFUA_Conformance conformance)
    {
        GetDocumentUnderTest()
            .WithSettings(new DocumentSettings
            {
                PDFUA_Conformance = conformance
            })
            .TestConformance();
    }

    private Document GetDocumentUnderTest(bool useImageWithIcc2 = false)
    {
        var imagePath = useImageWithIcc2 
            ? Path.Combine("Resources", "photo-icc2.jpeg") 
            : Path.Combine("Resources", "photo.jpeg");
        
        return Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Margin(60);

                    page.Content()
                        .PaddingVertical(30)
                        .Column(column =>
                        {
                            column.Item()
                                .ExtendVertical()
                                .AlignMiddle()
                                .SemanticHeader1()
                                .Text("Conformance Test:\nImages")
                                .FontSize(36)
                                .Bold()
                                .FontColor(Colors.Blue.Darken2);

                            column.Item().PageBreak();

                            column.Item()
                                .SemanticImage("Sample image description")
                                .Column(column =>
                                {
                                    column.Item().Width(300).Image(imagePath);
                                    column.Item().SemanticCaption().Text("Sample image caption");
                                });
                        });
                });
            })
            .WithMetadata(new DocumentMetadata
            {
                Language = "en-US",
                Title = "Conformance Test", 
                Subject = "Images"
            });
    }
}