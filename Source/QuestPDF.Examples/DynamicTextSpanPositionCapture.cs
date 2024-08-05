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
    public class DynamicTextSpanPositionCapture : IDynamicComponent
    {
        public DynamicComponentComposeResult Compose(DynamicContext context)
        {
            var containerLocation = context.GetElementCapturedLocations("container").FirstOrDefault();
            
            var spanStartLocation = context.GetElementCapturedLocations("span_start").FirstOrDefault();

            var spanEndLocation = context.GetElementCapturedLocations("span_end").FirstOrDefault();
            
            if (containerLocation == null || spanStartLocation == null || spanEndLocation == null)
            {
                return new DynamicComponentComposeResult
                {
                    Content = context.CreateElement(container => { }),
                    HasMoreContent = false
                };
            }

            var content = context.CreateElement(container =>
            {
                container
                    .TranslateX(0)
                    .TranslateY(spanStartLocation.Y - containerLocation.Y)
                    .Width(5)
                    .Height(spanEndLocation.Y - spanStartLocation.Y)
                    .Background(Colors.Red.Medium);
            });
            
            return new DynamicComponentComposeResult
            {
                Content = content,
                HasMoreContent = false
            };
        }
    }
    
    public static class DynamicTextSpanPositionCaptureExample
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
                            row.Spacing(10);
                            
                            row.ConstantItem(5).Dynamic(new DynamicTextSpanPositionCapture());

                            row.RelativeItem().CaptureLocation("container").Text(text =>
                            {
                                text.Justify();
                                text.DefaultTextStyle(x => x.FontSize(18));
                                
                                text.Span("Lorem Ipsum is simply dummy text of the printing and typesetting industry. ");
                                text.Element(TextInjectedElementAlignment.Top).CaptureLocation("span_start");
                                text.Span("Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.").BackgroundColor(Colors.Red.Lighten4);
                                text.Element(TextInjectedElementAlignment.Bottom).CaptureLocation("span_end");
                                text.Span(" It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.");
                            });
                        });
                });
        }
    }
}