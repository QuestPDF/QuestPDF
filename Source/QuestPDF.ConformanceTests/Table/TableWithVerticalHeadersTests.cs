using QuestPDF.ConformanceTests.TestEngine;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.ConformanceTests.Table;

internal class TableWithVerticalHeadersTests : ConformanceTestBase
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
                                columns.RelativeColumn();
                            });
                            
                            table.Header(header =>
                            {
                                header.Cell().Element(HeaderCellStyle).Text("Name");
                                header.Cell().Element(HeaderCellStyle).Text("Position");
                                header.Cell().Element(HeaderCellStyle).Text("Department");
                                header.Cell().Element(HeaderCellStyle).Text("Experience");
                            });

                            // Row 1:
                            table.Cell().Element(CellStyle).Text("John Smith");
                            table.Cell().Element(CellStyle).Text("Senior Developer");
                            table.Cell().Element(CellStyle).Text("Engineering");
                            table.Cell().Element(CellStyle).Text("5 years");
                            
                            // Row 2:
                            table.Cell().Element(CellStyle).Text("Jane Doe");
                            table.Cell().Element(CellStyle).Text("UX Designer");
                            table.Cell().Element(CellStyle).Text("Design");
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
                table.Child("THead", thead =>
                {
                    thead.Child("TR", row =>
                    {
                        row.Child("TH", th => th.Attribute("Table", "Scope", "Column").Child("P"));
                        row.Child("TH", th => th.Attribute("Table", "Scope", "Column").Child("P"));
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
                        row.Child("TD", td => td.Child("P"));
                        row.Child("TD", td => td.Child("P"));
                    });

                    tbody.Child("TR", row =>
                    {
                        row.Child("TD", td => td.Child("P"));
                        row.Child("TD", td => td.Child("P"));
                        row.Child("TD", td => td.Child("P"));
                        row.Child("TD", td => td.Child("P"));
                    });
                });
            });
        });
    }
}