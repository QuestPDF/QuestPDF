using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples.Text;

public class TextHyphenation
{
    [Test]
    public void SoftHyphen()
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
                                .Text("Bitte aktualisieren Sie die Zugriffsberechtigungen in der Serverkonfigurationsdatei.");

                            column.Item()
                                .Background(Colors.Grey.Lighten3)
                                .Text("Bit\u00ADte ak\u00ADtua\u00ADli\u00ADsie\u00ADren Sie die " +
                                      "Zu\u00ADgriffs\u00ADbe\u00ADrech\u00ADti\u00ADgun\u00ADgen in der " +
                                      "Ser\u00ADver\u00ADkon\u00ADfi\u00ADgu\u00ADra\u00ADti\u00ADons\u00ADda\u00ADtei.");
                        });
                });
            })
            .GenerateImages(x => "text-hyphenation.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
}