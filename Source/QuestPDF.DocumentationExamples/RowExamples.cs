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
                    page.Margin(25);

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
    public void SpacingExample()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(1000, 1000));
                    page.Margin(25);

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
    
    [Test]
    public void CustomSpacingExample()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(250, 0));
                    page.MaxSize(new PageSize(250, 1000));
                    page.Margin(25);

                    page.Content()
                        .Height(50)
                        .Row(row =>
                        {
                            row.RelativeItem().Background(Colors.Grey.Darken1);
                            row.ConstantItem(10);
                            row.RelativeItem().Background(Colors.Grey.Medium);
                            row.ConstantItem(20);
                            row.RelativeItem().Background(Colors.Grey.Lighten1);
                            row.ConstantItem(30);
                            row.RelativeItem().Background(Colors.Grey.Lighten2);
                        });
                });
            })
            .GenerateImages(x => "row-spacing-custom.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void DisableUniformItemsHeightExample()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(700, 0));
                    page.MaxSize(new PageSize(700, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);
                    page.PageColor(Colors.White);

                    page.Content()
                        .Row(row =>
                        {
                            row.Spacing(15);
                            
                            row.RelativeItem()
                                .Element(LabelStyle)
                                .Text("Programming is both a science and an art — it demands precision, creativity, and patience. At its core, it’s about understanding how to break down complex problems into small, logical steps that a computer can execute.");
                            
                            row.RelativeItem()
                                .Element(LabelStyle)
                                .Text("Programming is the art of turning ideas into logic, logic into code, and code into something that solves real problems.");
                            
                            static IContainer LabelStyle(IContainer container) => container
                                .ShrinkVertical()
                                .Background(Colors.Grey.Lighten3)
                                .CornerRadius(15)
                                .Padding(15);
                        });
                });
            })
            .GenerateImages(x => "row-uniform-height-enabled.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.High, RasterDpi = 144 });
    }
}