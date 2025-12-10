using QuestPDF.ConformanceTests.TestEngine;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.ConformanceTests;

internal class HyperlinkTests : ConformanceTestBase
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
                                .SemanticHeader1()
                                .Text("Conformance Test: Hyperlinks")
                                .FontSize(24)
                                .Bold()
                                .FontColor(Colors.Blue.Darken2);
                            
                            column.Item().Text("Please find the link below:");
                            
                            column.Item()
                                .SemanticLink("Link to the QuestPDF website")
                                .Hyperlink("https://questpdf.com")
                                .Text("QuestPDF website")
                                .Underline()
                                .FontColor(Colors.Blue.Darken2);
                        });
                });
            });
    }
    
    protected override SemanticTreeNode? GetExpectedSemanticTree()
    {
        return ExpectedSemanticTree.DocumentRoot(root =>
        {
            root.Child("H1", h1 => h1.Alt("Conformance Test: Hyperlinks"));
            
            root.Child("P");
            
            root.Child("Link", link =>
            {
                link.Alt("Link to the QuestPDF website");
                link.Child("P");
            });
        });
    }
}