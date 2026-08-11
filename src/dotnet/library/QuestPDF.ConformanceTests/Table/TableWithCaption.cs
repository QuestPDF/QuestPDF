using QuestPDF.ConformanceTests.TestEngine;
using QuestPDF.Drawing;
using QuestPDF.Fluent;

namespace QuestPDF.ConformanceTests.Table;

internal class TableWithCaption : ConformanceTestBase
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
                        .SemanticTable()
                        .Decoration(decoration =>
                        {
                            decoration
                                .Before()
                                .SemanticCaption()
                                .Text("Table caption");
                            
                            decoration
                                .Content()
                                .Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });
                                    
                                    table.Cell().Text("A1");
                                    table.Cell().Text("B1");
                                    table.Cell().Text("A2");
                                    table.Cell().Text("B2");
                                });
                        });
                });
            });
    }

    protected override SemanticTreeNode? GetExpectedSemanticTree()
    {
        return ExpectedSemanticTree.DocumentRoot(root =>
        {
            root.Child("Table", table =>
            {
                table.Child("Caption", caption => caption.Child("P"));
                
                table.Child("TBody", tbody =>
                {
                    tbody.Child("TR", row =>
                    {
                        row.Child("TD", td => td.Child("P"));
                        row.Child("TD", td => td.Child("P"));
                    });
                    
                    tbody.Child("TR", row =>
                    {
                        row.Child("TD", td => td.Child("P"));
                        row.Child("TD", td => td.Child("P"));
                    });
                });
            });
        });
    }
}