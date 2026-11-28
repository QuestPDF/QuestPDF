using QuestPDF.ConformanceTests.TestEngine;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.ConformanceTests.Table;

internal class TableWithFooterTests : ConformanceTestBase
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
                        .Shrink()
                        .Border(1)
                        .BorderColor(Colors.Grey.Darken1)
                        .SemanticTable()
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            foreach (var i in Enumerable.Range(1, 30))
                            {
                                table.Cell().Style(Cell).Text($"{i}/1");
                                table.Cell().Style(Cell).Text($"{i}/2");
                                table.Cell().Style(Cell).Text($"{i}/3");
                            }
                            
                            table.Footer(footer =>
                            {
                                footer.Cell().Style(FooterCell).Text("F11");
                                footer.Cell().Style(FooterCell).Text("F12");
                                footer.Cell().Style(FooterCell).Text("F13");
                                
                                footer.Cell().Style(FooterCell).Text("F21");
                                footer.Cell().Style(FooterCell).Text("F22");
                                footer.Cell().Style(FooterCell).Text("F23");
                            });
                            
                            IContainer Cell(IContainer container) =>
                                container
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .Padding(8);
                            
                            IContainer FooterCell(IContainer container) =>
                                container
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .Background(Colors.Grey.Lighten3)
                                    .Padding(8);
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
                table.Child("TBody", tbody =>
                {
                    foreach (var i in Enumerable.Range(1, 30))
                    {
                        tbody.Child("TR", row =>
                        {
                            row.Child("TD", td => td.Child("P"));
                            row.Child("TD", td => td.Child("P"));
                            row.Child("TD", td => td.Child("P"));
                        });
                    }
                });
                
                table.Child("TFoot", tfoot =>
                {
                    tfoot.Child("TR", row =>
                    {
                        row.Child("TD", td => td.Child("P"));
                        row.Child("TD", td => td.Child("P"));
                        row.Child("TD", td => td.Child("P"));
                    });
                        
                    tfoot.Child("TR", row =>
                    {
                        row.Child("TD", td => td.Child("P"));
                        row.Child("TD", td => td.Child("P"));
                        row.Child("TD", td => td.Child("P"));
                    });
                });
            });
        });
    }
}