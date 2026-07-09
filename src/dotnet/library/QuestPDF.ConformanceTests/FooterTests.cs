using QuestPDF.ConformanceTests.TestEngine;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace QuestPDF.ConformanceTests;

internal class FooterTests : ConformanceTestBase
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
                        .PaddingBottom(25)
                        .Column(column =>
                        {
                            column.Spacing(25);

                            column.Item()
                                .SemanticHeader1()
                                .Text("Conformance Test: Footer")
                                .FontSize(24)
                                .Bold()
                                .FontColor(Colors.Blue.Darken2);

                            column.Item()
                                .Text("Footer content should not be present in the semantic tree.");

                            column.Item()
                                .SemanticDivision()
                                .Column(column =>
                                {
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

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.CurrentPageNumber();
                            text.Span(" / ");
                            text.TotalPages();
                        });
                });
            });
    }

    protected override SemanticTreeNode? GetExpectedSemanticTree()
    {
        return ExpectedSemanticTree.DocumentRoot(root =>
        {
            root.Child("H1", h1 => h1.Alt("Conformance Test: Footer"));
            
            root.Child("P");

            root.Child("Div", div =>
            {
                foreach (var i in Enumerable.Range(1, 12))
                    div.Child("P");
            });
        });
    }
}