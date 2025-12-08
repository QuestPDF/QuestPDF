using QuestPDF.ConformanceTests.TestEngine;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace QuestPDF.ConformanceTests;

internal class HeaderTests : ConformanceTestBase
{
    protected override Document GetDocumentUnderTest()
    {
        return Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Margin(60);

                    page.Header()
                        .Column(column =>
                        {
                            column.Spacing(25);
                            
                            column.Item()
                                .SemanticHeader1()
                                .Text("Conformance Test: Header")
                                .FontSize(24)
                                .Bold()
                                .FontColor(Colors.Blue.Darken2);

                            column.Item()
                                .ShowOnce()
                                .Text("Only the first page of the Header should be present in the semantic tree.");
                            
                            column.Item()
                                .SkipOnce()
                                .Text("This item should NOT be present in the semantic tree.");
                        });
                    
                    page.Content()
                        .PaddingTop(25)
                        .SemanticDivision()
                        .Column(column =>
                        {
                            column.Spacing(25);
                            
                            foreach (var i in Enumerable.Range(1, 12))
                            {
                                column.Item()
                                    .Width(200)
                                    .Height(100)
                                    .Background(Colors.Grey.Lighten2)
                                    .AlignCenter()
                                    .AlignMiddle()
                                    .Text($"Item {i}");
                            }
                        });
                });
            });
    }

    protected override SemanticTreeNode? GetExpectedSemanticTree()
    {
        return ExpectedSemanticTree.DocumentRoot(root =>
        {
            root.Child("H1", h1 => h1.Alt("Conformance Test: Header"));
            
            root.Child("P");

            root.Child("Div", div =>
            {
                foreach (var i in Enumerable.Range(1, 12))
                    div.Child("P");
            });
        });
    }
}