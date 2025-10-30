using QuestPDF.ConformanceTests.TestEngine;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.ConformanceTests;

internal class LineTests : ConformanceTestBase
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
                                .Text("Conformance Test: Line Elements")
                                .FontSize(24)
                                .Bold()
                                .FontColor(Colors.Blue.Darken2);

                            column.Item()
                                .Text("Line elements should be rendered but semantically treated as artifacts.");

                            column.Item()
                                .LineHorizontal(2)
                                .LineColor(Colors.Red.Medium);

                            column.Item()
                                .Text(Placeholders.LoremIpsum());

                            column.Item()
                                .LineHorizontal(4)
                                .LineColor(Colors.Green.Medium)
                                .LineDashPattern([6, 6, 12, 6]);
    
                            column.Item()
                                .SemanticDivision()
                                .Background(Colors.Grey.Lighten3).Row(row =>
                                {
                                    row.RelativeItem()
                                        .PaddingVertical(25)
                                        .AlignRight()
                                        .Text("Text on the left side");
                                    
                                    row.AutoItem()
                                        .PaddingHorizontal(25)
                                        .LineVertical(4)
                                        .LineGradient([ Colors.Blue.Lighten2, Colors.Blue.Darken2 ]);
                                    
                                    row.RelativeItem()
                                        .PaddingVertical(25)
                                        .Text("Text on the right side");
                                });
                        });
                });
            });
    }
    
    protected override SemanticTreeNode? GetExpectedSemanticTree()
    {
        return ExpectedSemanticTree.DocumentRoot(root =>
        {
            root.Child("H1", h1 => h1.Alt("Conformance Test: Line Elements"));
            
            root.Child("P");
            root.Child("P");
            root.Child("Div", div =>
            {
                div.Child("P");
                div.Child("P");
            });
        });
    }
}