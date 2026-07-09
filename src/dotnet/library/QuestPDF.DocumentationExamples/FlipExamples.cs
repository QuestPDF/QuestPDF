using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class FlipExamples
{
    [Test]
    public void Example()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(350, 0));
                    page.MaxSize(new PageSize(350, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(15);

                            column.Item()
                                .Text("Read the message below by putting a mirror on the right side of the screen.");
                            
                            column.Item()
                                .AlignLeft()
                                .Background(Colors.Red.Lighten5)
                                .Padding(10)
                                .FlipHorizontal()
                                .Text("This is a secret message.")
                                .FontColor(Colors.Red.Darken2);
                        });
                });
            })
            .GenerateImages(x => "flip.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
}