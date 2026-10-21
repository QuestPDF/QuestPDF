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
                        #region docs:rounded-corners/consistent container
                        .Border(1, Colors.Black)
                        .Background(Colors.Grey.Lighten3)
                        .CornerRadius(25)
                        .Padding(25)
                        .Text("Container with consistently rounded corners");
                        #endregion
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
                        #region docs:rounded-corners/various container
                        .Border(1, Colors.Black)
                        .Background(Colors.Grey.Lighten3)
                        .CornerRadiusTopLeft(5)
                        .CornerRadiusTopRight(10)
                        .CornerRadiusBottomRight(20)
                        .CornerRadiusBottomLeft(40)
                        .Padding(25)
                        .Text("Container with rounded corners");
                        #endregion
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
                        #region docs:rounded-corners/complex container
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

                            var products = new[]
                            {
                                (Index: 1, Label: "Desk lamp", Price: "$34.99"),
                                (Index: 2, Label: "Notebook", Price: "$4.99"),
                                (Index: 3, Label: "Fountain pen", Price: "$59.00"),
                                (Index: 4, Label: "Paper clips", Price: "$1.49"),
                                (Index: 5, Label: "Monitor stand", Price: "$89.00")
                            };

                            foreach (var product in products)
                            {
                                table.Cell().Element(Style).Text(product.Index.ToString());
                                table.Cell().Element(Style).Text(product.Label);
                                table.Cell().Element(Style).Text(product.Price);
                                
                                IContainer Style(IContainer container)
                                {
                                    return container
                                        .Border(1, Colors.Grey.Darken2)
                                        .PaddingVertical(10)
                                        .PaddingHorizontal(15);
                                }
                            }
                        });
                        #endregion
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
                        #region docs:rounded-corners/image container
                        .CornerRadius(25)
                        .Image("Resources/landscape.jpg");
                        #endregion
                });
            })
            .GenerateImages(x => "rounded-corners-image.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
}