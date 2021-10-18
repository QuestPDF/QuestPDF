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
    public class TableWithSubtotals : IDynamic
    {
        private ICollection<int> Values { get; }
        private Queue<int> ValuesQueue { get; set; }

        public TableWithSubtotals(ICollection<int> values)
        {
            Values = values;
        }
        
        public void Reset()
        {
            ValuesQueue = new Queue<int>(Values);
        }

        public bool Compose(DynamicContext context, IContainer container)
        {
            var internalQueue = new Queue<int>(ValuesQueue);
            
            container.Box().Border(2).Background(Colors.Grey.Lighten3).Stack(stack =>
            {
                var summaryHeight = 40f;
                
                var totalHeight = summaryHeight;
                var total = 0;
                
                while (internalQueue.Any())
                {
                    var value = internalQueue.Peek();

                    var structure = context.Content(content =>
                    {
                        content
                            .Padding(10)
                            .Text(value);
                    });

                    var structureHeight = structure.Measure().Height;

                    if (totalHeight + structureHeight > context.AvailableSize.Height)
                        break;

                    totalHeight += structureHeight;
                    total += value;

                    stack.Item().Border(1).Element(structure);
                    internalQueue.Dequeue();
                }
                
                stack
                    .Item()
                    .ShowEntire()
                    .Border(2)
                    .Background(Colors.Grey.Lighten1)
                    .Padding(10)
                    .Text($"Total: {total}", TextStyle.Default.SemiBold());
            });

            if (context.IsDrawStep)
                ValuesQueue = internalQueue;
            
            return internalQueue.Any();
        }
    }
    
    public static class DynamicExamples
    {
        [Test]
        public static void Dynamic()
        {
            RenderingTest
                .Create()
                .PageSize(300, 500)
                .FileName()
                .ShowResults()
                .Render(container =>
                {
                    var values = Enumerable.Range(0, 15).ToList();
                    
                    container
                        .Background(Colors.White)
                        .Padding(25)
                        .Decoration(decoration =>
                        {
                            decoration
                                .Header()
                                .PaddingBottom(5)
                                .Text(text =>
                                {
                                    text.DefaultTextStyle(TextStyle.Default.SemiBold().Color(Colors.Blue.Darken2).Size(16));
                                    text.Span("Page ");
                                    text.CurrentPageNumber();
                                    text.Span(" of ");
                                    text.TotalPages();
                                });
                            
                            decoration
                                .Content()
                                .Dynamic(new TableWithSubtotals(values));
                        });
                });
        }
    }
}