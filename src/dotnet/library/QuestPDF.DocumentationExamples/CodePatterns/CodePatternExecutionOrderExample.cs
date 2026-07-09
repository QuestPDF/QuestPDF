using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples.CodePatterns;

public class CodePatternExecutionOrderExample
{
    [Test]
    public void Example()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(400, 0));
                    page.MaxSize(new PageSize(400, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(25);

                            column.Item()
                                .Border(1)
                                .Background(Colors.Blue.Lighten4)
                                .Padding(15)
                                .Text("border → background → padding");
                            
                            column.Item()
                                .Border(1)
                                .Padding(15)
                                .Background(Colors.Blue.Lighten4)
                                .Text("border → padding → background");
    
                            column.Item()
                                .Background(Colors.Blue.Lighten4)
                                .Padding(15)
                                .Border(1)
                                .Text("background → padding → border");
                            
                            column.Item()
                                .Padding(15)
                                .Border(1)
                                .Background(Colors.Blue.Lighten4)
                                .Text("padding → border → background");
                        });
                });
            })
            .GenerateImages(x => "code-pattern-execution-order.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
}