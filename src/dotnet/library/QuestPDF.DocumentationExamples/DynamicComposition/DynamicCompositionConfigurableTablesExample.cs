using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples.DynamicComposition;

public class DynamicCompositionConfigurableTablesExample
{
    [Test]
    public void Example()
    {
        var reportColumns = new List<ReportColumn>
        {
            new(Header: "SKU",        IsConstantWidth: true,  Size: 100, PropertyName: "sku"),
            new(Header: "Product",    IsConstantWidth: false, Size: 3,   PropertyName: "name"),
            new(Header: "Warehouse",  IsConstantWidth: false, Size: 2,   PropertyName: "warehouse"),
            new(Header: "In stock",   IsConstantWidth: true,  Size: 90,  PropertyName: "stock"),
            new(Header: "Unit price", IsConstantWidth: true,  Size: 110, PropertyName: "price")
        };

        var products = new List<Dictionary<string, string>>
        {
            new() { ["sku"] = "MO-1042", ["name"] = "Wireless Optical Mouse", ["warehouse"] = "Gdansk", ["stock"] = "145", ["price"] = "$24.99" },
            new() { ["sku"] = "KB-2205", ["name"] = "Mechanical Keyboard",    ["warehouse"] = "Warsaw", ["stock"] = "38",  ["price"] = "$89.50" },
            new() { ["sku"] = "HU-3310", ["name"] = "USB-C Hub 7-in-1",       ["warehouse"] = "Warsaw", ["stock"] = "76",  ["price"] = "$45.00" },
            new() { ["sku"] = "MS-4470", ["name"] = "27\" 4K Monitor Stand",  ["warehouse"] = "Krakow", ["stock"] = "12",  ["price"] = "$129.00" },
            new() { ["sku"] = "WC-5521", ["name"] = "Full HD Webcam",         ["warehouse"] = "Gdansk", ["stock"] = "210", ["price"] = "$59.00" }
        };

        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(720, 0));
                    page.MaxSize(new PageSize(720, 1000));
                    page.DefaultTextStyle(x => x.FontSize(16));
                    page.Margin(25);

                    page.Content()
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                foreach (var column in reportColumns)
                                {
                                    if (column.IsConstantWidth)
                                        columns.ConstantColumn(column.Size);
                                    else
                                        columns.RelativeColumn(column.Size);
                                }
                            });

                            table.Header(header =>
                            {
                                foreach (var column in reportColumns)
                                    header.Cell().Element(HeaderCellStyle).Text(column.Header);

                                static IContainer HeaderCellStyle(IContainer container)
                                {
                                    return container
                                        .Background(Colors.Blue.Darken2)
                                        .DefaultTextStyle(x => x.FontColor(Colors.White).SemiBold())
                                        .PaddingVertical(8)
                                        .PaddingHorizontal(10);
                                }
                            });

                            foreach (var product in products)
                            {
                                foreach (var column in reportColumns)
                                    table.Cell().Element(CellStyle).Text(product.GetValueOrDefault(column.PropertyName));
                            }

                            static IContainer CellStyle(IContainer container)
                            {
                                return container
                                    .BorderBottom(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .PaddingVertical(8)
                                    .PaddingHorizontal(10);
                            }
                        });
                });
            })
            .GenerateImages(x => "dynamic-composition-runtime-columns.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
}

public sealed record ReportColumn(string Header, bool IsConstantWidth, float Size, string PropertyName);
