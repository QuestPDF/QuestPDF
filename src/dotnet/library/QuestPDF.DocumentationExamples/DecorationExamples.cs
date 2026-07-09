using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class DecorationExamples
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
                    page.MaxSize(new PageSize(350, 300));
                    page.Margin(25);
                    page.DefaultTextStyle(x => x.FontSize(20));

                    page.Content()
                        .Background(Colors.Grey.Lighten3)
                        .Padding(15)
                        .Decoration(decoration =>
                        {
                            decoration
                                .Before()
                                .DefaultTextStyle(x => x.Bold())
                                .Column(column =>
                                {
                                    column.Item().ShowOnce().Text("Customer Instructions:");
                                    column.Item().SkipOnce().Text("Customer Instructions [continued]:");
                                });

                            decoration
                                .Content()
                                .PaddingTop(10)
                                .Text("Please wrap the item in elegant gift paper and include a small blank card for a personal message. If possible, remove any price tags or invoices from the package. Make sure the wrapping is secure but easy to open without damaging the contents.");
                        });
                });
            })
            .GenerateImages(x => $"decoration-{x}.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
}