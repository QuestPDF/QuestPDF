using QuestPDF.ConformanceTests.TestEngine;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.ConformanceTests;

internal class SemanticLinkAfterLinkTests : ConformanceTestBase
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
                                .Text("Conformance Test: SemanticLink After Link")
                                .FontSize(24)
                                .Bold()
                                .FontColor(Colors.Blue.Darken2);

                            column.Item().Text("Please find the links below and in the footer.");

                            column.Item()
                                .Hyperlink("https://www.questpdf.com")
                                .SemanticLink("Link to the QuestPDF website")
                                .Text("QuestPDF website")
                                .Underline()
                                .FontColor(Colors.Blue.Darken2);

                            column.Item()
                                .Lazy(lazy => lazy
                                    .Hyperlink("https://www.questpdf.com/api-reference/lazy.html")
                                    .SemanticLink("Link to the Lazy documentation")
                                    .Text("Lazy documentation"));

                            column.Item().Dynamic(new DynamicComponent());

                            foreach (var i in Enumerable.Range(1, 25))
                                column.Item().Width(100).Height(50).Background(Colors.Grey.Lighten3);
                        });

                    page.Footer()
                        .SectionLink("top")
                        .PaddingTop(5)
                        .SemanticLink("Link to the top of the document")
                        .Text("Back to the top")
                        .Underline()
                        .FontColor(Colors.Blue.Darken2);
                });
            });
    }

    protected override SemanticTreeNode? GetExpectedSemanticTree()
    {
        return ExpectedSemanticTree.DocumentRoot(root =>
        {
            root.Child("H1", h1 => h1.Alt("Conformance Test: SemanticLink After Link"));

            root.Child("P");

            // SemanticLink placed after Hyperlink or SectionLink gives the same result as placed before it
            root.Child("Link", link =>
            {
                link.Alt("Link to the QuestPDF website");
                link.Child("P");
            });

            root.Child("Link", link =>
            {
                link.Alt("Link to the Lazy documentation");
                link.Child("P");
            });

            root.Child("Link", link =>
            {
                link.Alt("Link to the Dynamic documentation");
                link.Child("P");
            });

            // the first occurrence of footer content should be treated as content,
            // all subsequent occurrences should be marked as artifacts
            root.Child("Link", link =>
            {
                link.Alt("Link to the top of the document");
                link.Child("P");
            });
        });
    }

    internal class DynamicComponent : IDynamicComponent
    {
        public DynamicComponentComposeResult Compose(DynamicContext context)
        {
            return new DynamicComponentComposeResult
            {
                Content = context.CreateElement(container => container
                    .Hyperlink("https://www.questpdf.com/api-reference/dynamic.html")
                    .SemanticLink("Link to the Dynamic documentation")
                    .Text("Dynamic documentation")),
                HasMoreContent = false
            };
        }
    }
}
