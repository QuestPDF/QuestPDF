using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples.CodePatterns;

public class CodePatternContentStylingExample
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
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(50);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                            });
                            
                            table.Header(header =>
                            {
                                header.Cell().Style(HeaderCell).Text("#");
                                header.Cell().Style(HeaderCell).Text("Product Name");
                                header.Cell().Style(HeaderCell).Text("Description");

                                IContainer HeaderCell(IContainer container)
                                {
                                    return container
                                        .Background(Colors.Blue.Lighten5)
                                        .Padding(10)
                                        .DefaultTextStyle(TextStyle.Default.FontColor(Colors.Blue.Darken4).Bold());
                                }
                            });

                            foreach (var i in Enumerable.Range(1, 5))
                            {
                                table.Cell().Style(Cell).Text(i.ToString());
                                table.Cell().Style(Cell).Text(Placeholders.Label());
                                table.Cell().Style(Cell).Text(Placeholders.Sentence());
                            }

                            IContainer Cell(IContainer container)
                            { 
                                return container
                                    .BorderTop(2)
                                    .BorderColor(Colors.Blue.Lighten3)
                                    .Padding(10);
                            }
                        });
                });
            })
            .GenerateImages(x => $"code-pattern-content-styling.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
}