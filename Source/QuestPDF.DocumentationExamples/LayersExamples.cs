using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class LayersExamples
{
    [Test]
    public void Example()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.ContinuousSize(450);
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);
                    
                    page.Content()
                        .Column(column =>
                        {
                            column.Item().PaddingBottom(15).Text("Proposed Business Card Design:").Bold();
                            
                            column.Item()
                                .AspectRatio(4 / 3f)
                                .Layers(layers =>
                                {
                                    layers.Layer().Image("Resources/card-background.jpg").FitUnproportionally();

                                    layers.PrimaryLayer()
                                        .TranslateY(75)
                                        .Column(innerColumn =>
                                        {
                                            innerColumn.Item()
                                                .AlignCenter()
                                                .Text("Horizon Ventures")
                                                .Bold().FontSize(32).FontColor(Colors.Blue.Darken2);

                                            innerColumn.Item().AlignCenter().Text("Your journey begins here");
                                        });
                                });
                        });
                });
            })
            .GenerateImages(x => "layers.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
}