using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class ScaleToFitExamples
{
    [Test]
    public void Example()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(1000, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content()
                        .Column(column =>
                        {
                            const string text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.";

                            foreach (var i in Enumerable.Range(4, 5))
                            {
                                column
                                    .Item()
                                    .Shrink()
                                    .Border(1)
                                    .Padding(15)
                                    .Width(i * 50) // sizes from 200x100 to 450x175
                                    .Height(i * 25)
                                    .ScaleToFit()
                                    .Text(text);
                            }
                        });
                });
            })
            .GenerateImages(x => "scale-to-fit.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.High, RasterDpi = 144 });
    }
}