using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using QuestPDF.Elements;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples
{
    public class DynamicPositionCaptureExample : IDynamicComponent
    {
        public DynamicComponentComposeResult Compose(DynamicContext context)
        {
            var positions = Enumerable
                .Range(0, 20)
                .SelectMany(x => context.GetElementCapturedLocations($"capture_{x}"))
                .ToList();
            
            var visibleCount = positions.Count(x => x.PageNumber == context.PageNumber);

            return new DynamicComponentComposeResult
            {
                Content = context.CreateElement(container => container.Text(visibleCount.ToString())),
                HasMoreContent = positions.Any(x => x.PageNumber > context.PageNumber + 1)
            };
        }
    }
    
    public static class DynamicPositionCapture
    {
        [Test]
        public static void Dynamic()
        {
            RenderingTest
                .Create()
                .PageSize(PageSizes.A4)
                .ShowResults()
                .ProducePdf()
                .Render(container =>
                {
                    container
                        .Background(Colors.White)
                        .Padding(25)
                        .Row(row =>
                        {
                            row.Spacing(25);
                            
                            row.RelativeItem().Border(1).Column(column =>
                            {
                                column.Spacing(25);
                                
                                foreach (var i in Enumerable.Range(0, 20))
                                {
                                    column.Item()
                                        .CaptureLocation($"capture_{i}")
                                        .Width(Random.Shared.Next(25, 125))
                                        .Height(Random.Shared.Next(25, 125))
                                        .Background(Placeholders.BackgroundColor());
                                }
                            });
                            
                            row.RelativeItem().Dynamic(new DynamicPositionCaptureExample());
                        });
                });
        }
    }
}