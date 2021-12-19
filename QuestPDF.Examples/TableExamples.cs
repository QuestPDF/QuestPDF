using System;
using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples
{
    public class TableExamples
    {
        public static Random Random { get; } = new Random(0);
        
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
                        .MaxHeight(500)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(100);
                                columns.RelativeColumn();
                                columns.ConstantColumn(100);
                                columns.ConstantColumn(200);
                            });

                            table.Cell().Row(1).Column(1).ColumnSpan(2).Element(CreateBox("A"));
                            table.Cell().Row(1).Column(3).Element(CreateBox("B"));
                            table.Cell().Row(1).Column(4).Element(CreateBox("C"));
                            
                            table.Cell().Row(2).Column(1).Element(CreateBox("D"));
                            table.Cell().Row(2).RowSpan(2).Column(2).Element(CreateBox("E"));
                            table.Cell().Row(2).RowSpan(3).Column(3).ColumnSpan(2).Element(CreateBox("F"));
                            
                            table.Cell().Row(3).RowSpan(2).Column(1).Element(CreateBox("G"));
                            table.Cell().Row(4).RowSpan(2).Column(2).Element(CreateBox("H"));
                            table.Cell().Row(5).Column(3).Element(CreateBox("I"));
                            table.Cell().Row(5).Column(4).Element(CreateBox("J"));
                            table.Cell().Row(5).RowSpan(2).Column(1).Element(CreateBox("K"));
                            table.Cell().Row(6).Column(2).ColumnSpan(2).Element(CreateBox("L"));
                            table.Cell().Row(6).Column(4).Element(CreateBox("M"));
                        });
                });

            Action<IContainer> CreateBox(string label)
            {
                return container =>
                {
                    var height = Random.Next(2, 7) * 25;
                    
                    container
                        .Border(1)
                        .Background(Placeholders.BackgroundColor())
                        .AlignCenter()
                        .AlignMiddle()
                        .Border(1)
                        .MinHeight(height)
                        .Text($"{label}: {height}");
                };
            }
        }
    }
}