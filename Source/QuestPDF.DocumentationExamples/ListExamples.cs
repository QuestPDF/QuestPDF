using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class ListExamples
{
    [Test]
    public void BulletpointExample()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(350, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content() 
                        .Column(column =>
                        {
                            column.Spacing(10);
                            
                            foreach (var i in Enumerable.Range(1, 7))
                            {
                                column.Item().Row(row =>
                                {
                                    row.ConstantItem(26).Image("Resources/bulletpoint.png");
                                    row.ConstantItem(5);
                                    row.RelativeItem().Text(Placeholders.Label());
                                });
                            }
                        });
                });
            })
            .GenerateImages(x => "list-unordered.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
    
    [Test]
    public void OrderedExample()
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
                            
                            foreach (var i in Enumerable.Range(1, 11))
                            {
                                column.Item().Row(row =>
                                {
                                    row.ConstantItem(35).Text($"{i}.");
                                    row.RelativeItem().Text(Placeholders.Sentence());
                                });
                            }
                        });
                });
            })
            .GenerateImages(x => "list-ordered.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
    
    [Test]
    public void Nested()
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
                            const float nestingSize = 25;
                            
                            column.Spacing(10);
                            
                            column.Item()
                                .Text("Algorithm: Checking if a Number is Prime")
                                .FontSize(24).FontColor(Colors.Blue.Darken2);

                            AddListItem(0, "1.", "Handle special cases");
                            AddListItem(1, "a)", "If n is less than 2, return false (not prime).");
                            AddListItem(1, "b)", "If n is 2, return true (prime).");
                            
                            AddListItem(0, "2.", "Check divisibility");
                            AddListItem(1, "-", "Iterate through numbers from 2 to n - 1:");
                            AddListItem(2, "-", "If n is divisible by any of these numbers, return false.");
                            
                            AddListItem(0, "3.", "Return true (if no divisors were found, n is prime).");

                            void AddListItem(int nestingLevel, string bulletText, string text)
                            {
                                column.Item().Row(row =>
                                {
                                    row.ConstantItem(nestingSize * nestingLevel);
                                    row.ConstantItem(nestingSize).Text(bulletText);
                                    row.RelativeItem().Text(text);
                                });
                            }
                        });
                });
            })
            .GenerateImages(x => "list-nested.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
}