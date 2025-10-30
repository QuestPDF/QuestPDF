using QuestPDF.ConformanceTests.TestEngine;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.ConformanceTests.Table;

internal class TableWithoutHeadersTests : ConformanceTestBase
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

                            // Row 1
                            table.Cell().Element(CellStyle).Text("11");
                            table.Cell().Element(CellStyle).Text("12");
                            table.Cell().Element(CellStyle).Text("13");

                            // Row 2
                            table.Cell().Element(CellStyle).Text("21");
                            table.Cell().Element(CellStyle).Text("22");
                            table.Cell().Element(CellStyle).Text("23");

                            // Row 3
                            table.Cell().Element(CellStyle).Text("31");
                            table.Cell().Element(CellStyle).Text("32");
                            table.Cell().Element(CellStyle).Text("33");

                            // Row 4
                            table.Cell().Element(CellStyle).Text("41");
                            table.Cell().Element(CellStyle).Text("42");
                            table.Cell().Element(CellStyle).Text("43");
                            
                            IContainer CellStyle(IContainer container) =>
                                container
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Lighten2)
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
                    tbody.Child("TR", row =>
                    {
                        row.Child("TD", th => th.Child("P"));
                        row.Child("TD", td => td.Child("P"));
                        row.Child("TD", td => td.Child("P"));
                    });

                    tbody.Child("TR", row =>
                    {
                        row.Child("TD", th => th.Child("P"));
                        row.Child("TD", td => td.Child("P"));
                        row.Child("TD", td => td.Child("P"));
                    });

                    tbody.Child("TR", row =>
                    {
                        row.Child("TD", th => th.Child("P"));
                        row.Child("TD", td => td.Child("P"));
                        row.Child("TD", td => td.Child("P"));
                    });

                    tbody.Child("TR", row =>
                    {
                        row.Child("TD", th => th.Child("P"));
                        row.Child("TD", td => td.Child("P"));
                        row.Child("TD", td => td.Child("P"));
                    });
                });
            });
        });
    }
}