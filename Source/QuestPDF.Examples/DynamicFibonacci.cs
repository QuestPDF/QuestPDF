using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using QuestPDF.Elements;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples
{
    public struct FibonacciHeaderState
    {
        public int Previous { get; set; }
        public int Current { get; set; }
    }
    
    public class FibonacciHeader : IDynamicComponent<FibonacciHeaderState>
    {
        public FibonacciHeaderState State { get; set; }
        
        public static readonly string[] ColorsTable =
        {
            Colors.Red.Lighten2,
            Colors.Orange.Lighten2,
            Colors.Green.Lighten2,
        };

        public FibonacciHeader(int previous, int current)
        {
            State = new FibonacciHeaderState
            {
                Previous = previous,
                Current = current
            };
        }

        public DynamicComponentComposeResult Compose(DynamicContext context)
        {
            var content = context.CreateElement(container =>
            {
                var colorIndex = State.Current % ColorsTable.Length;
                var color = ColorsTable[colorIndex];

                var ratio = (float)State.Current / State.Previous;
                
                container
                    .Background(color)
                    .Height(50)
                    .AlignMiddle()
                    .AlignCenter()
                    .Text($"{State.Current} / {State.Previous} = {ratio:N5}");
            });

            State = new FibonacciHeaderState
            {
                Previous = State.Current,
                Current = State.Previous + State.Current
            };
            
            return new DynamicComponentComposeResult
            {
                Content = content,
                HasMoreContent = false
            };
        }
    }
    
    public static class DynamicFibonacci
    {
        [Test]
        public static void Dynamic()
        {
            RenderingTest
                .Create()
                .ShowResults()
                .MaxPages(100)
                .ProduceImages()
                .RenderDocument(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A6);
                        page.PageColor(Colors.White);
                        page.Margin(1, Unit.Centimetre);
                        page.DefaultTextStyle(x => x.FontSize(18));

                        page.Header().Dynamic(new FibonacciHeader(17, 19));
                        
                        page.Content().Column(column =>
                        {
                            foreach (var i in Enumerable.Range(0, 50))
                                column.Item().PaddingTop(25).Background(Colors.Grey.Lighten2).Height(50);
                        });
                        
                        page.Footer().AlignCenter().Text(text =>
                        {
                            text.CurrentPageNumber();
                            text.Span(" / ");
                            text.TotalPages();
                        });
                    });
                });
        }
    }
}