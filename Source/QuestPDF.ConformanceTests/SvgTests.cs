using QuestPDF.ConformanceTests.TestEngine;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace QuestPDF.ConformanceTests;

internal class SvgTests : ConformanceTestBase
{
    protected override Document GetDocumentUnderTest()
    {
        return Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Margin(60);

                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(25);

                            column.Item()
                                .SemanticHeader1()
                                .Text("Conformance Test: SVG")
                                .FontSize(24)
                                .Bold()
                                .FontColor(Colors.Blue.Darken2);

                            column.Item()
                                .Text("SVG content should be rendered correctly and possible to be annotated as semantic image. Image taken from: undraw.co");

                            column.Item()
                                .SemanticImage("Sample SVG image description")
                                .Svg("Resources/image.svg");
                        });
                });
            });
    }

    protected override SemanticTreeNode? GetExpectedSemanticTree()
    {
        return ExpectedSemanticTree.DocumentRoot(root =>
        {
            root.Child("H1", h1 => h1.Alt("Conformance Test: SVG"));
            root.Child("P");
            root.Child("Figure", figure => figure.Alt("Sample SVG image description"));
        });
    }
}