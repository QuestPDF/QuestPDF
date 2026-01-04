using QuestPDF.ConformanceTests.TestEngine;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.ConformanceTests;

internal class DynamicComponentSemanticTests : ConformanceTestBase
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
                            column.Item()
                                .SemanticHeader1()
                                .Text("Before Dynamic");
                            
                            // Add a second header before the dynamic component
                            // to make the expected node IDs more obvious
                            column.Item()
                                .SemanticHeader2()
                                .Text("Static Header");
                            
                            column.Item()
                                .Dynamic(new SimpleDynamicComponent());
                            
                            column.Item()
                                .SemanticHeader2()
                                .Text("After Dynamic");
                        });
                });
            });
    }
    
    protected override SemanticTreeNode? GetExpectedSemanticTree()
    {
        // Don't check node IDs for now, just structure
        return ExpectedSemanticTree.DocumentRoot(root =>
        {
            root.Child("H1", h1 => h1.Alt("Before Dynamic"));
            root.Child("H2", h2 => h2.Alt("Static Header"));
            root.Child("H2", h2 => h2.Alt("Dynamic Header"));
            root.Child("P");
            root.Child("H2", h2 => h2.Alt("After Dynamic"));
        });
    }
    
    public class SimpleDynamicComponent : IDynamicComponent
    {
        public DynamicComponentComposeResult Compose(DynamicContext context)
        {
            var content = context.CreateElement(container =>
            {
                container.Column(column =>
                {
                    column.Item()
                        .SemanticHeader2()
                        .Text("Dynamic Header");
                    
                    column.Item()
                        .SemanticParagraph()
                        .Text("Dynamic Content");
                });
            });

            return new DynamicComponentComposeResult
            {
                Content = content,
                HasMoreContent = false
            };
        }
    }
}
