using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class ColumnExamples
{
    [Test]
    public void SimpleExample()
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
                        .Width(250)
                        .Padding(25)
                        .Column(column =>
                        {
                            column.Item().Background(Colors.Grey.Medium).Height(50);
                            column.Item().Background(Colors.Grey.Lighten1).Height(75);
                            column.Item().Background(Colors.Grey.Lighten2).Height(100);
                        });
                });
            })
            .GenerateImages(x => "column-simple.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void ManyExample()
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
                        .Width(250)
                        .Padding(25)
                        .Column(column =>
                        {
                            column.Spacing(25);
                            
                            column.Item().Background(Colors.Grey.Medium).Height(50);
                            column.Item().Background(Colors.Grey.Lighten1).Height(75);
                            column.Item().Background(Colors.Grey.Lighten2).Height(100);
                        });
                });
            })
            .GenerateImages(x => "column-spacing.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
}