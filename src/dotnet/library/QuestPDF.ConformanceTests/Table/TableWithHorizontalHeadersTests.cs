using QuestPDF.ConformanceTests.TestEngine;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.ConformanceTests.Table;

internal class TableWithHorizontalHeadersTests : ConformanceTestBase
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

                            // Row 1: Name
                            table.Cell().AsSemanticHorizontalHeader().Style(HeaderCell).Text("Name");
                            table.Cell().Style(Cell).Text("John Smith");
                            table.Cell().Style(Cell).Text("Jane Doe");

                            // Row 2: Position
                            table.Cell().AsSemanticHorizontalHeader().Style(HeaderCell).Text("Position");
                            table.Cell().Style(Cell).Text("Senior Developer");
                            table.Cell().Style(Cell).Text("UX Designer");

                            // Row 3: Department
                            table.Cell().AsSemanticHorizontalHeader().Style(HeaderCell).Text("Department");
                            table.Cell().Style(Cell).Text("Engineering");
                            table.Cell().Style(Cell).Text("Design");

                            // Row 4: Experience
                            table.Cell().AsSemanticHorizontalHeader().Style(HeaderCell).Text("Experience");
                            table.Cell().Style(Cell).Text("5 years");
                            table.Cell().Style(Cell).Text("3 years");

                            IContainer HeaderCell(IContainer container) =>
                                container
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .Background(Colors.Grey.Lighten3)
                                    .Padding(8)
                                    .AlignMiddle()
                                    .DefaultTextStyle(x => x.Bold());

                            IContainer Cell(IContainer container) =>
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
                        row.Child("TH", th => th.Attribute("Table", "Scope", "Row").Child("P"));
                        row.Child("TD", td => td.Child("P"));
                        row.Child("TD", td => td.Child("P"));
                    });

                    tbody.Child("TR", row =>
                    {
                        row.Child("TH", th => th.Attribute("Table", "Scope", "Row").Child("P"));
                        row.Child("TD", td => td.Child("P"));
                        row.Child("TD", td => td.Child("P"));
                    });

                    tbody.Child("TR", row =>
                    {
                        row.Child("TH", th => th.Attribute("Table", "Scope", "Row").Child("P"));
                        row.Child("TD", td => td.Child("P"));
                        row.Child("TD", td => td.Child("P"));
                    });

                    tbody.Child("TR", row =>
                    {
                        row.Child("TH", th => th.Attribute("Table", "Scope", "Row").Child("P"));
                        row.Child("TD", td => td.Child("P"));
                        row.Child("TD", td => td.Child("P"));
                    });
                });
            });
        });
    }
}