using QuestPDF.ConformanceTests.TestEngine;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace QuestPDF.ConformanceTests;

internal class DecorationTests : ConformanceTestBase
{
    protected override Document GetDocumentUnderTest()
    {
        QuestPDF.Settings.EnableDebugging = true;
        
        return Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(600, 975);
                    page.Margin(50);

                    page.Content()
                        .Decoration(decoration =>
                        {
                            decoration.Before()
                                .Column(column =>
                                {
                                    column.Item()
                                        .ShowOnce()
                                        .Height(50)
                                        .Width(200)
                                        .SemanticImage("First page: decoration before")
                                        .Image(Placeholders.Image);
                                    
                                    column.Item()
                                        .SkipOnce()
                                        .Text("Second page: decoration before");
                                });
                            
                            decoration
                                .Content()
                                .PaddingVertical(25)
                                .Column(column =>
                                {
                                    column.Spacing(25);
                                    
                                    foreach (var i in Enumerable.Range(1, 15))
                                    {
                                        column.Item()
                                            .Width(200)
                                            .Height(50)
                                            .Background(Colors.Grey.Lighten3)
                                            .AlignCenter()
                                            .AlignMiddle()
                                            .Text($"Item {i}");
                                    }
                                });
                            
                            decoration.After()
                                .Column(column =>
                                {
                                    column.Item()
                                        .ShowOnce()
                                        .Height(50)
                                        .Width(200)
                                        .SemanticImage("First page: decoration after")
                                        .Image(Placeholders.Image);
                                    
                                    column.Item()
                                        .SkipOnce()
                                        .Text("Second page: decoration after");
                                });
                        });
                });
            });
    }

    protected override SemanticTreeNode? GetExpectedSemanticTree()
    {
        return ExpectedSemanticTree.DocumentRoot(root =>
        {
            root.Child("Figure", figure => figure.Alt("First page: decoration before"));
                
            foreach (var i in Enumerable.Range(1, 10))
                root.Child("P");
                
            root.Child("Figure", figure => figure.Alt("First page: decoration after"));
            
            foreach (var i in Enumerable.Range(1, 5))
                root.Child("P");
        });
    }
}