using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class PlaceholderExamples
{
    [Test]
    public void TextExample()
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

                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(15);

                            AddItem("Name", Placeholders.Name());
                            AddItem("Email", Placeholders.Email());
                            AddItem("Phone", Placeholders.PhoneNumber());
                            AddItem("Date", Placeholders.ShortDate());
                            AddItem("Time", Placeholders.Time());
                            
                            void AddItem(string label, string value)
                            {
                                column.Item().Text(text =>
                                {
                                    text.Span($"{label}: ").Bold();
                                    text.Span(value);
                                });
                            }
                        });
                });
            })
            .GenerateImages(x => "placeholders-text.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void BackgroundColorExample()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(320, 0));
                    page.MaxSize(new PageSize(320, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content()
                        .Grid(grid =>
                        {
                            grid.Columns(5);
                            grid.Spacing(5);
    
                            foreach (var _ in Enumerable.Range(0, 25))
                            {
                                grid.Item()
                                    .Height(50)
                                    .Width(50)
                                    .Background(Placeholders.BackgroundColor());
                            }
                        });
                });
            })
            .GenerateImages(x => "placeholders-color-background.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void ColorExample()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(500, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(10);
                            
                            foreach (var i in Enumerable.Range(0, 5))
                            {
                                column.Item()
                                    .Text(Placeholders.Sentence())
                                    .FontColor(Placeholders.Color());
                            }
                        });
                });
            })
            .GenerateImages(x => "placeholders-color.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
    
    [Test]
    public void ImageExample()
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

                    page.Content()
                        .Width(200)
                        .Column(column =>
                        {
                            column.Spacing(10);

                            // provide an exact image resolution
                            column.Item()
                                .Image(Placeholders.Image(100, 50));
                            
                            // specify physical width and height of the image
                            column.Item()
                                .Width(200)
                                .Height(150)
                                .Image(Placeholders.Image);
                            
                            // specify target physical width and aspect ratio
                            column.Item()
                                .Width(200)
                                .AspectRatio(3 / 2f)
                                .Image(Placeholders.Image);
                        });
                });
            })
            .GenerateImages(x => "placeholders-image.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
    
    [Test]
    public void ElementExample()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A5);
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Header()
                        .Height(100)
                        .Placeholder("Header");
                    
                    page.Content()
                        .PaddingVertical(25)
                        .Placeholder();
                    
                    page.Footer()
                        .Height(100)
                        .Placeholder("Footer");
                });
            })
            .GenerateImages(x => "placeholder-element.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.High, RasterDpi = 144 });
    }
}