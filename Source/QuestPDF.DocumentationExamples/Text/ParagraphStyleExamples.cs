using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples.Text;

public class ParagraphStyleExamples
{
    [Test]
    public void TextAlignment()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(400, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(20);
                            
                            column.Item()
                                .Element(CellStyle)
                                .Text("This is an example of left-aligned text, showcasing how the text starts from the left margin and continues naturally across the container.")
                                .AlignLeft();

                            column.Item()
                                .Element(CellStyle)
                                .Text("This text is centered within its container, creating a balanced look, especially for titles or headers.")
                                .AlignCenter();

                            column.Item()
                                .Element(CellStyle)
                                .Text("This example demonstrates right-aligned text, often used for dates, numbers, or aligning text to the right margin.")
                                .AlignRight();

                            column.Item()
                                .Element(CellStyle)
                                .Text("Justified text adjusts the spacing between words so that both the left and right edges of the text block are aligned, creating a clean, newspaper-like look.")
                                .Justify();
    
                            static IContainer CellStyle(IContainer container) 
                                => container.Background(Colors.Grey.Lighten3).Padding(10);
                        });

                });
            })
            .GenerateImages(x => "text-paragraph-alignment.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void FirstLineIndentation()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(500, 1200));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content()
                        .Text(Placeholders.Paragraphs())
                        .ParagraphFirstLineIndentation(40);
                });
            })
            .GenerateImages(x => "text-paragraph-first-line-indentation.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.High, RasterDpi = 144 });
    }
    
    [Test]
    public void Spacing()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(500, 1200));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content()
                        .Text(Placeholders.Paragraphs())
                        .ParagraphSpacing(10);
                });
            })
            .GenerateImages(x => "text-paragraph-spacing.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.High, RasterDpi = 144 });
    }
    
    [Test]
    public void ClampLines()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(600, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);
                    
                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(10);

                            var paragraph = Placeholders.Paragraph();

                            column.Item()
                                .Background(Colors.Grey.Lighten3)
                                .Padding(5)
                                .Text(paragraph);
                            
                            column.Item()
                                .Background(Colors.Grey.Lighten3)
                                .Padding(5)
                                .Text(paragraph)
                                .ClampLines(3);
                        });
                });
            })
            .GenerateImages(x => "text-paragraph-clamp-lines.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
    
    [Test]
    public void ClampLinesWithCustomEllipsis()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(600, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content()
                        .Text(Placeholders.Paragraph())
                        .ClampLines(3, " [...]");
                });
            })
            .GenerateImages(x => "text-paragraph-clamp-lines-custom-ellipsis.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
}