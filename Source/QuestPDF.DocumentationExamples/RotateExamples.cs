using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class RotateExamples
{
    [Test]
    public void Example()
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
                            row.AutoItem()
                                .RotateLeft()
                                .AlignCenter()
                                .Text("Definition")
                                .Bold().FontColor(Colors.Blue.Darken2);
                            
                            row.AutoItem()
                                .PaddingHorizontal(15)
                                .LineVertical(2).LineColor(Colors.Blue.Medium);
                            
                            row.RelativeItem()
                                .Background(Colors.Blue.Lighten5)
                                .Padding(15)
                                .Text(text =>
                                {
                                    text.Span("A variable").Bold();
                                    text.Span(" is a named storage location in memory that holds a value which can be modified during program execution.");
                                });
                        });
                });
            })
            .GenerateImages(x => "rotate.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void FreeExample()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(1000, 1000));

                    page.Content()
                        .Background(Colors.Grey.Lighten2)
                        .Padding(25)
                        .Row(row =>
                        {
                            row.Spacing(25);
                            
                            AddIcon(0);
                            AddIcon(30);
                            AddIcon(45);
                            AddIcon(80);

                            void AddIcon(float angle)
                            {
                                const float itemSize = 100;
                                
                                row.AutoItem()
                                    .Width(itemSize)
                                    .AspectRatio(1)
                                    
                                    .TranslateX(itemSize / 2)
                                    .TranslateY(itemSize / 2)
                                    
                                    .Rotate(angle)
                                    
                                    .TranslateX(-itemSize / 2)
                                    .TranslateY(-itemSize / 2)
                                    
                                    .Svg("Resources/compass.svg");
                            }
                        });
                });
            })
            .GenerateImages(x => "rotate-free.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
}