using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
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
                        .Box()
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

                            // by using the custom element, we can reuse the same style
                            table.Cell().Row(1).Column(4).Element(x => TextContent(x, "A"));
                            table.Cell().Row(2).Column(2).Element(x => TextContent(x, "A"));
                            table.Cell().Row(3).Column(3).Element(x => TextContent(x, "A"));
                            table.Cell().Row(4).Column(1).Element(x => TextContent(x, "A"));

                            static void TextContent(IContainer container, string text)
                            {
                                container
                                    .Border(1)
                                    .Background(Colors.Grey.Lighten3)
                                    .MinWidth(50)
                                    .MinHeight(50)
                                    .AlignCenter()
                                    .AlignMiddle()
                                    .Text(text);
                            }
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
                        .Box()
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

                            table.DefaultCellStyle(cell =>
                            {
                                return cell
                                    .Border(1)
                                    .Background(Colors.Grey.Lighten3)
                                    .MinWidth(50)
                                    .MinHeight(50)
                                    .AlignCenter()
                                    .AlignMiddle();
                            });
                            
                            table.Cell().Row(1).Column(1).Text("A");
                            table.Cell().Row(2).Column(2).Text("B");
                            table.Cell().Row(1).Column(3).Text("C");
                            table.Cell().Row(2).Column(4).Text("D");
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
                        .Box()
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

                            table.Cell().TextBox("A");
                            table.Cell().Row(2).Column(2).TextBox("B");
                            table.Cell().TextBox("C");
                            table.Cell().Row(3).Column(3).TextBox("D");
                            table.Cell().ColumnSpan(2).TextBox("E");
                        });
                });
        }
        
        [Test]
        public void ExtendLastCellsToTableBottom()
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
                        .Box()
                        .Border(1)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });
                            
                            table.ExtendLastCellsToTableBottom();

                            table.Cell().Row(1).Column(1).TextBox("A");
                            table.Cell().Row(3).Column(1).TextBox("B");
                            table.Cell().Row(2).Column(2).TextBox("C");
                            table.Cell().Row(3).Column(3).TextBox("D");
                        });
                });
        }
        
        [Test]
        public void Overlapping()
        {
            RenderingTest
                .Create()
                .ProduceImages()
                .PageSize(170, 120)
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(10)
                        .Box()
                        .Border(1)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Cell().RowSpan(2).ColumnSpan(2).Background(Colors.Green.Lighten3);
                            table.Cell().Background(Colors.Blue.Lighten3).MinHeight(50);
                            table.Cell().Row(2).Column(2).ColumnSpan(2).Background(Colors.Red.Lighten3).MinHeight(50);
                        });
                });
        }
        
        [Test]
        public void Spans()
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
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Cell().RowSpan(2).ColumnSpan(2).TextBox("1");
                            table.Cell().ColumnSpan(2).TextBox("2");
                            table.Cell().TextBox("3");
                            table.Cell().TextBox("4");
                            table.Cell().RowSpan(2).TextBox("5");
                            table.Cell().ColumnSpan(2).TextBox("6");
                            table.Cell().RowSpan(2).TextBox("7");
                            table.Cell().TextBox("8");
                            table.Cell().TextBox("9");
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
                            });

                            table.Cell().RowSpan(4).TextBox("1");
                            
                            table.Cell().RowSpan(2).TextBox("2");
                            table.Cell().RowSpan(1).TextBox("3");
                            table.Cell().RowSpan(1).TextBox("4");
                            
                            table.Cell().RowSpan(2).TextBox("5");
                            table.Cell().RowSpan(1).TextBox("6");
                            table.Cell().RowSpan(1).TextBox("7");
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
                        .Box()
                        .Border(1)
                        .Decoration(decoration =>
                        {
                            decoration
                                .Header()
                                .DefaultTextStyle(TextStyle.Default.SemiBold())
                                .Table(table =>
                                {
                                    table.ColumnsDefinition(DefineTableColumns);
                                    table.DefaultCellStyle(cell => DefineDefaultCellStyle(cell, Colors.Grey.Lighten3));
                                    
                                    table.Cell().RowSpan(2).ExtendHorizontal().AlignLeft().Text("Document type");
                                    
                                    table.Cell().ColumnSpan(2).Text("Inches");
                                    table.Cell().ColumnSpan(2).Text("Points");
                                    
                                    table.Cell().Text("Width");
                                    table.Cell().Text("Height");
                                    
                                    table.Cell().Text("Width");
                                    table.Cell().Text("Height");
                                });
                            
                            decoration
                                .Content()
                                .Table(table =>
                                {
                                    table.ColumnsDefinition(DefineTableColumns);
                                    table.DefaultCellStyle(cell => DefineDefaultCellStyle(cell, Colors.White));
                                    
                                    foreach (var page in pageSizes)
                                    {
                                        table.Cell().ExtendHorizontal().AlignLeft().Text(page.name);
                                        
                                        // inches
                                        table.Cell().Text(page.width);
                                        table.Cell().Text(page.height);
                                        
                                        // points
                                        table.Cell().Text(page.width * inchesToPoints);
                                        table.Cell().Text(page.height * inchesToPoints);
                                    }
                                });
   
                            void DefineTableColumns(TableColumnsDefinitionDescriptor columns)
                            {
                                columns.RelativeColumn();
                                
                                columns.ConstantColumn(75);
                                columns.ConstantColumn(75);
                                
                                columns.ConstantColumn(75);
                                columns.ConstantColumn(75);
                            }

                            IContainer DefineDefaultCellStyle(IContainer container, string backgroundColor)
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
                        });
                });
        }
        
        [Test]
        public void PerformanceText_TemperatureReport()
        {
            RenderingTest
                .Create()
                .ProduceImages()
                .PageSize(PageSizes.A4)
                .MaxPages(10_000)
                .EnableCaching()
                .EnableDebugging(false)
                .ShowResults()
                .Render(container => GeneratePerformanceStructure(container, 1));
        }
        
        public static void GeneratePerformanceStructure(IContainer container, int repeats)
        {
            container
                .Padding(25)
                //.Background(Colors.Blue.Lighten2)
                .Box()
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
    }
    
    public static class TableTestsExtensions
    {
        public static void TextBox(this IContainer container, string text)
        {
            container
                .Border(1)
                .Background(Colors.Grey.Lighten3)
                .MinWidth(50)
                .MinHeight(50)
                .AlignCenter()
                .AlignMiddle()
                .Text(text, TextStyle.Default.Size(16));
        }
    }
}