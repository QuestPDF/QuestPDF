using QuestPDF.ConformanceTests.TestEngine;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace QuestPDF.ConformanceTests;

internal class TextLinkInFooterTests : ConformanceTestBase
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
                                .Section("top")
                                .SemanticHeading1()
                                .Text("Conformance Test: Text Links in Footer")
                                .FontSize(24)
                                .Bold()
                                .FontColor(Colors.Blue.Darken2);

                            column.Item().Text("Please find the links in the footer.");

                            foreach (var i in Enumerable.Range(1, 25))
                                column.Item().Width(100).Height(50).Background(Colors.Grey.Lighten3);
                        });

                    page.Footer().Text(text =>
                    {
                        text.SectionLink("Back to the top", "top").Underline().FontColor(Colors.Blue.Darken2);
                        text.Span(" | ");
                        text.Hyperlink("QuestPDF website", "https://www.questpdf.com").Underline().FontColor(Colors.Blue.Darken2);
                    });
                });
            });
    }

    protected override SemanticTreeNode? GetExpectedSemanticTree()
    {
        return ExpectedSemanticTree.DocumentRoot(root =>
        {
            root.Child("H1", h1 => h1.Alt("Conformance Test: Text Links in Footer"));

            root.Child("P");

            // the first occurrence of footer content should be treated as content,
            // all subsequent occurrences should be marked as artifacts, together with their links
            root.Child("P", paragraph =>
            {
                paragraph.Child("Link", link => link.Alt("Back to the top"));
                paragraph.Child("Link", link => link.Alt("QuestPDF website"));
            });
        });
    }
}
