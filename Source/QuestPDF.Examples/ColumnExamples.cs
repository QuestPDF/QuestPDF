using System;
using System.Linq;
using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples
{
    public class ColumnExamples
    {
        [Test]
        public void Column()
        {
            RenderingTest
                .Create()
                .PageSize(PageSizes.A4)
                .ShowResults()
                .ProducePdf()
                .Render(container =>
                {
                    container.Column(column =>
                    {
                        foreach (var i in Enumerable.Range(0, 10))
                            column.Item().Element(Block);

                        static void Block(IContainer container)
                        {
                            container
                                .Width(72)
                                .Height(3.5f, Unit.Inch)
                                .Height(1.5f, Unit.Inch)
                                .Background(Placeholders.BackgroundColor());
                        }
                    });
                });
        }
        
        [Test]
        public void ColumnDoesNotPutDoubleSpacingWhenChildIsEmpty()
        {
            RenderingTest
                .Create()
                .PageSize(PageSizes.A4)
                .ShowResults()
                .ProducePdf()
                .Render(container =>
                {
                    container.Padding(50).Shrink().Border(1).Background(Colors.Grey.Lighten3).Column(column =>
                    {
                        column.Spacing(30);
                        
                        column.Item();
                        column.Item();
                        column.Item();
                        
                        foreach (var i in Enumerable.Range(0, 5))
                        {
                            column.Item().Element(Block);
                            column.Item().Element(Block);
                            column.Item().Element(Block);
                        
                            column.Item();
                        
                            column.Item().Element(Block);
                            column.Item().Element(Block);
                            column.Item().Element(Block);
                            column.Item().Element(Block);
                        
                            column.Item();
                            column.Item();
                        
                            column.Item().Element(Block);
                            column.Item().Element(Block);
                        
                            column.Item();
                            column.Item();
                        }
                        
                        static void Block(IContainer container)
                        {
                            container
                                .Width(100)
                                .Height(30)
                                .Background(Placeholders.Color());
                        }
                    });
                });
        }
        
        [Test]
        public void Stability_NoItems()
        {
            RenderingTest
                .Create()
                .ProducePdf()
                .MaxPages(100)
                .PageSize(250, 150)
                .Render(container =>
                {
                    container
                        .Padding(25)
                        .Column(column => { });
                });
        }
    }
}