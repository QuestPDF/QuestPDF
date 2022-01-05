using System.Drawing;
using System.Linq;
using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples
{
    public class DefaultTextStyleExamples
    {
        [Test]
        public void Placeholder()
        {
            RenderingTest
                .Create()
                .PageSize(220, 270)
                .ProduceImages()
                .ShowResults()
                .EnableDebugging()
                .Render(container =>
                {
                    container
                        .Padding(10)
                        .DefaultTextStyle(TextStyle.Default.Bold().Underline())
                        .Stack(stack =>
                        { 
                            stack.Item().Text("Default style applies to all children", TextStyle.Default);
                            stack.Item().Text("You can override certain styles", TextStyle.Default.Underline(false).Color(Colors.Green.Darken2));
                            
                            stack.Item().PaddingTop(10).Border(1).Grid(grid =>
                            {
                                grid.Columns(4);

                                foreach (var i in Enumerable.Range(1, 16))
                                {
                                    grid.Item()
                                        .Border(1)
                                        .BorderColor(Colors.Grey.Lighten1)
                                        .Background(Colors.Grey.Lighten3)
                                        .Width(50)
                                        .Height(50)
                                        .AlignCenter()
                                        .AlignMiddle()
                                        .Text(i, TextStyle.Default.Size(16 + i / 4));   
                                }
                            });
                        });
                });
        }
    }
}