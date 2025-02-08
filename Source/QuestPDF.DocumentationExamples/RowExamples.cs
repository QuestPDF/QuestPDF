using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class RowExamples
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
                        .Padding(25)
                        .Width(325)
                        .Row(row =>
                        {
                            row.ConstantItem(100)
                                .Background(Colors.Grey.Medium)
                                .Padding(10)
                                .Text("100pt");
    
                            row.RelativeItem()
                                .Background(Colors.Grey.Lighten1)
                                .Padding(10)
                                .Text("75pt");
    
                            row.RelativeItem(2)
                                .Background(Colors.Grey.Lighten2)
                                .Padding(10)
                                .Text("150pt");
                        });
                });
            })
            .GenerateImages(x => "row-simple.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void AutoItem()
    {
        // TODO: improve and update documentation
        
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(500, 300));
                    page.DefaultTextStyle(x => x.FontSize(20));

                    page.Content()
                        .Padding(25)
                        .Row(row =>
                        {
                            row.AutoItem().Text("Auto column");
                            row.RelativeItem().Text(Placeholders.LoremIpsum());
                        });
                });
            })
            .GeneratePdf();
    }
    
    [Test]
    public void SpacingExample()
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
                        .Padding(25)
                        .Width(220)
                        .Height(50)
                        .Row(row =>
                        {
                            row.Spacing(10);
        
                            row.RelativeItem(2).Background(Colors.Grey.Medium);
                            row.RelativeItem(3).Background(Colors.Grey.Lighten1);
                            row.RelativeItem(5).Background(Colors.Grey.Lighten2);
                        });
                });
            })
            .GenerateImages(x => "row-spacing.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
}