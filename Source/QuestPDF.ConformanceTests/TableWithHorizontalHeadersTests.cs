using QuestPDF.ConformanceTests.TestEngine;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.ConformanceTests;

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
                        .Table(table =>
                        {
                            table.ApplySemanticTags();

                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            // Row 1: Name
                            table.Cell().AsSemanticHorizontalHeader().Element(HeaderCellStyle).Text("Name");
                            table.Cell().Element(CellStyle).Text("John Smith");
                            table.Cell().Element(CellStyle).Text("Jane Doe");

                            // Row 2: Position
                            table.Cell().AsSemanticHorizontalHeader().Element(HeaderCellStyle).Text("Position");
                            table.Cell().Element(CellStyle).Text("Senior Developer");
                            table.Cell().Element(CellStyle).Text("UX Designer");

                            // Row 3: Department
                            table.Cell().AsSemanticHorizontalHeader().Element(HeaderCellStyle).Text("Department");
                            table.Cell().Element(CellStyle).Text("Engineering");
                            table.Cell().Element(CellStyle).Text("Design");

                            // Row 4: Experience
                            table.Cell().AsSemanticHorizontalHeader().Element(HeaderCellStyle).Text("Experience");
                            table.Cell().Element(CellStyle).Text("5 years");
                            table.Cell().Element(CellStyle).Text("3 years");

                            IContainer HeaderCellStyle(IContainer container) =>
                                container
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .Background(Colors.Grey.Lighten3)
                                    .Padding(8)
                                    .AlignMiddle()
                                    .DefaultTextStyle(x => x.Bold());

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