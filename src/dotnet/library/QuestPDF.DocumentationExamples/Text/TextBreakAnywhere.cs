using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples.Text;

public class TextBreakAnywhere
{
    [Test]
    public void BreakAnywhere()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(360, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(20);

                            column.Item()
                                .Background(Colors.Grey.Lighten3)
                                .Text("https://www.questpdf.com/api-reference/text/font-management.html#manual-font-registration");

                            column.Item()
                                .Background(Colors.Grey.Lighten3)
                                .Text("https://www.questpdf.com/api-reference/text/font-management.html#manual-font-registration")
                                .BreakAnywhere();
                        });
                });
            })
            .GenerateImages(x => "text-break-anywhere-long-link.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
}
