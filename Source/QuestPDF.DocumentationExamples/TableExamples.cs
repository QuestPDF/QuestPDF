using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class TableExamples
{
    [Test]
    public void Basic()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(700, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content()
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(75);
                                columns.ConstantColumn(150);
                                columns.ConstantColumn(200);
                                columns.ConstantColumn(200);
                            });
            
                            table.Cell().Row(1).Column(3).ColumnSpan(2)
                                .Element(HeaderCellStyle).AlignCenter()
                                .Text("Predicted condition").Bold();
                            
                            table.Cell().Row(3).Column(1).RowSpan(2)
                                .Element(HeaderCellStyle).RotateLeft().AlignCenter().AlignMiddle()
                                .Text("Actual\ncondition").Bold();
            
                            table.Cell().Row(2).Column(3)
                                .Element(HeaderCellStyle).AlignCenter()
                                .Text("Positive (PP)");
                            
                            table.Cell().Row(2).Column(4)
                                .Element(HeaderCellStyle).AlignCenter()
                                .Text("Negative (PN)");
            
                            table.Cell().Row(3).Column(2)
                                .Element(HeaderCellStyle).AlignMiddle().Text("Positive (P)");
                            
                            table.Cell().Row(4).Column(2)
                                .Element(HeaderCellStyle).AlignMiddle()
                                .Text("Negative (N)");
            
                            table.Cell()
                                .Row(3).Column(3).Element(GoodCellStyle)
                                .Text("True positive (TP)");
                            
                            table.Cell()
                                .Row(3).Column(4).Element(BadCellStyle)
                                .Text("False negative (FN)");
            
                            table.Cell().Row(4).Column(3)
                                .Element(BadCellStyle).Text("False positive (FP)");
                            
                            table.Cell().Row(4).Column(4)
                                .Element(GoodCellStyle).Text("True negative (TN)");

                            static IContainer CellStyle(IContainer container, Color color)
                                => container.Border(1).Background(color).PaddingHorizontal(10).PaddingVertical(15);

                            static IContainer HeaderCellStyle(IContainer container) 
                                => CellStyle(container, Colors.Grey.Lighten3);
                            
                            static IContainer GoodCellStyle(IContainer container) 
                                => CellStyle(container, Colors.Green.Lighten4).DefaultTextStyle(x => x.FontColor(Colors.Green.Darken2));
                            
                            static IContainer BadCellStyle(IContainer container) 
                                => CellStyle(container, Colors.Red.Lighten4).DefaultTextStyle(x => x.FontColor(Colors.Red.Darken2));
                        });
                });
            })
            .GenerateImages(x => "table-basic.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void ColumnsDefinition()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(700, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content()
                        .Width(325)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(100);
                                columns.RelativeColumn();
                                columns.RelativeColumn(2);
                            });

                            table.Cell().ColumnSpan(3)
                                .Background(Colors.Grey.Lighten2).Element(CellStyle)
                                .Text("Total width: 325px");
                            
                            table.Cell().Element(CellStyle).Text("C: 100");
                            table.Cell().Element(CellStyle).Text("R: 1");
                            table.Cell().Element(CellStyle).Text("R: 2");
                            
                            table.Cell().Element(CellStyle).Text("100px");
                            table.Cell().Element(CellStyle).Text("75px");
                            table.Cell().Element(CellStyle).Text("150px");

                            static IContainer CellStyle(IContainer container)
                                => container.Border(1).Padding(10);
                        });
                });
            })
            .GenerateImages(x => "table-columns-definition.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
}