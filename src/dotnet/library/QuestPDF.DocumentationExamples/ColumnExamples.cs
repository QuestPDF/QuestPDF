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
                    page.MinSize(new PageSize(250, 0));
                    page.MaxSize(new PageSize(250, 1000));
                    page.Margin(25);
                    
                    page.Content()
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
    public void SpacingExample()
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
                        .Column(column =>
                        {
                            column.Item().Background(Colors.Grey.Darken1).Height(50);
                            column.Item().Height(10);
                            column.Item().Background(Colors.Grey.Medium).Height(50);
                            column.Item().Height(20);
                            column.Item().Background(Colors.Grey.Lighten1).Height(50);
                            column.Item().Height(30);
                            column.Item().Background(Colors.Grey.Lighten2).Height(50);
                        });
                });
            })
            .GenerateImages(x => "column-spacing-custom.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
        
    [Test]
    public void DisableUniformItemsWidthExample()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(400, 0));
                    page.MaxSize(new PageSize(400, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);
                    page.PageColor(Colors.White);

                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(15);
                            
                            column.Item()
                                .Element(LabelStyle)
                                .Text("REST API");
                            
                            column.Item()
                                .Element(LabelStyle)
                                .Text("Garbage Collection");
                            
                            column.Item()
                                .Element(LabelStyle)
                                .Text("Object-Oriented Programming");
                            
                            static IContainer LabelStyle(IContainer container) => container
                                .ShrinkHorizontal()
                                .Background(Colors.Grey.Lighten3)
                                .CornerRadius(15)
                                .Padding(15);
                        });
                });
            })
            .GenerateImages(x => "column-uniform-width-disabled.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
}