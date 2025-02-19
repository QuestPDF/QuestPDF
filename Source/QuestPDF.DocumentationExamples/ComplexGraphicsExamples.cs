using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class ComplexGraphicsExamples
{
    [Test]
    public void RoundedRectangleWithGradient()
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
                        .Layers(layers =>
                        {
                            layers.Layer().Svg(size =>
                            {
                                return $"""
                                        <svg width="{size.Width}" height="{size.Height}" xmlns="http://www.w3.org/2000/svg">
                                            <defs>
                                              <linearGradient id="backgroundGradient" x1="0%" y1="0%" x2="100%" y2="100%">
                                                <stop stop-color="#00E5FF" offset="0%"/>
                                                <stop stop-color="#2979FF" offset="100%"/>
                                              </linearGradient>
                                            </defs>
                                        
                                            <rect x="0" y="0" width="{size.Width}" height="{size.Height}" rx="{size.Height / 2}" ry="{size.Height / 2}" fill="url(#backgroundGradient)" />
                                        </svg>
                                        """;
                            });

                            layers.PrimaryLayer()
                                .PaddingVertical(10)
                                .PaddingHorizontal(20)
                                .Text("QuestPDF")
                                .FontColor(Colors.White)
                                .FontSize(32)
                                .ExtraBlack();
                        });
                });
            })
            .GenerateImages(x => "complex-graphics-rounded-rectangle-with-gradient.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
    
    [Test]
    public void DottedLine()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(500, 0));
                    page.MaxSize(new PageSize(500, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);
                    
                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(5);
                            
                            foreach (var i in Enumerable.Range(1, 5))
                            {
                                var pageNumber = i * 7 + 4;
                                
                                column.Item().Row(row =>
                                {
                                    row.AutoItem().Text($"{i}.");
                                    row.ConstantItem(10);
                                    row.AutoItem().Text(Placeholders.Label());

                                    row.RelativeItem().PaddingHorizontal(3).TranslateY(20).Height(2).Svg(size =>
                                    {
                                        return $"""
                                                <svg width="{size.Width}" height="{size.Height}" xmlns="http://www.w3.org/2000/svg">
                                                    <line x1="0" y1="0" x2="{size.Width}" y2="0" fill="none" stroke="black" stroke-width="2" stroke-dasharray="2 6" />
                                                </svg>
                                                """;
                                    });

                                    row.AutoItem().Text($"{pageNumber}");
                                });
                            }
                        });
                });
            })
            .GenerateImages(x => "complex-graphics-dotted-line.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
}