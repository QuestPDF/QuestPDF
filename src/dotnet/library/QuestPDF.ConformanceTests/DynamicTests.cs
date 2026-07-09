using QuestPDF.ConformanceTests.TestEngine;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.ConformanceTests;

internal class DynamicTests : ConformanceTestBase
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
                                column.Item().Dynamic(new DynamicComponent(i));
                            
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

    internal class DynamicComponent(int index) : IDynamicComponent
    {
        public DynamicComponentComposeResult Compose(DynamicContext context)
        {
            var result = context.CreateElement(container =>
            {
                container.SemanticArticle().Column(column =>
                {
                    column.Item().SemanticHeader3().Text($"Article {index}").Bold();

                    foreach (var j in Enumerable.Range(0, 10))
                    {
                        column.Item().Text($"{index} - {j}");
                    }
                });
            });

            if (result.Size.Height > context.AvailableSize.Height)
            {
                return new DynamicComponentComposeResult
                {
                    Content = context.CreateElement(container => { }),
                    HasMoreContent = true
                };
            }
            
            return new DynamicComponentComposeResult
            {
                Content = result,
                HasMoreContent = false
            };
        }
    }
}