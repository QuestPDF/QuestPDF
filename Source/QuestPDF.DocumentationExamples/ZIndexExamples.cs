using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class ZIndexExamples
{
    [Test]
    public void Example()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(650, 0));
                    page.MaxSize(new PageSize(650, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content()
                        .PaddingVertical(15)
                        .Border(2)
                        .Row(row =>
                        {
                            row.RelativeItem()
                                .Background(Colors.Grey.Lighten3)
                                .Element(c => AddPricingItem(c, "Community", "Free"));
                            
                            row.RelativeItem()
                                .ZIndex(1) // -1 or 0 or 1
                                .Padding(-15)
                                .Border(1)
                                .Background(Colors.Grey.Lighten1)
                                .PaddingTop(15)
                                .Element(c => AddPricingItem(c, "Professional", "$699"));
                            
                            row.RelativeItem()
                                .Background(Colors.Grey.Lighten3)
                                .Element(c => AddPricingItem(c, "Enterprise", "$1999")); 

                            void AddPricingItem(IContainer container, string name, string formattedPrice)
                            {
                                container
                                    .Padding(25)
                                    .Column(column =>
                                    {
                                        column.Item().AlignCenter().Text(name).FontSize(24).Black();
                                        column.Item().AlignCenter().Text(formattedPrice).FontSize(20).SemiBold();
                                        
                                        column.Item().PaddingHorizontal(-25).PaddingVertical(10).LineHorizontal(1);
                                        
                                        foreach (var i in Enumerable.Range(1, 4))
                                        {
                                            column.Item()
                                                .PaddingTop(10)
                                                .AlignCenter()
                                                .Text(Placeholders.Label())
                                                .FontSize(16)
                                                .Light();
                                        }
                                    });
                            }
                        });
                });
            })
            .GenerateImages(x => "zindex-positive.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
}