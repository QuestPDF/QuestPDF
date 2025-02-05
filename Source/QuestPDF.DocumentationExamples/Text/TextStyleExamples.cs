using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples.Text;

public class TextStyleExamples
{
    [Test]
    public void FontSize()
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
                            column.Spacing(10);

                            column.Item()
                                .Text("This is small text (16pt)")
                                .FontSize(16);

                            column.Item()
                                .Text("This is medium text (24pt)")
                                .FontSize(24);

                            column.Item()
                                .Text("This is large text (36pt)")
                                .FontSize(36);
                        });
                });
            })
            .GenerateImages(x => "text-font-size.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void FontFamily()
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
                            column.Spacing(10);

                            column.Item().Text("This is text with default font (Lato)");

                            column.Item().Text("This is text with Times New Roman font")
                                .FontFamily("Times New Roman");

                            column.Item().Text("This is text with Courier New font")
                                .FontFamily("Courier New");
                        });
                });
            })
            .GenerateImages(x => "text-font-family.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void FontColor()
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
                        .Text(text =>
                        {
                            text.Span("Each pixel consists of three sub-pixels: ");
                            text.Span("red").FontColor(Colors.Red.Medium);
                            text.Span(", ");
                            text.Span("green").FontColor(Colors.Green.Medium);
                            text.Span(" and ");
                            text.Span("blue").FontColor(Colors.Blue.Medium);
                            text.Span(".");
                        });
                    });
            })
            .GenerateImages(x => "text-font-color.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
    
    [Test]
    public void BackgroundColor()
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
                        .Text(text =>
                        {
                            text.Span("The term ");
                            text.Span("algorithm").BackgroundColor(Colors.Yellow.Lighten3).Bold();
                            text.Span(" refers to a set of rules or steps used to solve a problem.");
                        });
                });
            })
            .GenerateImages(x => "text-font-background.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
    
    [Test]
    public void Italic()
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
                        .Text(text =>
                        {
                            text.Span("In this sentence, the word ");
                            text.Span("important").Italic();
                            text.Span(" is emphasized using italics.");
                        });
                });
            })
            .GenerateImages(x => "text-font-italic.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
    
    [Test]
    public void FontWeight()
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
                        .Text(text =>
                        {
                            text.Span("This sentence demonstrates ");
                            text.Span("bold").Bold();
                            text.Span(", ");
                            text.Span("normal").NormalWeight();
                            text.Span(", ");
                            text.Span("light").Light();
                            text.Span(" and ");
                            text.Span("thin").Thin();
                            text.Span(" font weights.");
                        });
                });
            })
            .GenerateImages(x => "text-font-weight.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
    
    [Test]
    public void Subscript()
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
                        .Text(text =>
                        {
                            text.Span("H");
                            text.Span("2").Subscript();
                            text.Span("O is the chemical formula for water.");
                        });
                });
            })
            .GenerateImages(x => "text-subscript.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
    
    [Test]
    public void Superscript()
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
                        .Text(text =>
                        {
                            text.Span("E = mc");
                            text.Span("2").Superscript();
                            text.Span(" is the equation of mass-energy equivalence.");
                        });
                });
            })
            .GenerateImages(x => "text-superscript.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
    
    [Test]
    public void LineHeight()
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
                            column.Spacing(20);

                            float[] lineHeights = [0.75f, 1f, 2f];
                            var paragraph = Placeholders.Paragraph();
                            
                            foreach (var lineHeight in lineHeights)
                            {
                                column
                                    .Item()
                                    .Background(Colors.Grey.Lighten3)
                                    .Padding(5)
                                    .Text(paragraph)
                                    .FontSize(16)
                                    .LineHeight(lineHeight);
                            }
                        });
                });
            })
            .GenerateImages(x => "text-line-height.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
    
    [Test]
    public void LetterSpacing()
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
                            column.Spacing(20);
                            
                            var letterSpacing = new[] { -0.08f, 0f, 0.2f };
                            var paragraph = Placeholders.Sentence();

                            foreach (var spacing in letterSpacing)
                            {
                                column
                                    .Item()
                                    .Background(Colors.Grey.Lighten3)
                                    .Padding(5)
                                    .Text(paragraph)
                                    .FontSize(18)
                                    .LetterSpacing(spacing);
                            }
                        });
                });
            })
            .GenerateImages(x => "text-letter-spacing.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
    
    [Test]
    public void WordSpacing()
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
                            column.Spacing(20);
                            
                            var wordSpacing = new[] { -0.2f, 0f, 0.4f };
                            var paragraph = Placeholders.Sentence();

                            foreach (var spacing in wordSpacing)
                            {
                                column.Item()
                                    .Background(Colors.Grey.Lighten3)
                                    .Padding(5)
                                    .Text(paragraph)
                                    .FontSize(16)
                                    .WordSpacing(spacing);
                            }
                        });
                });
            })
            .GenerateImages(x => "text-word-spacing.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
    
    [Test]
    public void FontFallback()
    {
        Settings.UseEnvironmentFonts = false;
        
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
                        .Text("The Arabic word for programming is البرمجة.")
                        .FontFamily("Lato", "Noto Sans Arabic");
                });
            })
            .GenerateImages(x => "text-font-fallback.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
    
    [Test]
    public void FontFallbackEmoji()
    {
        Settings.UseEnvironmentFonts = false;
        
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
                        .Text("Popular emojis include 😊, 😂, ❤️, 👍, and 😎.")
                        .FontFamily("Lato", "Noto Emoji");
                });
            })
            .GenerateImages(x => "text-font-fallback-emoji.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
    
    [Test]
    public void TextFontFeatures()
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
                        .Row(row =>
                        {
                            row.Spacing(25);
    
                            row.RelativeItem()
                                .Background(Colors.Grey.Lighten3)
                                .Padding(10)
                                .Column(column =>
                                {
                                    column.Item().Text("Without ligatures").FontSize(16);
                                    
                                    column.Item()
                                        .Text("fly and fight")
                                        .FontSize(32)
                                        .DisableFontFeature(FontFeatures.StandardLigatures);
                                });
    
                            row.RelativeItem()
                                .Background(Colors.Grey.Lighten3)
                                .Padding(10)
                                .Column(column =>
                                {
                                    column.Item().Text("With ligatures").FontSize(16);
                                    
                                    column.Item().Text("fly and fight")
                                        .FontSize(32)
                                        .EnableFontFeature(FontFeatures.StandardLigatures);
                                });
                        });
                });
            })
            .GenerateImages(x => "text-font-features.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
    
    [Test]
    public void DecorationTypes()
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
                        .Text(text =>
                        {
                            text.Span("There are a couple of available text decorations: ");
                            text.Span("underline").Underline().FontColor(Colors.Red.Medium);
                            text.Span(", ");
                            text.Span("strikethrough").Strikethrough().FontColor(Colors.Green.Medium);
                            text.Span(" and ");
                            text.Span("overline").Overline().FontColor(Colors.Blue.Medium);
                            text.Span(". ");
                        });
                }); 
            })
            .GenerateImages(x => "text-decoration-types.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
    
    [Test]
    public void DecorationStyles()
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
                        .Text(text =>
                        {
                            text.Span("Moreover, the decoration can be ");
                            
                            text.Span("solid").FontColor(Colors.Indigo.Medium).Underline().DecorationSolid();
                            text.Span(", ");
                            text.Span("double").FontColor(Colors.Blue.Medium).Underline().DecorationDouble();
                            text.Span(", ");
                            text.Span("wavy").FontColor(Colors.LightBlue.Medium).Underline().DecorationWavy();
                            text.Span(", ");
                            text.Span("dotted").FontColor(Colors.Cyan.Medium).Underline().DecorationDotted();
                            text.Span(" or ");
                            text.Span("dashed").FontColor(Colors.Green.Medium)
                                .Underline().DecorationDashed();
                            text.Span(".");
                        });
                }); 
            })
            .GenerateImages(x => "text-decoration-styles.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
    
    [Test]
    public void DecorationsAdvanced()
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
                        .Text(text =>
                        {
                            text.Span("This text contains a ");
                            
                            text.Span("seriuos")
                                .Underline()
                                .DecorationWavy()
                                .DecorationColor(Colors.Red.Medium)
                                .DecorationThickness(2);
                            
                            text.Span(" typo.");
                        });
                });
            })
            .GenerateImages(x => "text-decoration-advanced.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
}