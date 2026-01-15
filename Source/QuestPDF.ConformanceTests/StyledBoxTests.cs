using QuestPDF.ConformanceTests.TestEngine;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.ConformanceTests;

internal class StyledBoxTests : ConformanceTestBase
{
    protected override Document GetDocumentUnderTest()
    {
        var avoidTransparency = TestContext.CurrentContext.Test.Arguments.FirstOrDefault() is PDFA_Conformance.PDFA_1A or PDFA_Conformance.PDFA_1B;
        
        return Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Margin(60);

                    page.Content()
                        .PaddingVertical(30)
                        .SemanticSection()
                        .Column(column =>
                        {
                            column.Spacing(30);

                            column.Item()
                                .SemanticHeader1()
                                .Text("Conformance Test: Styled Boxes")
                                .FontSize(36)
                                .Bold()
                                .FontColor(Colors.Blue.Darken2);

                            column.Item()
                                .Background(Colors.Blue.Lighten4)
                                .Padding(20)
                                .Text("Background only")
                                .FontSize(16);

                            column.Item()
                                .Border(2, Colors.Blue.Darken2)
                                .Padding(20)
                                .Text("Border only")
                                .FontSize(16);

                            column.Item()
                                .Background(Colors.White)
                                .Shadow(new BoxShadowStyle
                                {
                                    OffsetX = 5,
                                    OffsetY = 5,
                                    Blur = avoidTransparency ? 0 : 10,
                                    Spread = 5,
                                    Color = Colors.Grey.Medium
                                })
                                .Padding(20)
                                .Text("Simple shadow")
                                .FontSize(16);

                            column.Item()
                                .Border(1, Colors.Purple.Lighten4)
                                .Background(Colors.Purple.Lighten5)
                                .CornerRadius(15)
                                .Padding(20)
                                .Text("Rounded corners")
                                .FontSize(16);
                        });
                });
            });
    }

    protected override SemanticTreeNode? GetExpectedSemanticTree()
    {
        return ExpectedSemanticTree.DocumentRoot(root =>
        {
            root.Child("Sect", sect =>
            {
                sect.Child("H1", h1 => h1.Alt("Conformance Test: Styled Boxes"));

                foreach (var i in Enumerable.Range(1, 4))
                    sect.Child("P");
            });
        });
    }
}