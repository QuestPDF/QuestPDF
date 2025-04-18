using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples.CodePatterns;

public class CodePatternElementPositionLocatorExample
{
    [Test]
    public void Example()
    {
        var content = GenerateReport();
        File.WriteAllBytes("code-pattern-element-position-locator.pdf", content);
    }
    
    public byte[] GenerateReport()
    {
        return Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A6);
                    page.Margin(25);
                    page.DefaultTextStyle(x => x.FontSize(20));
                    
                    page.Content()
                        .Background(Colors.White)
                        .Row(row =>
                        {
                            row.Spacing(10);

                            row.ConstantItem(5).Dynamic(new DynamicTextSpanPositionCapture());
 
                            row.RelativeItem().CapturePosition("container").Text(text =>
                            {
               
                                text.Justify();
                                text.DefaultTextStyle(x => x.FontSize(18));

                                text.Span("Lorem Ipsum is simply dummy text of the printing and typesetting industry.");
                                text.Element(TextInjectedElementAlignment.Top).CapturePosition("span_start");
                                text.Span("Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.").BackgroundColor(Colors.Red.Lighten4);
                                text.Element(TextInjectedElementAlignment.Bottom).CapturePosition("span_end");
                                text.Span(" It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.");
                            });
                        });

                    page.Footer().Text(text =>
                    {
                        text.Span("Page ");
                        text.CurrentPageNumber();
                        text.Span(" of ");
                        text.TotalPages();
                    });
                });
            })
            .GeneratePdf();
    }
    

    public class DynamicTextSpanPositionCapture : IDynamicComponent
    {
        public DynamicComponentComposeResult Compose(DynamicContext context)
        {
            var containerLocation = context.GetElementCapturedPositions("container").FirstOrDefault(x => x.PageNumber == context.PageNumber);

            var spanStartLocation = context.GetElementCapturedPositions("span_start").FirstOrDefault();

            var spanEndLocation = context.GetElementCapturedPositions("span_end").FirstOrDefault();

            if (containerLocation == null || spanStartLocation == null || spanEndLocation == null || containerLocation.PageNumber > spanStartLocation.PageNumber || containerLocation.PageNumber < spanEndLocation.PageNumber)
            {
                return new DynamicComponentComposeResult
                {
                    Content = context.CreateElement(container => { }),
                    HasMoreContent = false
                };
            }
            
            var positionStart = containerLocation.PageNumber > spanStartLocation.PageNumber ? containerLocation.Y : spanStartLocation.Y;
            var positionEnd = containerLocation.PageNumber < spanEndLocation.PageNumber ? (containerLocation.Y + containerLocation.Height) : (spanEndLocation.Y + spanEndLocation.Height);

            var content = context.CreateElement(container =>
            {
                container
                    .TranslateX(0)
                    .TranslateY(positionStart - containerLocation.Y)
                    .Width(5)
                    .Height(positionEnd - positionStart)
                    .Background(Colors.Red.Medium);
            });

            return new DynamicComponentComposeResult
            {
                Content = content,
                HasMoreContent = false
            };
        }
    }
}