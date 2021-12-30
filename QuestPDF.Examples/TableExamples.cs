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
        public void TemperatureReport()
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
}