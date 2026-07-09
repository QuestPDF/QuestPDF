using QuestPDF.ConformanceTests.TestEngine;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.ConformanceTests;

internal class IgnoreTests : ConformanceTestBase
{
    protected override Document GetDocumentUnderTest()
    {
        var photo = File.ReadAllBytes("Resources/photo.jpeg");
        
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

                            column.Item().Text("This photo has semantic meaning:");
                            
                            column.Item()
                                .SemanticImage("A beautiful landscape")
                                .Image(photo);
                            
                            column.Item().Text("While this one doesn't:");
                            
                            column.Item()
                                .SemanticIgnore()
                                .Image(photo);
                        });
                });
            });
    }

    protected override SemanticTreeNode? GetExpectedSemanticTree()
    {
        return ExpectedSemanticTree.DocumentRoot(root =>
        {
            root.Child("P");
            root.Child("Figure", figure => figure.Alt("A beautiful landscape"));
            root.Child("P");
        });
    }
}