using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class UnconstrainedExamples
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

                    page.Content()
                        .Width(400)
                        .Height(350)
                        .Padding(25)
                        .PaddingLeft(50) 
                        .Column(column =>
                        {
                            column.Item().Width(300).Height(150).Background(Colors.Blue.Lighten3);

                            column
                                .Item()
                                .Unconstrained()
                                .TranslateX(-50)
                                .TranslateY(-50)
                                .Width(100)
                                .Height(100)
                                .Background(Colors.Blue.Darken2);

                            column.Item().Width(300).Height(150).Background(Colors.Blue.Lighten2);
                        });
                });
            })
            .GenerateImages(x => "unconstrained.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
}