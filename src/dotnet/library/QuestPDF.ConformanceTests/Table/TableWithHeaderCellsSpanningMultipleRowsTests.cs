using QuestPDF.ConformanceTests.TestEngine;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.ConformanceTests.Table;

internal class TableWithHeaderCellsSpanningMultipleRowsTests : ConformanceTestBase
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
                            });
                            
                            table.Header(header =>
                            {
                                header.Cell().Element(HeaderCellStyle).Text("Year");
                                header.Cell().Element(HeaderCellStyle).Text("Quarter");
                                header.Cell().Element(HeaderCellStyle).Text("Outcome");
                                header.Cell().Element(HeaderCellStyle).Text("Income");
                            });

                            table.Cell().RowSpan(4).AsSemanticHorizontalHeader().Element(HeaderCellStyle).Text("2024");
                                
                            table.Cell().AsSemanticHorizontalHeader().Element(HeaderCellStyle).Text("Q1");
                            table.Cell().Element(CellStyle).Text(Placeholders.Price());
                            table.Cell().Element(CellStyle).Text(Placeholders.Price());
                                
                            table.Cell().AsSemanticHorizontalHeader().Element(HeaderCellStyle).Text("Q2");
                            table.Cell().Element(CellStyle).Text(Placeholders.Price());
                            table.Cell().Element(CellStyle).Text(Placeholders.Price());
                                
                            table.Cell().AsSemanticHorizontalHeader().Element(HeaderCellStyle).Text("Q3");
                            table.Cell().Element(CellStyle).Text(Placeholders.Price());
                            table.Cell().Element(CellStyle).Text(Placeholders.Price());
                                
                            table.Cell().AsSemanticHorizontalHeader().Element(HeaderCellStyle).Text("Q4");
                            table.Cell().Element(CellStyle).Text(Placeholders.Price());
                            table.Cell().Element(CellStyle).Text(Placeholders.Price());

                            table.Cell().RowSpan(2).AsSemanticHorizontalHeader().Element(HeaderCellStyle).Text("2025");

                            table.Cell().AsSemanticHorizontalHeader().Element(HeaderCellStyle).Text("Q1");
                            table.Cell().Element(CellStyle).Text(Placeholders.Price());
                            table.Cell().Element(CellStyle).Text(Placeholders.Price());
                                
                            table.Cell().AsSemanticHorizontalHeader().Element(HeaderCellStyle).Text("Q2");
                            table.Cell().Element(CellStyle).Text(Placeholders.Price());
                            table.Cell().Element(CellStyle).Text(Placeholders.Price());
                            
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
                        row.Child("TH", th => th.Id(5).Child("P"));
                        row.Child("TH", th => th.Id(6).Child("P"));
                        row.Child("TH", th => th.Id(7).Child("P"));
                        row.Child("TH", th => th.Id(8).Child("P"));
                    });
                });
                
                table.Child("TBody", tbody =>
                {
                    tbody.Child("TR", row =>
                    {
                        row.Child("TH", th => th
                            .Id(15)
                            .Attribute("Table", "RowSpan", 4)
                            .Attribute("Table", "Headers", new[] { 5 })
                            .Child("P"));
                        
                        row.Child("TH", th => th
                            .Id(16)
                            .Attribute("Table", "Headers", new[] { 6, 15 })
                            .Child("P"));
                        
                        row.Child("TD", td => td
                            .Id(17)
                            .Attribute("Table", "Headers", new[] { 7, 15, 16 })
                            .Child("P"));
                        
                        row.Child("TD", td => td
                            .Id(18)
                            .Attribute("Table", "Headers", new[] { 8, 15, 16 })
                            .Child("P"));
                    });
                    
                    tbody.Child("TR", row =>
                    {
                        row.Child("TH", th => th
                            .Id(20)
                            .Attribute("Table", "Headers", new[] { 6, 15 })
                            .Child("P"));
                        
                        row.Child("TD", td => td
                            .Id(21)
                            .Attribute("Table", "Headers", new[] { 7, 15, 20 })
                            .Child("P"));
                        
                        row.Child("TD", td => td
                            .Id(22)
                            .Attribute("Table", "Headers", new[] { 8, 15, 20 })
                            .Child("P"));
                    });
                    
                    tbody.Child("TR", row =>
                    {
                        row.Child("TH", th => th
                            .Id(24)
                            .Attribute("Table", "Headers", new[] { 6, 15 })
                            .Child("P"));
                        
                        row.Child("TD", td => td
                            .Id(25)
                            .Attribute("Table", "Headers", new[] { 7, 15, 24 })
                            .Child("P"));
                        
                        row.Child("TD", td => td
                            .Id(26)
                            .Attribute("Table", "Headers", new[] { 8, 15, 24 })
                            .Child("P"));
                    });
                    
                    tbody.Child("TR", row =>
                    {
                        row.Child("TH", th => th
                            .Id(28)
                            .Attribute("Table", "Headers", new[] { 6, 15 })
                            .Child("P"));
                        
                        row.Child("TD", td => td
                            .Id(29)
                            .Attribute("Table", "Headers", new[] { 7, 15, 28 })
                            .Child("P"));
                        
                        row.Child("TD", td => td
                            .Id(30)
                            .Attribute("Table", "Headers", new[] { 8, 15, 28 })
                            .Child("P"));
                    });

                    tbody.Child("TR", row =>
                    {
                        row.Child("TH", th => th
                            .Id(32)
                            .Attribute("Table", "RowSpan", 2)
                            .Attribute("Table", "Headers", new[] { 5 })
                            .Child("P"));
                        
                        row.Child("TH", th => th
                            .Id(33)
                            .Attribute("Table", "Headers", new[] { 6, 32 })
                            .Child("P"));
                        
                        row.Child("TD", td => td
                            .Id(34)
                            .Attribute("Table", "Headers", new[] { 7, 32, 33 })
                            .Child("P"));
                        
                        row.Child("TD", td => td
                            .Id(35)
                            .Attribute("Table", "Headers", new[] { 8, 32, 33 })
                            .Child("P"));
                    });
                    
                    tbody.Child("TR", row =>
                    {
                        row.Child("TH", th => th
                            .Id(37)
                            .Attribute("Table", "Headers", new[] { 6, 32 })
                            .Child("P"));
                        
                        row.Child("TD", td => td
                            .Id(38)
                            .Attribute("Table", "Headers", new[] { 7, 32, 37 })
                            .Child("P"));
                        
                        row.Child("TD", td => td
                            .Id(39)
                            .Attribute("Table", "Headers", new[] { 8, 32, 37 })
                            .Child("P"));
                    });
                });
            });
        });
    }
}