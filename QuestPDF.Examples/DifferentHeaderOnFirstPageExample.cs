using System.Linq;
using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples
{
    public class DifferentHeaderOnFirstPageExample
    {
        [Test]
        public void Placeholder()
        {
            RenderingTest
                .Create()
                .PageSize(PageSizes.A6)
                .ProduceImages()
                .ShowResults()
                .EnableDebugging()
                .RenderDocument(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A6);
                        page.Margin(30);
                        page.Background(Colors.White);
                        
                        page.Header().Stack(stack =>
                        {
                            stack.Item().ShowOnce().Background(Colors.Blue.Lighten2).Height(60);
                            stack.Item().SkipOnce().Background(Colors.Green.Lighten2).Height(40);
                        });
                        
                        page.Content().PaddingVertical(10).Stack(stack =>
                        {
                            stack.Spacing(10);

                            foreach (var _ in Enumerable.Range(0, 13))
                                stack.Item().Background(Colors.Grey.Lighten2).Height(40);
                        });
                        
                        page.Footer().AlignCenter().Text(text =>
                        {
                            text.DefaultTextStyle(TextStyle.Default.Size(16));
                            
                            text.CurrentPageNumber();
                            text.Span(" / ");
                            text.TotalPages();
                        });
                    });
                });
        }
    }
}