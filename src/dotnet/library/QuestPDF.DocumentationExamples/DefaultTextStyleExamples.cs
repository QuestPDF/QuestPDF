using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class DefaultTextStyleExamples
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
                        .Padding(25)
                        .DefaultTextStyle(x => x.Bold().Underline())
                        .Column(column =>
                        { 
                            column.Spacing(10);
                            
                            column.Item().Text("Inherited bold and underline");
                            column.Item().Text("Disabled underline, inherited bold and adjusted font color").Underline(false).FontColor(Colors.Green.Darken2);
    
                            column.Item()
                                .DefaultTextStyle(x => x.DecorationWavy().FontColor(Colors.LightBlue.Darken3))
                                .Text("Changed underline type and adjusted font color");
                        }); 
                });
            })
            .GenerateImages(x => "default-text-style.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
}