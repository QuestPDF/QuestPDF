using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class SkipLastExamples
{
    [Test]
    public void Example()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(350, 500);
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content()
                        .Decoration(decoration =>
                        {
                            decoration.Before()
                                .DefaultTextStyle(x => x.Bold())
                                .Column(column =>
                                {
                                    column.Item().ShowOnce().Text("Terms and Conditions");
                                    column.Item().SkipOnce().Text("Terms and Conditions [continued]");
                                });

                            decoration.Content()
                                .PaddingTop(10)
                                .ExtendHorizontal()
                                .Column(column =>
                                {
                                    column.Spacing(10);

                                    foreach (var i in Enumerable.Range(1, 12))
                                    {
                                        column.Item()
                                            .Height(30)
                                            .Background(Colors.Grey.Lighten3)
                                            .AlignCenter()
                                            .AlignMiddle()
                                            .Text($"Clause {i}");
                                    }
                                });

                            decoration.After()
                                .PaddingTop(5)
                                .Column(column =>
                                {
                                    column.Item()
                                        .SkipLast()
                                        .Text("Continued on next page...")
                                        .Italic()
                                        .FontSize(14)
                                        .FontColor(Colors.Grey.Darken1);
                                });
                        });
                });
            })
            .GeneratePdfAndShow();
            // .GenerateImages(x => $"skip-last-{x}.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
}
