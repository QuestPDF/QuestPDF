using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class RoundedCornersExamples
{
    [Test]
    public void Consistent()
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
                    page.PageColor(Colors.White);

                    page.Content()
                        .Border(1, Colors.Black)
                        .Background(Colors.Grey.Lighten3)
                        .CornerRadius(25)
                        .Padding(25)
                        .Text("Container with consistently rounded corners");
                });
            })
            .GenerateImages(x => "rounded-corners-consistent.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
    
    [Test]
    public void Various()
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
                    page.PageColor(Colors.White);

                    page.Content()
                        .Border(1, Colors.Black)
                        .Background(Colors.Grey.Lighten3)
                        .CornerRadiusTopLeft(5)
                        .CornerRadiusTopRight(10)
                        .CornerRadiusBottomRight(20)
                        .CornerRadiusBottomLeft(40)
                        .Padding(25)
                        .Text("Container with rounded corners");
                });
            })
            .GenerateImages(x => "rounded-corners-various.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
    
    [Test]
    public void Complex()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(550, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);
                    page.PageColor(Colors.White);

                    page.Content()
                        .Border(1, Colors.Black)
                        .CornerRadius(15)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(100);
                                columns.RelativeColumn();
                                columns.ConstantColumn(150);
                            });
                            
                            table.Header(header =>
                            {
                                header.Cell().Element(Style).Text("Index");
                                header.Cell().Element(Style).Text("Label");
                                header.Cell().Element(Style).Text("Price");

                                IContainer Style(IContainer container)
                                {
                                    return container
                                        .Border(1, Colors.Grey.Darken2)
                                        .Background(Colors.Grey.Lighten3)
                                        .PaddingVertical(10)
                                        .PaddingHorizontal(15)
                                        .DefaultTextStyle(x => x.Bold());
                                }
                            });

                            foreach (var index in Enumerable.Range(1, 5))
                            {
                                table.Cell().Element(Style).Text(index.ToString());
                                table.Cell().Element(Style).Text(Placeholders.Label());
                                table.Cell().Element(Style).Text(Placeholders.Price());
                                
                                IContainer Style(IContainer container)
                                {
                                    return container
                                        .Border(1, Colors.Grey.Darken2)
                                        .PaddingVertical(10)
                                        .PaddingHorizontal(15);
                                }
                            }
                        });
                });
            })
            .GenerateImages(x => "rounded-corners-complex.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
    
    [Test]
    public void Image()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(450, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);
                    page.PageColor(Colors.White);

                    page.Content()
                        .CornerRadius(25)
                        .Image("Resources/landscape.jpg");
                });
            })
            .GenerateImages(x => "rounded-corners-image.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
}