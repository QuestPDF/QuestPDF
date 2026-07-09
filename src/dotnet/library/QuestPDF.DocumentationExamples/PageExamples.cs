using QuestPDF.Companion;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class PageExamples
{
    [Test]
    public void Simple()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A5);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(24));

                    page.Header()
                        .Text("Hello, World!")
                        .FontSize(48).Bold();

                    page.Content()
                        .PaddingVertical(25)
                        .Text(Placeholders.LoremIpsum())
                        .Justify();
        
                    page.Footer()
                        .AlignCenter() 
                        .Text(text =>
                        {
                            text.CurrentPageNumber();
                            text.Span(" / ");
                            text.TotalPages();
                        });
                });
            })
            .GenerateImages(x => "page-simple.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void MainSlots()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(24));

                    page.Header()
                        .Background(Colors.Grey.Lighten1)
                        .Height(125)
                        .AlignCenter()
                        .AlignMiddle()
                        .Text("Header");
                    
                    page.Content()
                        .Background(Colors.Grey.Lighten2)
                        .AlignCenter()
                        .AlignMiddle()
                        .Text("Content");
                    
                    page.Footer()
                        .Background(Colors.Grey.Lighten1)
                        .Height(75)
                        .AlignCenter()
                        .AlignMiddle()
                        .Text("Footer");
                });
            })
            .GenerateImages(x => "page-main-slots.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void Foreground()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(20));

                    page.Header()
                        .PaddingBottom(1, Unit.Centimetre)
                        .Text("Report")
                        .FontSize(30)
                        .Bold();
                    
                    page.Content()
                        .Text(Placeholders.Paragraphs())
                        .ParagraphSpacing(1, Unit.Centimetre)
                        .Justify();
 
                    page.Foreground().Svg("Resources/draft-foreground.svg").FitArea();
                });
            })
            .GenerateImages(x => "page-foreground.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.High, RasterDpi = 144 });
    }
    
    [Test]
    public void Background()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                { 
                    page.Size(PageSizes.A4.Landscape());

                    page.Background().Svg("Resources/certificate-background.svg").FitArea();

                    page.Content() 
                        .PaddingLeft(10, Unit.Centimetre)
                        .PaddingRight(5 , Unit.Centimetre)
                        .AlignMiddle()
                        .Column(column =>
                        {
                            column.Item().Height(50).Svg("Resources/questpdf-logo.svg");
                            
                            column.Item().Height(50);
                            
                            column.Item().Text("CERTIFICATE").FontSize(64).ExtraBlack();
                            
                            column.Item().Height(25);
                            
                            column.Item()
                                .Shrink().BorderBottom(1).Padding(10)
                                .Text("Marcin Ziąbek").FontSize(32).Italic();
                            
                            column.Item().Height(10); 
                            
                            column.Item()
                                .Text($"has successfully completed the course \"QuestPDF Basics\" on {DateTime.Now:dd MMM yyyy}.")
                                .FontSize(20).Light();
                        });
                });
            })
            .GenerateImages(x => $"page-background.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
}