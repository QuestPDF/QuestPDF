using QuestPDF.ConformanceTests.TestEngine;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace QuestPDF.ConformanceTests;

internal class MultiColumnTests : ConformanceTestBase
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
                        .MultiColumn(multiColumn =>
                        {
                            multiColumn.Spacing(75);
                            
                            multiColumn
                                .Spacer()
                                .PaddingHorizontal(25)
                                .Background(Colors.Blue.Lighten4)
                                .RotateLeft()
                                .AlignMiddle()
                                .AlignCenter()
                                .Text("This text should not be a part of the semantic tree")
                                .FontColor(Colors.Blue.Darken4)
                                .Bold();
                                
                            multiColumn
                                .Content()
                                .Column(column =>
                                {
                                    column.Spacing(25);
                                    
                                    foreach (var i in Enumerable.Range(1, 25))
                                    {
                                        column.Item()
                                            .AlignCenter()
                                            .Background(Colors.Grey.Lighten3)
                                            .Padding(10)
                                            .Text(text =>
                                            {
                                                text.Span($"Chapter {i}: ").Bold();
                                                text.Span(Placeholders.LoremIpsum());
                                            });
                                    }
                                });
                        });
                });
            });
    }
    
    protected override SemanticTreeNode? GetExpectedSemanticTree()
    {
        return ExpectedSemanticTree.DocumentRoot(root =>
        {
            foreach (var i in Enumerable.Range(1, 25))
                root.Child("P");
        });
    }
}