using QuestPDF.ConformanceTests.TestEngine;
using QuestPDF.Drawing;
using QuestPDF.Fluent;

namespace QuestPDF.ConformanceTests;

internal class OrderOfSemanticItemsTests : ConformanceTestBase
{
    protected override Document GetDocumentUnderTest()
    {
        // this test checks if SemanticTag registers semantic content only in actual Skia drawing canvas
        
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
                                .Text("1 - H1");
                            
                            column.Item()
                                .SemanticHeader2()
                                .Text("2 - H2");
                            
                            column.Item()
                                .SemanticHeader2()
                                .Text("3 - H2");
                            
                            column.Item().MultiColumn(multiColumn =>
                            {
                                multiColumn.Spacing(75);
                            
                                multiColumn.Content().Column(column =>
                                {
                                    column.Item()
                                        .SemanticHeader2()
                                        .Text("4 - H2");
                                    
                                    column.Item()
                                        .SemanticHeader3()
                                        .Text("5 - H3");
                                    
                                    column.Item()
                                        .SemanticHeader3()
                                        .Text("6 - H3");
                                });
                            });
                        });
                });
            });
    }
    
    protected override SemanticTreeNode? GetExpectedSemanticTree()
    {
        return ExpectedSemanticTree.DocumentRoot(root =>
        {
            root.Child("H1", h1 => h1.Alt("1 - H1"));
            
            root.Child("H2", h2 => h2.Alt("2 - H2"));
            root.Child("H2", h2 => h2.Alt("3 - H2"));
            
            root.Child("H2", h2 => h2.Alt("4 - H2"));
            root.Child("H3", h3 => h3.Alt("5 - H3"));
            root.Child("H3", h3 => h3.Alt("6 - H3"));
        });
    }
}