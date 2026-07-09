using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class BorderExamples
{
    [Test]
    public void SimpleExample()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.ContinuousSize(450);
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);
                    page.PageColor(Colors.White);
                    
                    page.Content()
                        .Border(3, Colors.Blue.Darken4)
                        .Background(Colors.Blue.Lighten5)
                        .Padding(25) 
                        .Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontColor(Colors.Blue.Darken4).FontSize(16));
                            text.Span("TIP: ").Bold();
                            text.Span("You can use borders to create visual separation between elements in your document. Borders can be applied to any element, including text, images, and containers.");
                        });
                });
            })
            .GenerateImages(x => "border-simple.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void Multiple()
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
                    page.PageColor(Colors.White);
                    
                    page.Content()
                        .Shrink()
                        
                        .BorderVertical(5)
                        .BorderColor(Colors.Green.Darken2)
                        .BorderAlignmentInside()

                        .Container()

                        .BorderHorizontal( 10)
                        .BorderColor(Colors.Blue.Lighten1)
                        .BorderAlignmentInside()
                        
                        .Background(Colors.Grey.Lighten2)
                        .PaddingVertical(25)
                        .PaddingHorizontal(50)
                        .Text("Content");
                });
            })
            .GenerateImages(x => "border-multiple.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void ConsistentThickness()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(550, 0));
                    page.MaxSize(new PageSize(550, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);
                    page.PageColor(Colors.White);

                    page.Content()
                        .Row(row =>
                        {
                            row.Spacing(25);
                            
                            row.RelativeItem()
                                .Border(1, Colors.Black)
                                .Padding(10)
                                .AlignCenter()
                                .Text("Thin");
                            
                            row.RelativeItem()
                                .Border(3, Colors.Black)
                                .Padding(10)
                                .AlignCenter()
                                .Text("Medium");
                            
                            row.RelativeItem()
                                .Border(9, Colors.Black)
                                .Padding(10)
                                .AlignCenter()
                                .Text("Bold");
                        });
                });
            })
            .GenerateImages(x => "border-thickness-consistent.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }

    [Test]
    public void VariousThickness()
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
                    page.PageColor(Colors.White);

                    page.Content()
                        .BorderLeft(4)
                        .BorderTop(6)
                        .BorderRight(8) 
                        .BorderBottom(10)
                        .Padding(25)
                        .Text("Sample text");
                });
            })
            .GenerateImages(x => "border-thickness-various.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void Alignment()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(725, 0));
                    page.MaxSize(new PageSize(725, 1000)); 
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(50);
                    page.PageColor(Colors.White);

                    page.Content()
                        .Row(row =>
                        {
                            row.Spacing(25);
                            
                            row.RelativeItem()
                                .Background(Colors.Grey.Lighten1)
                                .Padding(25)
                                .Text("No Border");
                            
                            row.RelativeItem()
                                .Border(10, Colors.Grey.Darken2)
                                .BorderAlignmentInside()
                                .Padding(25)
                                .Text("Border Inside");
                            
                            row.RelativeItem()
                                .Border(10, Colors.Grey.Darken2)
                                .BorderAlignmentMiddle()
                                .Padding(25)
                                .Text("Border Middle");
                            
                            row.RelativeItem()
                                .Border(10, Colors.Grey.Darken2)
                                .BorderAlignmentOutside()
                                .Padding(25)
                                .Text("Border Outside");
                        });
                });
            })
            .GenerateImages(x => "border-alignment.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void RoundedCorners1()
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
                    page.PageColor(Colors.White);

                    page.Content()
                        .CornerRadius(10)
                        .Border(1, Colors.Black)
                        .Background(Colors.Grey.Lighten2)
                        .Padding(25)
                        .Text("Border with rounded corners"); 
                });
            })
            .GenerateImages(x => "border-rounded-corners-1.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void RoundedCorners2()
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
                    page.PageColor(Colors.White);

                    page.Content()
                        .CornerRadius(10)
                        .BorderLeft(10)
                        .BorderAlignmentInside()
                        .BorderColor(Colors.Green.Darken2)
                        .Background(Colors.Green.Lighten4)
                        .Padding(25)
                        .PaddingLeft(10)
                        .DefaultTextStyle(x => x.FontColor(Colors.Green.Darken4))
                        .Column(column =>
                        {
                            column.Item().Text("Completed").Bold();
                            column.Item().Height(5);
                            column.Item().Text("The invoice has been paid in full.").FontSize(16);
                        });
                });
            })
            .GenerateImages(x => "border-rounded-corners-2.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void SolidColor()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.ContinuousSize(450);
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);
                    page.PageColor(Colors.White);
                    
                    page.Content()
                        .Row(row =>
                        {
                            var colors = new[]
                            {
                                Colors.Red.Medium,
                                Colors.Green.Medium,
                                Colors.Blue.Medium
                            };
                            
                            row.Spacing(25);
                            
                            foreach (var color in colors)
                            {
                                row.RelativeItem()
                                    .Border(5)
                                    .BorderColor(color)
                                    .Padding(15)
                                    .Text(color)
                                    .FontColor(color);
                            }
                        });
                });
            })
            .GenerateImages(x => "border-color-solid.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void Gradient()
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
                    page.PageColor(Colors.White);
                    
                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(25);
                            
                            column.Item()
                                .Border(5)
                                .BorderLinearGradient(0, [Colors.Red.Darken1, Colors.Blue.Darken1])
                                .BorderAlignmentInside()
                                .Padding(25)
                                .Text("Horizontal gradient");
                            
                            column.Item()
                                .Border(10)
                                .BorderLinearGradient(45, [Colors.Green.Darken1, Colors.LightGreen.Darken1, Colors.Yellow.Darken1])
                                .BorderAlignmentInside()
                                .Padding(25)
                                .Text("Diagonal gradient");
                            
                            column.Item()
                                .Border(10)
                                .BorderLinearGradient(90, [Colors.Yellow.Darken1, Colors.Amber.Darken1, Colors.Orange.Darken1])
                                .CornerRadius(20)
                                .Padding(25)
                                .Text("Vertical gradient");
                        });
                });
            })
            .GenerateImages(x => "border-color-gradient.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
}