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
                        .Table(table =>
                        {
                            table.ApplySemanticTags();

                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            foreach (var i in Enumerable.Range(1, 30))
                            {
                                table.Cell().Element(CellStyle).Text($"{i}/1");
                                table.Cell().Element(CellStyle).Text($"{i}/2");
                                table.Cell().Element(CellStyle).Text($"{i}/3");
                            }
                            
                            table.Footer(footer =>
                            {
                                footer.Cell().Element(FooterCellStyle).Text("F11");
                                footer.Cell().Element(FooterCellStyle).Text("F12");
                                footer.Cell().Element(FooterCellStyle).Text("F13");
                                
                                footer.Cell().Element(FooterCellStyle).Text("F21");
                                footer.Cell().Element(FooterCellStyle).Text("F22");
                                footer.Cell().Element(FooterCellStyle).Text("F23");
                            });
                            
                            IContainer CellStyle(IContainer container) =>
                                container
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .Padding(8);
                            
                            IContainer FooterCellStyle(IContainer container) =>
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
                
                table.Child("TFoot", footer =>
                {
                    footer.Child("TR", tfoot =>
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
        });
    }
}