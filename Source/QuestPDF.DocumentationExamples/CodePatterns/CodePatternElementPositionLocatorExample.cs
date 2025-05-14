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
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.ContinuousSize(575);
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content()
                        .Background(Colors.White)
                        .Row(row =>
                        {
                            row.Spacing(25);

                            row.ConstantItem(0).Dynamic(new DynamicTextSpanPositionCapture());

                            row.RelativeItem().CapturePosition("container").Text(text =>
                            {
                                text.Justify();
                                
                                var mistakeTextStyle = TextStyle.Default
                                    .FontColor(Colors.Red.Darken3)
                                    .BackgroundColor(Colors.Red.Lighten4)
                                    .Strikethrough()
                                    .DecorationThickness(2);
                                
                                var correctionTextStyle = TextStyle.Default
                                    .FontColor(Colors.Green.Darken3)
                                    .BackgroundColor(Colors.Green.Lighten4);

                                text.Span("Proofreading").Bold().Underline().DecorationThickness(2);
                                text.Span(" technical documentation is a critical quality assurance step that ensures clarity, accuracy, and consistency across all written content. It involves more than just checking for grammar and ");
                                text.Span("spilling").Style(mistakeTextStyle);
                                text.Span("spelling").Style(correctionTextStyle);
                                text.Element(TextInjectedElementAlignment.Middle).CapturePosition("mistake");
                                text.Span(" errors—it also includes verifying terminology, code syntax, formatting standards, and logical flow. A common best practice is to have the content reviewed by both a subject matter ");
                                text.Span("export").Style(mistakeTextStyle);
                                text.Span("expert").Style(correctionTextStyle);
                                text.Element(TextInjectedElementAlignment.Middle).CapturePosition("mistake");
                                text.Span(" and a language specialist, ensuring that the material is technically sound while also being accessible to the intended audience.");
                            });
                        });
                });
            })
            .GenerateImages(x => "code-pattern-element-position-locator.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }

    public class DynamicTextSpanPositionCapture : IDynamicComponent
    {
        public DynamicComponentComposeResult Compose(DynamicContext context)
        {
            var containerLocation = context.GetElementCapturedPositions("container").FirstOrDefault(x => x.PageNumber == context.PageNumber);
            var mistakeLocations = context.GetElementCapturedPositions("mistake").Where(x => x.PageNumber == context.PageNumber).ToList();
            
            if (containerLocation == null || mistakeLocations.Count == 0)
            {
                return new DynamicComponentComposeResult
                {
                    Content = context.CreateElement(_ => { }),
                    HasMoreContent = false
                };
            }

            var content = context.CreateElement(container =>
            {
                container.Layers(layers =>
                {
                    layers.PrimaryLayer();

                    foreach (var mistakeLocation in mistakeLocations)
                    {
                        layers
                            .Layer()
                            .Unconstrained() 
                            .TranslateY(mistakeLocation.Y - containerLocation.Y)
                            .TranslateX(-12)
                            .TranslateY(-12)
                            .Width(24)
                            .Svg("Resources/proofreading.svg");
                    }
                });
            });

            return new DynamicComponentComposeResult
            {
                Content = content,
                HasMoreContent = false
            };
        }
    }
}