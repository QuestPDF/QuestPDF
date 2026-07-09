using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class ShowOnceExamples
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
                            decoration.Before().Column(column =>
                            {
                                column.Item()
                                    .ShowOnce()
                                    .Row(row =>
                                    {
                                        row.ConstantItem(80).AspectRatio(4 / 3f).Placeholder();
                                        row.ConstantItem(10);
                                        row.RelativeItem()
                                            .AlignMiddle()
                                            .Column(innerColumn =>
                                            {
                                                innerColumn.Item().Text("Invoice #1234").FontSize(24).Bold();
                                                innerColumn.Item().Text($"Generated on {DateTime.Now:d}").FontSize(16).Light();
                                            });
                                    });
                                
                                column.Item()
                                    .SkipOnce()
                                    .Text("Invoice #1234").FontSize(24).Bold();
                            });
                            
                            // generate dummy content
                            decoration.Content()
                                .PaddingTop(15)
                                .ExtendHorizontal()
                                .Column(column =>
                                {
                                    column.Spacing(10);
                                    
                                    foreach (var i in Enumerable.Range(1, 15))
                                    {
                                        column.Item()
                                            .Height(30)
                                            .Background(Colors.Grey.Lighten3)
                                            .AlignCenter()
                                            .AlignMiddle()
                                            .Text($"{i}");
                                    }
                                });
                        });
                });
            })
            .GenerateImages(x => $"show-once-{x}.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
}