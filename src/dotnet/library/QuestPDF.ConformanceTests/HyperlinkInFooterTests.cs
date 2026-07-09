using QuestPDF.ConformanceTests.TestEngine;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace QuestPDF.ConformanceTests;

internal class HyperlinkInFooterTests : ConformanceTestBase
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
                        .PaddingVertical(25)
                        .Column(column =>
                        {
                            column.Spacing(15);

                            column.Item()
                                .SemanticHeader1()
                                .Text("Conformance Test: Hyperlink in Footer")
                                .FontSize(24)
                                .Bold()
                                .FontColor(Colors.Blue.Darken2);
                            
                            column.Item().Text("Please find the link in the footer.");
                            
                            foreach (var i in Enumerable.Range(1, 25))
                                column.Item().Width(100).Height(50).Background(Colors.Grey.Lighten3);
                        });
                    
                    page.Footer()
                        .SemanticLink("Link to the QuestPDF website")
                        .Hyperlink("https://www.questpdf.com")
                        .Text("https://www.questpdf.com")
                        .Underline()
                        .FontColor(Colors.Blue.Darken2);
                });
            });
    }
    
    protected override SemanticTreeNode? GetExpectedSemanticTree()
    {
        return ExpectedSemanticTree.DocumentRoot(root =>
        {
            root.Child("H1", h1 => h1.Alt("Conformance Test: Hyperlink in Footer"));
            
            root.Child("P");
            
            // the first occurrence of footer content should be treated as content,
            // all subsequent occurrences should be marked as artifacts.
            root.Child("Link", link =>
            {
                link.Alt("Link to the QuestPDF website");
                link.Child("P");
            });
        });
    }
}