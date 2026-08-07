using QuestPDF.ConformanceTests.TestEngine;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.ConformanceTests;

internal class FigureBoundingBoxTests : ConformanceTestBase
{
    protected override Document GetDocumentUnderTest()
    {
        var imageData = File.ReadAllBytes("Resources/photo.jpeg");

        return Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(50);

                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(25);

                            column.Item()
                                .SemanticHeader1()
                                .Text("Conformance Test: Figure Bounding Boxes")
                                .FontSize(24)
                                .Bold()
                                .FontColor(Colors.Blue.Darken2);

                            column.Item()
                                .Width(300)
                                .SemanticImage("Image with a fixed width")
                                .Image(imageData);

                            column.Item()
                                .PaddingLeft(100)
                                .Rotate(10)
                                .Width(200)
                                .SemanticImage("Rotated image")
                                .Image(imageData);

                            column.Item()
                                .Width(150)
                                .SemanticFormula("Mass-energy equivalence formula")
                                .ExtendHorizontal()
                                .Height(30)
                                .Text("E = mc2");
                        });
                });
            });
    }

    protected override SemanticTreeNode? GetExpectedSemanticTree()
    {
        return ExpectedSemanticTree.DocumentRoot(root =>
        {
            root.Child("H1", h1 => h1.Alt("Conformance Test: Figure Bounding Boxes"));

            root.Child("Figure", figure => figure
                .Alt("Image with a fixed width")
                .Attribute("Layout", "BBox", new[] { 50f, 538.9813f, 350f, 738.2001f }));

            // the attribute describes an axis-aligned rectangle that encloses the rotated content
            root.Child("Figure", figure => figure
                .Alt("Rotated image")
                .Attribute("Layout", "BBox", new[] { 126.93735f, 348.4569f, 346.96155f, 513.9813f }));

            root.Child("Formula", formula =>
            {
                formula.Alt("Mass-energy equivalence formula");
                formula.Attribute("Layout", "BBox", new[] { 50f, 326.16882f, 200f, 356.16882f });
                formula.Child("P");
            });
        });
    }
}

internal class FigureSpanningMultiplePagesTests
{
    private const int LineCount = 40;

    [Test]
    public void BoundingBoxIsNotEmittedForFiguresSpanningMultiplePages()
    {
        var document = Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A5);
                    page.Margin(25);

                    page.Content()
                        .SemanticImage("Chart spanning multiple pages")
                        .Column(column =>
                        {
                            foreach (var index in Enumerable.Range(1, LineCount))
                                column.Item().Height(20).Text($"Chart line {index}");
                        });
                });
            });

        var expectedSemanticTree = ExpectedSemanticTree.DocumentRoot(root =>
        {
            // the figure spans two pages, so the Layout/BBox attribute should not be present
            root.Child("Figure", figure =>
            {
                figure.Alt("Chart spanning multiple pages");

                foreach (var _ in Enumerable.Range(1, LineCount))
                    figure.Child("P");
            });
        });

        document.TestSemanticTree(expectedSemanticTree);
    }
}
