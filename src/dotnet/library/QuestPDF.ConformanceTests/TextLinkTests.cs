using QuestPDF.ConformanceTests.TestEngine;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace QuestPDF.ConformanceTests;

internal class TextLinkTests : ConformanceTestBase
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
                            column.Spacing(15);

                            column.Item()
                                .Section("top")
                                .SemanticHeading1()
                                .Text("Conformance Test: Text Links")
                                .FontSize(24)
                                .Bold()
                                .FontColor(Colors.Blue.Darken2);

                            column.Item().Text(text =>
                            {
                                text.Span("Please visit the ");
                                text.Hyperlink("QuestPDF website", "https://www.questpdf.com").Underline().FontColor(Colors.Blue.Darken2);
                                text.Span(", or go back to ");
                                text.SectionLink("the top of the document", "top").Underline().FontColor(Colors.Blue.Darken2);
                                text.Span(".");
                            });
                        });
                });
            });
    }

    protected override SemanticTreeNode? GetExpectedSemanticTree()
    {
        return ExpectedSemanticTree.DocumentRoot(root =>
        {
            root.Child("H1", h1 => h1.Alt("Conformance Test: Text Links"));

            // each link is a Link element inside its paragraph, described by its text
            root.Child("P", paragraph =>
            {
                paragraph.Child("Link", link => link.Alt("QuestPDF website"));
                paragraph.Child("Link", link => link.Alt("the top of the document"));
            });
        });
    }
}
