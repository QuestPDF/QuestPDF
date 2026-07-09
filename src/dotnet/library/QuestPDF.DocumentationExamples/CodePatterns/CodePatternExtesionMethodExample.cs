using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples.CodePatterns;

public class CodePatternExtensionMethodExample
{
    [Test]
    public void Example()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(600, 0));
                    page.MaxSize(new PageSize(600, 1000));
                    page.DefaultTextStyle(x => x.FontSize(14));
                    page.Margin(25);

                    page.Content()
                        .Border(1)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(3);
                            });
                            
                            table.Cell().TableLabelCell("Product name");
                            table.Cell().TableValueCell().Text(Placeholders.Label());
                            
                            table.Cell().TableLabelCell("Description");
                            table.Cell().TableValueCell().Text(Placeholders.Sentence());
                            
                            table.Cell().TableLabelCell("Price");
                            table.Cell().TableValueCell().Text(Placeholders.Price());
                            
                            table.Cell().TableLabelCell("Date of production");
                            table.Cell().TableValueCell().Text(Placeholders.ShortDate());
                            
                            table.Cell().ColumnSpan(2).TableLabelCell("Photo of the product");
                            table.Cell().ColumnSpan(2).TableValueCell().AspectRatio(16 / 9f).Image(Placeholders.Image);
                        });
                });
            })
            .GenerateImages(x => "code-pattern-extension-methods.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
}

public static class TableExtensions
{
    private static IContainer TableCellStyle(this IContainer container, string backgroundColor)
    {
        return container
            .Border(1)
            .BorderColor(Colors.Black)
            .Background(backgroundColor)
            .Padding(10);
    }
    
    public static void TableLabelCell(this IContainer container, string text)
    {
        container
            .TableCellStyle(Colors.Grey.Lighten3)
            .Text(text)
            .Bold();
    }
    
    public static IContainer TableValueCell(this IContainer container)
    {
        return container.TableCellStyle(Colors.Transparent);
    }
}