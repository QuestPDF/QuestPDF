using System;
using System.Linq;
using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

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
                            
                            // table.Cell().Row(1).Column(1).ColumnSpan(2).Element(CreateBox("A"));
                            // table.Cell().Row(1).Column(3).Element(CreateBox("B"));
                            // table.Cell().Row(1).Column(4).Element(CreateBox("C"));
                            //
                            // table.Cell().Row(2).Column(1).Element(CreateBox("D"));
                            // table.Cell().Row(2).RowSpan(2).Column(2).Element(CreateBox("E"));
                            // table.Cell().Row(2).RowSpan(3).Column(3).ColumnSpan(2).Element(CreateBox("F"));
                            //
                            // table.Cell().Row(3).RowSpan(2).Column(1).Element(CreateBox("G"));
                            // table.Cell().Row(4).RowSpan(2).Column(2).Element(CreateBox("H"));
                            // table.Cell().Row(5).Column(3).Element(CreateBox("I"));
                            // table.Cell().Row(5).Column(4).Element(CreateBox("J"));
                            // table.Cell().Row(5).RowSpan(2).Column(1).Element(CreateBox("K"));
                            // table.Cell().Row(6).Column(2).ColumnSpan(2).Element(CreateBox("L"));
                            // table.Cell().Row(6).Column(4).Element(CreateBox("M"));
                        });
                });
        }
        
        [Test]
        public void PerformanceTest()
        {
            RenderingTest
                .Create()
                .ProducePdf()
                .PageSize(1002, 2002)
                .MaxPages(1000)
                .EnableCaching()
                .EnableDebugging(false)
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(1)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                foreach (var size in Enumerable.Range(0, 10))
                                    columns.ConstantColumn(100);
                            });

                            foreach (var i in Enumerable.Range(1, 10_000))
                            {
                                table
                                    .Cell()
                                    .RowSpan((uint)Random.Next(1, 5))
                                    .ColumnSpan((uint)Random.Next(1, 5))
                                    .Element(CreateBox(i.ToString()));
                            }
                        });
                });
        }
        
        private Action<IContainer> CreateBox(string label)
        {
            return container =>
            {
                var height = Random.Next(2, 7) * 25;
                    
                container
                    .Border(2)
                    .Background(Placeholders.BackgroundColor())
                    .Layers(layers =>
                    {
                        layers
                            .PrimaryLayer()
                            .AlignCenter()
                            .AlignMiddle()
                            .Height(height)
                            .Width(80)
                            .Border(1);
                            
                        layers
                            .Layer()
                            .AlignCenter()
                            .AlignMiddle()
                            .Text($"{label}: {height}px");
                    });
            };
        }
    }
}