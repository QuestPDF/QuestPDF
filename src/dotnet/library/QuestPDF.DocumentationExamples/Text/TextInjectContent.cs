using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples.Text;

public class TextInjectContent
{
    [Test]
    public void InjectImage()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(500, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content()
                        .Text(text =>
                        {
                            text.Span("A unit test can either ");
                            text.Element().PaddingBottom(-4).Height(24).Image("Resources/unit-test-completed-icon.png");
                            text.Span(" pass").FontColor(Colors.Green.Medium);
                            text.Span(" or ");
                            text.Element().PaddingBottom(-4).Height(24).Image("Resources/unit-test-failed-icon.png");
                            text.Span(" fail").FontColor(Colors.Red.Medium);
                            text.Span(".");
                        });
                });
            })
            .GenerateImages(x => "text-inject-image.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void InjectSvg()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(350, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content()
                        .Text(text =>
                        {
                            text.Span("To synchronize your email inbox, please click the ");
                            text.Element().PaddingBottom(-4).Height(24).Svg("Resources/mail-synchronize-icon.svg");
                            text.Span(" icon.");
                        });
                });
            })
            .GenerateImages(x => "text-inject-svg.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void InjectPosition()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(400, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content()
                        .Text(text =>
                        {
                            text.Span("This ");
    
                            text.Element(TextInjectedElementAlignment.AboveBaseline)
                                .Width(12).Height(12)
                                .Background(Colors.Green.Medium);
    
                            text.Span(" element is positioned above the baseline, while this ");
    
                            text.Element(TextInjectedElementAlignment.BelowBaseline)
                                .Width(12).Height(12)
                                .Background(Colors.Blue.Medium);
    
                            text.Span(" element is positioned below the baseline.");
                        });
                });
            })
            .GenerateImages(x => "text-inject-position.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
}