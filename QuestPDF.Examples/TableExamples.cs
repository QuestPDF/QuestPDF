using System;
using System.ComponentModel;
using System.Diagnostics;
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
        public static Random Random { get; } = new Random();

        [Test]
        public void Example()
        {
            RenderingTest
                .Create()
                .ProduceImages()
                .PageSize(PageSizes.A4)
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(25)
                        .Box()
                        .Border(2) 
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(100);
                                columns.RelativeColumn();
                                columns.ConstantColumn(100);
                                columns.ConstantColumn(200);
                            });

                            table.Cell().ColumnSpan(2).Element(CreateBox("A"));
                            table.Cell().Element(CreateBox("B"));
                            table.Cell().Element(CreateBox("C"));
                            
                            table.Cell().Element(CreateBox("D"));
                            table.Cell().RowSpan(2).Element(CreateBox("E"));
                            table.Cell().RowSpan(3).ColumnSpan(2).Element(CreateBox("F"));
                            
                            table.Cell().RowSpan(2).Element(CreateBox("G"));
                            table.Cell().RowSpan(2).Element(CreateBox("H"));
                            table.Cell().Element(CreateBox("I"));
                            table.Cell().Element(CreateBox("J"));
                            table.Cell().RowSpan(2).Element(CreateBox("K"));
                            table.Cell().ColumnSpan(2).Element(CreateBox("L"));
                            table.Cell().Element(CreateBox("M"));
                        });
                });
        }
        
        [Test]
        public void TreeTable()
        {
            RenderingTest
                .Create()
                .ProducePdf()
                .PageSize(PageSizes.A4)
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(25)
                        .Box()
                        .Border(2) 
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(100);
                                columns.RelativeColumn(100);
                                columns.RelativeColumn(100);
                            });

                            table.Cell().RowSpan(4).Element(CreateBox("A"));
                            
                            table.Cell().RowSpan(2).Element(CreateBox("B"));
                            table.Cell().Element(CreateBox("C"));
                            table.Cell().Element(CreateBox("D"));
                            
                            table.Cell().RowSpan(2).Element(CreateBox("E"));
                            table.Cell().Element(CreateBox("F"));
                            table.Cell().Element(CreateBox("G"));
                        });
                });
        }
        
        [Test]
        public void TemperatureReport()
        {
            RenderingTest
                .Create()
                .ProducePdf()
                .PageSize(PageSizes.A4)
                .ShowResults()
                .Render(container => GeneratePerformanceStructure(container, 10));
        }
        
        [Test]
        public void TemperatureReport_PerformanceTest()
        {
            RenderingTest
                .Create()
                .ProducePdf()
                .PageSize(PageSizes.A4)
                .MaxPages(10000)
                .EnableCaching()
                .EnableDebugging(false)
                .ShowResults()
                .Render(container => GeneratePerformanceStructure(container, 250));
        }
        
        public static void GeneratePerformanceStructure(IContainer container, int repeats)
        {
            container
                .Padding(25)
                .Box()
                .Border(2) 
                .Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(100);
                        columns.RelativeColumn();
                        columns.ConstantColumn(100);
                        columns.RelativeColumn();
                    });

                    foreach (var i in Enumerable.Range(0, repeats))
                    {
                        table.Cell().RowSpan(3).LabelCell("Project");
                        table.Cell().RowSpan(3).ValueCell(Placeholders.Sentence());
                
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
        
        private Action<IContainer> CreateBox(string label)
        {
            return container =>
            {
                var height = Random.Next(2, 6) * 10;
                    
                container
                    .Background(Placeholders.BackgroundColor())
                    // .AlignCenter()
                    // .AlignMiddle()
                    .Height(height);
                    // .Text($"{label}: {height}px");
            };
        }
    }
}