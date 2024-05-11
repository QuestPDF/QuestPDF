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
        
        [Test]
        public void ColumnWithShowOnce()
        {
            RenderingTest
                .Create()
                .ProducePdf()
                .MaxPages(100)
                .ShowResults()
                .RenderDocument(document =>
                {
                    document.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(50);

                        page.Content().PaddingVertical(25).Column(column =>
                        {
                            column.Spacing(25);
                            
                            foreach (var i in Enumerable.Range(0, 50))
                                column.Item().Height(75).Width(100 + i * 5).Background(Colors.Grey.Lighten2);
                        });

                        page.Header().Background(Colors.Grey.Lighten4).Column(column =>
                        {
                            column.Spacing(10);
                            column.Item().Background(Colors.Red.Lighten3).Text("First line");
                            column.Item().Background(Colors.Green.Lighten3).ShowIf(x => x.PageNumber % 3 != 0).Text("Second line");
                            column.Item().Background(Colors.Blue.Lighten3).ShowIf(x => x.PageNumber % 2 != 0).Text("Third line");
                        });
                    });
                });
        }
    }
}