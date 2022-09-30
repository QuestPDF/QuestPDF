using System.Linq;
using NUnit.Framework;
using QuestPDF.Elements;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples
{
    public class RightToLeftExamples
    {
        [Test]
        public void Unconstrained()
        {
            RenderingTest
                .Create()
                .ProduceImages()
                .PageSize(600, 600)
                .ShowResults()
                .Render(container =>
                {
                    container
                        .AlignCenter()
                        .AlignMiddle()
                        .ContentFromRightToLeft()
                        .Unconstrained()
                        .Background(Colors.Red.Medium)
                        .Height(200)
                        .Width(200);
                });
        }
        
        [Test]
        public void Row()
        {
            RenderingTest
                .Create()
                .ProduceImages()
                .PageSize(600, 600)
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(25)
                        .ContentFromRightToLeft()
                        .Border(1)
                        .Row(row =>
                        {
                            row.ConstantItem(200).Background(Colors.Red.Lighten2).Height(200);
                            row.ConstantItem(150).Background(Colors.Green.Lighten2).Height(200);
                            row.ConstantItem(100).Background(Colors.Blue.Lighten2).Height(200);
                        });
                });
        }
        
        [Test]
        public void MinimalBox()
        {
            RenderingTest
                .Create()
                .ProduceImages()
                .PageSize(600, 600)
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(25)
                        .ContentFromRightToLeft()
                        .Border(1)
                        .MinimalBox()
                        .Column(column =>
                        {
                            column.Item().Background(Colors.Red.Lighten2).Width(200).Height(200);
                            column.Item().Background(Colors.Green.Lighten2).Width(150).Height(150);
                            column.Item().Background(Colors.Blue.Lighten2).Width(100).Height(100);
                        });
                });
        }
        
        [Test]
        public void Table()
        {
            RenderingTest
                .Create()
                .ProduceImages()
                .PageSize(600, 600)
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(25)
                        .ContentFromRightToLeft()
                        .Border(1)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(200);
                                columns.ConstantColumn(150);
                                columns.ConstantColumn(100);
                            });

                            table.Cell().Background(Colors.Red.Lighten2).Height(200);
                            table.Cell().Background(Colors.Green.Lighten2).Height(200);
                            table.Cell().Background(Colors.Blue.Lighten2).Height(200);
                        });
                });
        }
        
        [Test]
        public void AspectRatio()
        {
            RenderingTest
                .Create()
                .ProduceImages()
                .PageSize(600, 600)
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(25)
                        .ContentFromRightToLeft()
                        .Border(1)
                        .AspectRatio(0.55f, AspectRatioOption.FitArea)
                        .Background(Colors.Red.Medium);
                });
        }
        
        [Test]
        public void Constrained()
        {
            RenderingTest
                .Create()
                .ProduceImages()
                .PageSize(600, 600)
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(25)
                        .ContentFromRightToLeft()
                        .Border(1)
                        .MaxWidth(100)
                        .Background(Colors.Red.Medium);
                });
        }
        
        [Test]
        public void Dynamic()
        {
            RenderingTest
                .Create()
                .ProduceImages()
                .PageSize(600, 600)
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(25)
                        .ContentFromRightToLeft()
                        .Dynamic(new SimpleDynamic());
                });
        }

        class SimpleDynamic : IDynamicComponent<int>
        {
            public int State { get; set; }
            
            public DynamicComponentComposeResult Compose(DynamicContext context)
            {
                var content = context.CreateElement(container =>
                {
                    container
                        .Row(row =>
                        {
                            row.ConstantItem(200).Background(Colors.Red.Lighten2).Height(200);
                            row.ConstantItem(150).Background(Colors.Green.Lighten2).Height(200);
                            row.ConstantItem(100).Background(Colors.Blue.Lighten2).Height(200);
                        });
                });
                
                return new DynamicComponentComposeResult
                {
                    Content = content,
                    HasMoreContent = false
                };
            }
        }
        
        [Test]
        public void Grid()
        {
            RenderingTest
                .Create()
                .ProduceImages()
                .PageSize(600, 600)
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(25)
                        .Border(1)
                        .ContentFromRightToLeft()
                        .Grid(grid =>
                        {
                            grid.Spacing(25);
                            grid.AlignRight();
                            
                            foreach (var i in Enumerable.Range(1, 6))
                            {
                                grid.Item(i).Background(Placeholders.BackgroundColor()).Height(100);
                            }
                        });
                });
        }
        
        [Test]
        public void Inlined()
        {
            RenderingTest
                .Create()
                .ProduceImages()
                .PageSize(600, 800)
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(25)
                        .Border(1)
                        .ContentFromRightToLeft()
                        .Inlined(inlined =>
                        {
                            inlined.Spacing(25);
                            
                            foreach (var i in Enumerable.Range(5, 10))
                            {
                                inlined.Item().Background(Placeholders.BackgroundColor()).Height(25 + i * 5).Width(i * 25);
                            }
                        });
                });
        }
        
        [Test]
        public void Text()
        {
            RenderingTest
                .Create()
                .ProduceImages()
                .PageSize(600, 600)
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(25)
                        .MinimalBox()
                        .Border(1)
                        .ContentFromRightToLeft()
                        .Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(32));
                            
                            foreach (var i in Enumerable.Range(1, 100))
                                text.Span($"{i}").FontColor(Placeholders.Color()).BackgroundColor("#2000");
                        });
                });
        }
    }
}