using QuestPDF.ConformanceTests.TestEngine;
using QuestPDF.Drawing;
using QuestPDF.Fluent;

namespace QuestPDF.ConformanceTests;

internal class LazyTests : ConformanceTestBase
{
    protected override Document GetDocumentUnderTest()
    {
        var imageData = File.ReadAllBytes("Resources/photo.jpeg");
        
        return Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Margin(60);

                    page.Content()
                        .PaddingVertical(30)
                        .Column(column =>
                        {
                            column.Spacing(25);

                            column.Item().SemanticHeader1().Text("Conformance Test: Lazy");
                            
                            column.Item().SemanticHeader2().Text("Before lazy");

                            foreach (var i in Enumerable.Range(0, 10))
                            {
                                column.Item()
                                    .Lazy(lazy =>
                                    {
                                        lazy.SemanticArticle().Column(innerColumn =>
                                        {
                                            innerColumn.Item().SemanticHeader3().Text($"Article {i}").Bold();
                                            
                                            foreach (var j in Enumerable.Range(0, 10))
                                            {
                                                innerColumn.Item().Text($"{i} - {j}");
                                            }
                                        });
                                    });
                            }
                            
                            column.Item().SemanticHeader2().Text("After lazy");
                        });
                });
            });
    }

    protected override SemanticTreeNode? GetExpectedSemanticTree()
    {
        return ExpectedSemanticTree.DocumentRoot(root =>
        {
            root.Child("H1", h1 => h1.Alt("Conformance Test: Lazy"));
            root.Child("H2", h2 => h2.Alt("Before lazy"));

            foreach (var i in Enumerable.Range(0, 10))
            {
                root.Child("Art", art =>
                {
                    art.Child("H3", h3 => h3.Alt($"Article {i}"));
         
                    foreach (var j in Enumerable.Range(0, 10))
                    {
                        art.Child("P");
                    }
                });
            }

            root.Child("H2", h2 => h2.Alt("After lazy"));
        });
    }
}