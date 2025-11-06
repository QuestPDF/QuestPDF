using QuestPDF.ConformanceTests.TestEngine;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.ConformanceTests.Table;

internal class TableWithHeaderCellsSpanningMultipleColumnsTests : ConformanceTestBase
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
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });
                            
                            table.Header(header =>
                            {
                                header.Cell().RowSpan(2).Element(HeaderCellStyle).Text("Paper Type");
                                header.Cell().ColumnSpan(2).Element(HeaderCellStyle).Text("Width");
                                header.Cell().ColumnSpan(2).Element(HeaderCellStyle).Text("Height");
                                header.Cell().Element(HeaderCellStyle).Text("Inches");
                                header.Cell().Element(HeaderCellStyle).Text("Points");
                                header.Cell().Element(HeaderCellStyle).Text("Inches");
                                header.Cell().Element(HeaderCellStyle).Text("Points");
                            });

                            table.Cell().AsSemanticHorizontalHeader().Element(HeaderCellStyle).Text("A3");
                            table.Cell().Element(CellStyle).Text(Placeholders.Decimal());
                            table.Cell().Element(CellStyle).Text(Placeholders.Decimal());
                            table.Cell().Element(CellStyle).Text(Placeholders.Decimal());
                            table.Cell().Element(CellStyle).Text(Placeholders.Decimal());
                                
                            table.Cell().AsSemanticHorizontalHeader().Element(HeaderCellStyle).Text("A4");
                            table.Cell().Element(CellStyle).Text(Placeholders.Decimal());
                            table.Cell().Element(CellStyle).Text(Placeholders.Decimal());
                            table.Cell().Element(CellStyle).Text(Placeholders.Decimal());
                            table.Cell().Element(CellStyle).Text(Placeholders.Decimal());
                            
                            table.Cell().AsSemanticHorizontalHeader().Element(HeaderCellStyle).Text("A5");
                            table.Cell().Element(CellStyle).Text(Placeholders.Decimal());
                            table.Cell().Element(CellStyle).Text(Placeholders.Decimal());
                            table.Cell().Element(CellStyle).Text(Placeholders.Decimal());
                            table.Cell().Element(CellStyle).Text(Placeholders.Decimal());
                            
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
                        row.Child("TH", th => th
                            .Id(5)
                            .Attribute("Table", "RowSpan", 2)
                            .Child("P"));
                        
                        row.Child("TH", th => th
                            .Id(6)
                            .Attribute("Table", "ColSpan", 2)
                            .Child("P"));
                        
                        row.Child("TH", th => th
                            .Id(7)
                            .Attribute("Table", "ColSpan", 2)
                            .Child("P"));
                    });
                    
                    thead.Child("TR", row =>
                    {
                        row.Child("TH", th => th
                            .Id(9)
                            .Attribute("Table", "Headers", new[] { 6 })
                            .Child("P"));
                        
                        row.Child("TH", th => th
                            .Id(10)
                            .Attribute("Table", "Headers", new[] { 6 })
                            .Child("P"));
                        
                        row.Child("TH", th => th
                            .Id(11)
                            .Attribute("Table", "Headers", new[] { 7 })
                            .Child("P"));
                        
                        row.Child("TH", th => th
                            .Id(12)
                            .Attribute("Table", "Headers", new[] { 7 })
                            .Child("P"));
                    });
                });

                table.Child("TBody", tbody =>
                {
                    tbody.Child("TR", row =>
                    {
                        row.Child("TH", th => th
                            .Id(22)
                            .Attribute("Table", "Headers", new[] { 5 })
                            .Child("P"));
                        
                        row.Child("TD", th => th
                            .Id(23)
                            .Attribute("Table", "Headers", new[] { 6, 9, 22 })
                            .Child("P"));
                        
                        row.Child("TD", td => td
                            .Id(24)
                            .Attribute("Table", "Headers", new[] { 6, 10, 22 })
                            .Child("P"));
                        
                        row.Child("TD", td => td
                            .Id(25)
                            .Attribute("Table", "Headers", new[] { 7, 11, 22 })
                            .Child("P"));
                        
                        row.Child("TD", td => td
                            .Id(26)
                            .Attribute("Table", "Headers", new[] { 7, 12, 22 })
                            .Child("P"));
                    });
                    
                    tbody.Child("TR", row =>
                    {
                        row.Child("TH", th => th
                            .Id(28)
                            .Attribute("Table", "Headers", new[] { 5 })
                            .Child("P"));
                        
                        row.Child("TD", th => th
                            .Id(29)
                            .Attribute("Table", "Headers", new[] { 6, 9, 28 })
                            .Child("P"));
                        
                        row.Child("TD", td => td
                            .Id(30)
                            .Attribute("Table", "Headers", new[] { 6, 10, 28 })
                            .Child("P"));
                        
                        row.Child("TD", td => td
                            .Id(31)
                            .Attribute("Table", "Headers", new[] { 7, 11, 28 })
                            .Child("P"));
                        
                        row.Child("TD", td => td
                            .Id(32)
                            .Attribute("Table", "Headers", new[] { 7, 12, 28 })
                            .Child("P"));
                    });
                    
                    tbody.Child("TR", row =>
                    {
                        row.Child("TH", th => th
                            .Id(34)
                            .Attribute("Table", "Headers", new[] { 5 })
                            .Child("P"));
                        
                        row.Child("TD", th => th
                            .Id(35)
                            .Attribute("Table", "Headers", new[] { 6, 9, 34 })
                            .Child("P"));
                        
                        row.Child("TD", td => td
                            .Id(36)
                            .Attribute("Table", "Headers", new[] { 6, 10, 34 })
                            .Child("P"));
                        
                        row.Child("TD", td => td
                            .Id(37)
                            .Attribute("Table", "Headers", new[] { 7, 11, 34 })
                            .Child("P"));
                        
                        row.Child("TD", td => td
                            .Id(38)
                            .Attribute("Table", "Headers", new[] { 7, 12, 34 })
                            .Child("P"));
                    });
                });
            });
        });
    }
}