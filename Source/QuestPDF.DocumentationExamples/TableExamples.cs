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
                    page.MaxSize(new PageSize(500, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content()
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(50);
                                columns.RelativeColumn();
                                columns.ConstantColumn(125);
                            });
            
                            table.Header(header =>
                            {
                                header.Cell().BorderBottom(2).Padding(8).Text("#");
                                header.Cell().BorderBottom(2).Padding(8).Text("Product");
                                header.Cell().BorderBottom(2).Padding(8).AlignRight().Text("Price");
                            });
                            
                            foreach (var i in Enumerable.Range(0, 6))
                            {
                                var price = Math.Round(Random.Shared.NextDouble() * 100, 2);
                                 
                                table.Cell().Padding(8).Text($"{i + 1}");
                                table.Cell().Padding(8).Text(Placeholders.Label());
                                table.Cell().Padding(8).AlignRight().Text($"${price}");
                            }
                        });
                });
            })
            .GenerateImages(x => "table-simple.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void CellStyleExample()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(500, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    string[] weatherIcons = ["cloudy.svg", "lightning.svg", "pouring.svg", "rainy.svg", "snowy.svg", "windy.svg"];
                    
                    page.Content()
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.ConstantColumn(125);
                                columns.ConstantColumn(125);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Day");
                                header.Cell().Element(CellStyle).AlignCenter().Text("Weather");
                                header.Cell().Element(CellStyle).AlignRight().Text("Temp");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container
                                        .Background(Colors.Blue.Darken2)
                                        .DefaultTextStyle(x => x.FontColor(Colors.White).Bold())
                                        .PaddingVertical(8)
                                        .PaddingHorizontal(16);
                                }
                            });

                            foreach (var i in Enumerable.Range(0, 7))
                            {
                                var weatherIndex = Random.Shared.Next(0, weatherIcons.Length);

                                table.Cell().Element(CellStyle)
                                    .Text(new DateTime(2025, 2, 26).AddDays(i).ToString("dd MMMM"));
                                
                                table.Cell().Element(CellStyle).AlignCenter().Height(24)
                                    .Svg($"Resources/WeatherIcons/{weatherIcons[weatherIndex]}");
                                
                                table.Cell().Element(CellStyle).AlignRight()
                                    .Text($"{Random.Shared.Next(-10, 35)}°");

                                IContainer CellStyle(IContainer container)
                                {
                                    var backgroundColor = i % 2 == 0 
                                        ? Colors.Blue.Lighten5 
                                        : Colors.Blue.Lighten4;

                                    return container
                                        .Background(backgroundColor)
                                        .PaddingVertical(8)
                                        .PaddingHorizontal(16);
                                }
                            }
                        });
                });
            })
            .GenerateImages(x => "table-cell-style.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void OverlappingCells()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(700, 1000));
                    page.DefaultTextStyle(x => x.FontSize(16));
                    page.Margin(25);

                    string[] dayNames = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"];
                     
                    page.Content()
                        .Border(1)
                        .BorderColor(Colors.Grey.Lighten1)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                // hour column
                                columns.ConstantColumn(60);
                                
                                // day columns
                                foreach (var i in Enumerable.Range(0, 5))
                                    columns.RelativeColumn();
                            });
                            
                            // even/odd columns background
                            foreach (var column in Enumerable.Range(0, 7))
                            {
                                var backgroundColor = column % 2 == 0 ? Colors.Grey.Lighten3 : Colors.White;
                                table.Cell().Column((uint)column).RowSpan(24).Background(backgroundColor); 
                            }
                            
                            // hours and hour lines
                            foreach (var hour in Enumerable.Range(6, 10))
                            {
                                table.Cell().Column(1).Row((uint)hour)
                                    .PaddingVertical(5).PaddingHorizontal(10).AlignRight()
                                    .Text($"{hour}");
                                
                                table.Cell().Row((uint)hour).ColumnSpan(6)
                                    .Border(1).BorderColor(Colors.Grey.Lighten1).Height(20);
                            }
                            
                            // dates and day names
                            foreach (var i in Enumerable.Range(0, 5))
                            {
                                table.Cell()
                                    .Column((uint) i + 2).Row(1).Padding(5)
                                    .Column(column =>
                                    {
                                        column.Item().AlignCenter().Text($"{17  + i}").FontSize(24).Bold();
                                        column.Item().AlignCenter().Text(dayNames[i]).Light();
                                    });
                            }
                            
                            // standup events
                            foreach (var i in Enumerable.Range(1, 4))
                                AddEvent((uint)i, 8, 1, "Standup", Colors.Blue.Lighten4, Colors.Blue.Darken3);
                            
                            // other events
                            AddEvent(2, 11, 2, "Interview", Colors.Red.Lighten4, Colors.Red.Darken3);
                            AddEvent(3, 12, 3, "Demo", Colors.Red.Lighten4, Colors.Red.Darken3);
                            AddEvent(5, 5, 17, "PTO", Colors.Green.Lighten4, Colors.Green.Darken3);

                            void AddEvent(uint day, uint hour, uint length, string name, Color backgroundColor, Color textColor)
                            {
                                table.Cell()
                                    .Column(day + 1).Row(hour).RowSpan(length)
                                    .Padding(5).Background(backgroundColor).Padding(5)
                                    .AlignCenter().AlignMiddle() 
                                    .Text(name).FontColor(textColor);
                            }
                        });
                });
            })
            .GenerateImages(x => "table-overlapping-cells.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void ManualCellPlacement()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(700, 1000));
                    page.DefaultTextStyle(x => x.FontSize(16 ));
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
                                .Element(HeaderCellStyle)
                                .Text("Predicted condition").Bold();
                            
                            table.Cell().Row(3).Column(1).RowSpan(2)
                                .Element(HeaderCellStyle).RotateLeft()
                                .Text("Actual\ncondition").Bold().AlignCenter();
            
                            table.Cell().Row(2).Column(3)
                                .Element(HeaderCellStyle)
                                .Text("Positive (PP)");
                            
                            table.Cell().Row(2).Column(4)
                                .Element(HeaderCellStyle)
                                .Text("Negative (PN)");
            
                            table.Cell().Row(3).Column(2)
                                .Element(HeaderCellStyle).Text("Positive (P)");
                            
                            table.Cell().Row(4).Column(2)
                                .Element(HeaderCellStyle)
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
                                => container.Border(1).Background(color).PaddingHorizontal(10).PaddingVertical(15).AlignCenter().AlignMiddle();

                            static IContainer HeaderCellStyle(IContainer container) 
                                => CellStyle(container, Colors.Grey.Lighten4 );
                            
                            static IContainer GoodCellStyle(IContainer container) 
                                => CellStyle(container, Colors.Green.Lighten4).DefaultTextStyle(x => x.FontColor(Colors.Green.Darken2));
                            
                            static IContainer BadCellStyle(IContainer container) 
                                => CellStyle(container, Colors.Red.Lighten4).DefaultTextStyle(x => x.FontColor(Colors.Red.Darken2));
                        });
                });
            })
            .GenerateImages(x => "table-manual-cell-placement.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
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
                    page.DefaultTextStyle(x => x.FontSize(16));
                    page.Margin(25);

                    page.Content()
                        .Width(450)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(150);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(3);
                            });

                            table.Cell().ColumnSpan(3)
                                .Background(Colors.Grey.Lighten2).Element(CellStyle)
                                .Text("Total width: 450px");
                            
                            table.Cell().Element(CellStyle).Text("Constant: 150px");
                            table.Cell().Element(CellStyle).Text("Relative: 2*");
                            table.Cell().Element(CellStyle).Text("Relative: 3*");
                            
                            table.Cell().Element(CellStyle).Text("150px");
                            table.Cell().Element(CellStyle).Text("120px");
                            table.Cell().Element(CellStyle).Text("180px");

                            static IContainer CellStyle(IContainer container)
                                => container.Border(1).Padding(10);
                        });
                });
            })
            .GenerateImages(x => "table-columns-definition.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void HeaderAndFooter()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(600, 250));
                    page.DefaultTextStyle(x => x.FontSize(16));
                    page.Margin(25);

                    page.Content()
                        .Border(1)
                        .BorderColor(Colors.Grey.Lighten1)
                        .Table(table =>
                        {
                            var pageSizes = new List<(string name, double width, double height)>()
                            {
                                ("Letter (ANSI A)", 8.5f, 11),
                                ("Legal", 8.5f, 14),
                                ("Ledger (ANSI B)", 11, 17),
                                ("Tabloid (ANSI B)", 17, 11),
                                ("ANSI C", 22, 17),
                                ("ANSI D", 34, 22),
                                ("ANSI E", 44, 34)
                            };

                            const int inchesToPoints = 72;

                            IContainer DefaultCellStyle(IContainer container, string backgroundColor)
                            {
                                return container
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Lighten1)
                                    .Background(backgroundColor)
                                    .PaddingVertical(5)
                                    .PaddingHorizontal(10)
                                    .AlignCenter()
                                    .AlignMiddle();
                            }

                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();

                                columns.ConstantColumn(80);
                                columns.ConstantColumn(80);

                                columns.ConstantColumn(80);
                                columns.ConstantColumn(80);
                            });

                            table.Header(header =>
                            {
                                // please be sure to call the 'header' handler!

                                header.Cell().RowSpan(2).Element(CellStyle).ExtendHorizontal().AlignLeft()
                                    .Text("Document type").Bold();

                                header.Cell().ColumnSpan(2).Element(CellStyle).Text("Inches").Bold();
                                header.Cell().ColumnSpan(2).Element(CellStyle).Text("Points").Bold();

                                header.Cell().Element(CellStyle).Text("Width");
                                header.Cell().Element(CellStyle).Text("Height");

                                header.Cell().Element(CellStyle).Text("Width");
                                header.Cell().Element(CellStyle).Text("Height");

                                // you can extend existing styles by creating additional methods
                                IContainer CellStyle(IContainer container) =>
                                    DefaultCellStyle(container, Colors.Grey.Lighten3);
                            });

                            foreach (var page in pageSizes)
                            {
                                table.Cell().Element(CellStyle).ExtendHorizontal().AlignLeft().Text(page.name);

                                // inches
                                table.Cell().Element(CellStyle).Text(page.width);
                                table.Cell().Element(CellStyle).Text(page.height);

                                // points
                                table.Cell().Element(CellStyle).Text(page.width * inchesToPoints);
                                table.Cell().Element(CellStyle).Text(page.height * inchesToPoints);

                                IContainer CellStyle(IContainer container) =>
                                    DefaultCellStyle(container, Colors.White).ShowOnce();
                            }
                        });
                });
            })
            .GenerateImages(x => $"table-header-and-footer-{x}.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
}