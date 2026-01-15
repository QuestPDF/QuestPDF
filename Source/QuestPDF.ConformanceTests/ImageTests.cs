using QuestPDF.ConformanceTests.TestEngine;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.ConformanceTests;

internal class ImageTests : ConformanceTestBase
{
    [OneTimeSetUp]
    public void Setup()
    {
        // PDF/A-1a and PDF/A-1b require ICC profile version 2
        // prepare an image with ICC profile version 2
        var sourceImagePath = Path.Combine("Resources", "photo.jpeg");
        var targetImagePath = Path.Combine("Resources", "photo-icc2.jpeg");

        ImageHelpers.ConvertImageIccColorSpaceProfileToVersion2(sourceImagePath, targetImagePath);
    }
    
    protected override Document GetDocumentUnderTest()
    {
        var useImageWithIcc2 = TestContext.CurrentContext.Test.Arguments.FirstOrDefault() is PDFA_Conformance.PDFA_1A or PDFA_Conformance.PDFA_1B;
        
        var imagePath = useImageWithIcc2 
            ? Path.Combine("Resources", "photo-icc2.jpeg") 
            : Path.Combine("Resources", "photo.jpeg");

        var imageData = File.ReadAllBytes(imagePath);
        
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
                            column.Spacing(25);

                            column.Item()
                                .SemanticHeader1()
                                .Text("Conformance Test: Images")
                                .FontSize(24)
                                .Bold()
                                .FontColor(Colors.Blue.Darken2);
                            
                            column.Item()
                                .Width(300)
                                .SemanticImage("Sample image description")
                                .Column(column =>
                                {
                                    column.Item().Image(imageData);
                                    column.Item().PaddingTop(5).AlignCenter().SemanticCaption().Text("Sample image caption");
                                });
                        });
                });
            });
    }

    protected override SemanticTreeNode? GetExpectedSemanticTree()
    {
        return ExpectedSemanticTree.DocumentRoot(root =>
        {
            root.Child("H1", h1 => h1.Alt("Conformance Test: Images"));

            root.Child("Figure", figure =>
            {
                figure.Alt("Sample image description");
                figure.Child("Caption", caption => caption.Child("P"));
            });
        });
    }
}