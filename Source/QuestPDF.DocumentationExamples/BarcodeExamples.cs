using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ZXing;
using ZXing.OneD;
using ZXing.QrCode;
using ZXing.Rendering;

namespace QuestPDF.DocumentationExamples;

public class BarcodeExamples
{
    [Test]
    public void BarcodeExample()
    {
        Settings.UseEnvironmentFonts = false;

        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(550, 0));
                    page.MaxSize(new PageSize(550, 1000));
                    page.DefaultTextStyle(x => x.FontSize(16));
                    
                    page.Content()
                        .Background(Colors.Grey.Lighten3)
                        .Padding(25)
                        .Row(row =>
                        {
                            var productId = Random.Shared.NextInt64() % 10_000_000;
                            
                            row.Spacing(20);

                            row.RelativeItem().Text(text =>
                            {
                                text.ParagraphSpacing(10);
                                
                                text.Span("Product ID: ").Bold();
                                text.Line(productId.ToString("D7"));
                                
                                text.Span("Name: ").Bold();
                                text.Line(Placeholders.Label());
           
                                text.Span("Description: ").Bold();
                                text.Span(Placeholders.Sentence());
                            });

                            row.AutoItem()
                                .Background(Colors.White)
                                .AlignCenter()
                                .AlignMiddle()
                                .Width(200)
                                .Height(75)
                                .Svg(size =>
                                {
                                    var content = productId.ToString("D7");
                                    
                                    var writer = new EAN8Writer();
                                    var eanCode = writer.encode(content, BarcodeFormat.EAN_8, (int)size.Width, (int)size.Height);
                                    var renderer = new SvgRenderer { FontName = "Lato", FontSize = 16 };
                                    return renderer.Render(eanCode, BarcodeFormat.EAN_8, content).Content;
                                });
                        });
                });
            })
            .GenerateImages(x => "barcode.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
    
    [Test]
    public void QRCodeExample()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(550, 0));
                    page.MaxSize(new PageSize(550, 1000)); 
                    page.DefaultTextStyle(x => x.FontSize(20));
                    
                    page.Content()
                        .Background(Colors.Grey.Lighten3)
                        .Padding(25)
                        .Row(row =>
                        {
                            const string url = "https://en.wikipedia.org/wiki/Algorithm";
                            
                            row.Spacing(20);

                            row.RelativeItem()
                                .AlignMiddle()
                                .Text(text =>
                                {
                                    text.Justify();
                                    text.Span("In mathematics and computer science, ");
                                    text.Span("an algorithm").Bold().BackgroundColor(Colors.White);
                                    text.Span(" is a finite sequence of mathematically rigorous instructions, typically used to solve a class of specific problems or to perform a computation. ");
                                    text.Hyperlink("Learn more", url).Underline().FontColor(Colors.Blue.Darken2);
                                });
                            
                            row.ConstantItem(5, Unit.Centimetre)
                                .AspectRatio(1)
                                .Background(Colors.White)
                                .Svg(size =>
                                {
                                    var writer = new QRCodeWriter();
                                    var qrCode = writer.encode(url, BarcodeFormat.QR_CODE, (int)size.Width, (int)size.Height);
                                    var renderer = new SvgRenderer { FontName = "Lato" };
                                    return renderer.Render(qrCode, BarcodeFormat.EAN_13, null).Content;
                                });
                        });
                });
            })
            .GenerateImages(x => "qrcode.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
}