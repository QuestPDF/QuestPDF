using QuestPDF.ConformanceTests.TestEngine;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.ConformanceTests.Table;

/// <summary>
/// The SemanticTable method does not need to be applied directly on the container receiving the Table invocation.
/// Single-child styling containers (e.g. Padding, Border, Background) are allowed between them.
/// </summary>
internal class TableWithIntermediateContainersTests : ConformanceTestBase
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
                        .SemanticTable()
                        .Border(1)
                        .BorderColor(Colors.Grey.Darken1)
                        .Background(Colors.Grey.Lighten4)
                        .Padding(10)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Product");
                                header.Cell().Element(CellStyle).Text("Price");
                            });

                            table.Cell().Element(CellStyle).Text("Apple");
                            table.Cell().Element(CellStyle).Text("10");

                            table.Cell().Element(CellStyle).Text("Orange");
                            table.Cell().Element(CellStyle).Text("15");

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
                table.Child("THead", thead =>
                {
                    thead.Child("TR", row =>
                    {
                        row.Child("TH", th => th.Attribute("Table", "Scope", "Column").Child("P"));
                        row.Child("TH", th => th.Attribute("Table", "Scope", "Column").Child("P"));
                    });
                });

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
