using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Color = QuestPDF.Infrastructure.Color;
using IContainer = QuestPDF.Infrastructure.IContainer;

namespace QuestPDF.Examples
{
    public class TableExamples
    {
        [Test]
        public void BasicPlacement()
        {
            RenderingTest
                .Create()
                .ProduceImages()
                .PageSize(220, 220)
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(10)
                        .MinimalBox()
                        .Border(1)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            // by using custom 'Element' method, we can reuse visual configuration
                            table.Cell().Row(1).Column(4).Element(Block).Text("A");
                            table.Cell().Row(2).Column(2).Element(Block).Text("B");
                            table.Cell().Row(3).Column(3).Element(Block).Text("C");
                            table.Cell().Row(4).Column(1).Element(Block).Text("D");
                        });
                });
        }
        
        [Test]
        public void PagingSupport()
        {
            RenderingTest
                .Create()
                .ProducePdf()
                .PageSize(420, 220)
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(10)
                        .MinimalBox()
                        .Border(1)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            // by using custom 'Element' method, we can reuse visual configuration
                            table.Cell().Element(Block).Text(Placeholders.Label());
                            table.Cell().Element(Block).Text(Placeholders.Label());
                            table.Cell().Element(Block).Text(Placeholders.Paragraph());
                            table.Cell().Element(Block).Text(Placeholders.Label());
                        });
                });
        }
        
        [Test]
        public void DefaultCellStyle()
        {
            RenderingTest
                .Create()
                .ProduceImages()
                .PageSize(220, 120)
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(10)
                        .MinimalBox()
                        .Border(1)
                        .DefaultTextStyle(TextStyle.Default.Size(16))
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Cell().Row(1).Column(1).Element(Block).Text("A");
                            table.Cell().Row(2).Column(2).Element(Block).Text("B");
                            table.Cell().Row(1).Column(3).Element(Block).Text("C");
                            table.Cell().Row(2).Column(4).Element(Block).Text("D");
                        });
                });
        }
        
        [Test]
        public void ColumnsDefinition()
        {
            RenderingTest
                .Create()
                .ProduceImages()
                .PageSize(320, 80)
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(10)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(50);
                                columns.ConstantColumn(100);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(3);
                            });

                            table.Cell().ColumnSpan(4).LabelCell("Total width: 300px");
                            table.Cell().ValueCell("50px");
                            table.Cell().ValueCell("100px");
                            table.Cell().ValueCell("100px");
                            table.Cell().ValueCell("150px");
                        });
                });
        }

        [Test]
        public void PartialAutoPlacement()
        {
            RenderingTest
                .Create()
                .ProduceImages()
                .PageSize(220, 220)
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(10)
                        .MinimalBox()
                        .Border(1)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Cell().Element(Block).Text("A");
                            table.Cell().Row(2).Column(2).Element(Block).Text("B");
                            table.Cell().Element(Block).Text("C");
                            table.Cell().Row(3).Column(3).Element(Block).Text("D");
                            table.Cell().ColumnSpan(2).Element(Block).Text("E");
                        });
                });
        }
        
        [Test]
        public void ExtendLastCellsToTableBottom()
        {
            RenderingTest
                .Create()
                .ProduceImages()
                .PageSize(220, 170)
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(10)
                        .MinimalBox()
                        .Border(1)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });
                            
                            table.ExtendLastCellsToTableBottom();

                            table.Cell().Row(1).Column(1).Element(Block).Text("A");
                            table.Cell().Row(3).Column(1).Element(Block).Text("B");
                            table.Cell().Row(2).Column(2).Element(Block).Text("C");
                            table.Cell().Row(3).Column(3).Element(Block).Text("D");
                            table.Cell().Row(2).RowSpan(2).Column(4).Element(Block).Text("E");
                        });
                });
        }
        
        [Test]
        public void Overlapping()
        {
            RenderingTest
                .Create()
                .ProduceImages()
                .PageSize(170, 170)
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(10)
                        .MinimalBox()
                        .Border(1)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Cell().Row(1).RowSpan(3).Column(1).ColumnSpan(3).Background(Colors.Grey.Lighten3).MinHeight(150);
                            table.Cell().Row(1).RowSpan(2).Column(1).ColumnSpan(2).Background(Colors.Grey.Lighten1).MinHeight(100);
                            table.Cell().Row(3).Column(3).Background(Colors.Grey.Darken1).MinHeight(50);
                        });
                });
        }
        
        [Test]
        public void Spans()
        {
            RenderingTest
                .Create()
                .ProduceImages()
                .PageSize(220, 170)
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(10)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Cell().RowSpan(2).ColumnSpan(2).Element(Block).Text("1");
                            table.Cell().ColumnSpan(2).Element(Block).Text("2");
                            table.Cell().Element(Block).Text("3");
                            table.Cell().Element(Block).Text("4");
                            table.Cell().RowSpan(2).Element(Block).Text("5");
                            table.Cell().ColumnSpan(2).Element(Block).Text("6");
                            table.Cell().RowSpan(2).Element(Block).Text("7");
                            table.Cell().Element(Block).Text("8");
                            table.Cell().Element(Block).Text("9");
                        });
                });
        }

        [Test]
        public void Stability()
        {
            RenderingTest
                .Create()
                .ProduceImages()
                .PageSize(300, 300)
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(10)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            var image = Placeholders.Image(400, 300);
                            
                            table.Cell().Image(image);
                            table.Cell().Image(image);
                            table.Cell().Image(image);
                            table.Cell().Image(image);
                            table.Cell().Image(image);
                            table.Cell().Image(image);
                        });
                });
        }
        
        [Test]
        public void TableContinued()
        {
            RenderingTest
                .Create()
                .ProducePdf()
                .PageSize(PageSizes.A5)
                .ShowResults()
                .Render(container =>
                {
                    var content = new[]
                    {
                        ("Label 1", "Value 234"),
                        ("Label 12", "Value 34"),
                        ("Label 123", "Value 4")
                    };
                    
                    container
                        .Padding(20)
                        .AlignRight()
                        .Column(column =>
                        {
                            foreach (var value in content)
                            {
                                column.Item().AlignRight().Row(row =>
                                {
                                    row.AutoItem().Text($"{value.Item1}:").Bold();
                                    row.ConstantItem(75).AlignRight().Text(value.Item2);
                                });
                            }
                        });
                });
        }
        
        [Test]
        public void Bug_RowSpanWorksIncorrectly()
        {
            // https://github.com/QuestPDF/QuestPDF/issues/552
            
            RenderingTest
                .Create()
                .ProduceImages()
                .PageSize(PageSizes.A5.Landscape())
                .ShowResults()
                .Render(container =>
                {
                    container.Padding(20).Table(table => 
                    {
                        table.ColumnsDefinition(columns => 
                        {
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                        });

                        foreach (var i in Enumerable.Range(6, 9))
                        {
                            table.Cell().Element(CellStyleMainTable).Text($"{i:00}:00");
                            
                            foreach (var j in Enumerable.Range(1, 3))
                                table.Cell().Element(CellStyleMainTable).Text("");
                        }

                        static IContainer CellStyleMainTable(IContainer container) 
                        {
                            return container
                                .Border(0.5f).BorderColor(Colors.Blue.Lighten4)
                                .Background(Colors.Blue.Lighten5)
                                .PaddingVertical(5);
                        }

                        table.Cell().Row(1).RowSpan(3).Column(2).Element(BlockAccepted).Text("3 rows");
                        table.Cell().Row(1).RowSpan(6).Column(3).Element(BlockAccepted).Text("6 rows");
                        table.Cell().Row(3).RowSpan(5).Column(4).Element(BlockAccepted).Text("5 rows");
                        
                        static IContainer BlockAccepted(IContainer container)
                        {
                            return container
                                .Border(1.5f).BorderColor(Colors.Green.Lighten2)
                                .Background(Colors.Green.Lighten4);
                        }
                    });
                });
        }
        
        [Test]
        public void TableHeader()
        {
            RenderingTest
                .Create()
                .ProduceImages()
                .PageSize(500, 200)
                .ShowResults()
                .EnableDebugging()
                .Render(container =>
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
                    
                    container
                        .Padding(10)
                        .MinimalBox()
                        .Border(1)
                        .Table(table =>
                        {
                            IContainer DefaultCellStyle(IContainer container, Color backgroundColor)
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
                                
                                columns.ConstantColumn(75);
                                columns.ConstantColumn(75);
                                
                                columns.ConstantColumn(75);
                                columns.ConstantColumn(75);
                            });
                            
                            table.Header(header =>
                            {
                                header.Cell().RowSpan(2).Element(CellStyle).ExtendHorizontal().AlignLeft().Text("Document type");
                                    
                                header.Cell().ColumnSpan(2).Element(CellStyle).Text("Inches");
                                header.Cell().ColumnSpan(2).Element(CellStyle).Text("Points");
                                    
                                header.Cell().Element(CellStyle).Text("Width");
                                header.Cell().Element(CellStyle).Text("Height");
                                    
                                header.Cell().Element(CellStyle).Text("Width");
                                header.Cell().Element(CellStyle).Text("Height");

                                // you can extend already existing styles by creating additional methods
                                IContainer CellStyle(IContainer container) => DefaultCellStyle(container, Colors.Grey.Lighten3); 
                            });
        
                            foreach (var page in pageSizes)
                            {
                                table.Cell().Element(CellStyle).ExtendHorizontal().AlignLeft().Text(page.name);
                                        
                                // inches
                                table.Cell().Element(CellStyle).Text(page.width.ToString(CultureInfo.InvariantCulture));
                                table.Cell().Element(CellStyle).Text(page.height.ToString(CultureInfo.InvariantCulture));
                                        
                                // points
                                table.Cell().Element(CellStyle).Text((page.width * inchesToPoints).ToString(CultureInfo.InvariantCulture));
                                table.Cell().Element(CellStyle).Text((page.height * inchesToPoints).ToString(CultureInfo.InvariantCulture));
                                        
                                IContainer CellStyle(IContainer container) => DefaultCellStyle(container, Colors.White); 
                            }
                        });
                });
        }
        
        [Test]
        public void PerformanceText_TemperatureReport()
        {
            RenderingTest
                .Create()
                .ProducePdf()
                .PageSize(PageSizes.A4)
                .MaxPages(10_000)
                .EnableCaching()
                .EnableDebugging(false)
                .ShowResults()
                .Render(container => GeneratePerformanceStructure(container, 250));
        }
        
        public static void GeneratePerformanceStructure(IContainer container, int repeats)
        {
            container
                .Padding(25)
                //.Background(Colors.Blue.Lighten2)
                .MinimalBox()
                .Border(1)
                //.Background(Colors.Red.Lighten2)
                .Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(100);
                        columns.RelativeColumn();
                        columns.ConstantColumn(100);
                        columns.RelativeColumn();
                    });
                    
                    table.ExtendLastCellsToTableBottom();

                    foreach (var i in Enumerable.Range(0, repeats))
                    {
                        table.Cell().RowSpan(3).LabelCell("Project");
                        table.Cell().RowSpan(3).ShowEntire().ValueCell(Placeholders.Sentence());
                
                        table.Cell().LabelCell("Report number");
                        table.Cell().ValueCell(i.ToString());
                        
                        table.Cell().LabelCell("Date");
                        table.Cell().ValueCell(Placeholders.ShortDate());

                        table.Cell().LabelCell("Inspector");
                        table.Cell().ValueCell("Marcin Ziąbek");
                
                        table.Cell().ColumnSpan(2).LabelCell("Morning weather");
                        table.Cell().ColumnSpan(2).LabelCell("Evening weather");
                
                        table.Cell().ValueCell("Time");
                        table.Cell().ValueCell("7:13");
                
                        table.Cell().ValueCell("Time");
                        table.Cell().ValueCell("18:25");
                
                        table.Cell().ValueCell("Description");
                        table.Cell().ValueCell("Sunny");
                
                        table.Cell().ValueCell("Description");
                        table.Cell().ValueCell("Windy");
                
                        table.Cell().ValueCell("Wind");
                        table.Cell().ValueCell("Mild");
                
                        table.Cell().ValueCell("Wind");
                        table.Cell().ValueCell("Strong");
                
                        table.Cell().ValueCell("Temperature");
                        table.Cell().ValueCell("17°C");
                
                        table.Cell().ValueCell("Temperature");
                        table.Cell().ValueCell("32°C");
                
                        table.Cell().LabelCell("Remarks");
                        table.Cell().ColumnSpan(3).ValueCell(Placeholders.Paragraph());
                    }
                });
        }
        
        // this method uses a higher order function to define a custom and dynamic style
        static IContainer Block(IContainer container)
        {
            return container
                .Border(1)
                .Background(Colors.Grey.Lighten3)
                .ShowOnce()
                .MinWidth(50)
                .MinHeight(50)
                .Padding(10)
                .AlignCenter()
                .AlignMiddle();
        }
    }
}