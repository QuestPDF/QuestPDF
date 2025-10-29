using QuestPDF.ConformanceTests.TestEngine;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.ConformanceTests;

internal class LineTests : ConformanceTestBase
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
                            column.Spacing(25);

                            column.Item().Text("Line tests");

                            column.Item()
                                .LineHorizontal(6)
                                .LineColor(Colors.Red.Medium);
                            
                            column.Item()
                                .LineHorizontal(6)
                                .LineColor(Colors.Green.Medium)
                                .LineDashPattern([6, 6, 12, 6]);
                            
                            column.Item()
                                .Height(150)
                                .LineVertical(6)
                                .LineGradient([ Colors.Blue.Lighten2, Colors.Blue.Darken2 ]);
                        });
                });
            })
            .WithMetadata(new DocumentMetadata
            {
                Language = "en-US",
                Title = "Conformance Test", 
                Subject = "Table of Contents"
            });
    }
    
    protected override SemanticTreeNode? GetExpectedSemanticTree()
    {
        return new SemanticTreeNode
        {
            NodeId = 1,
            Type = "Document",
            Children =
            {
                new SemanticTreeNode
                {
                    NodeId = 2,
                    Type = "P"
                }
            }
        };
    }
}