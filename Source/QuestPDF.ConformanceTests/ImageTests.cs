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

    protected override SemanticTreeNode? GetExpectedSemanticTree()
    {
        throw new NotImplementedException();
    }
}