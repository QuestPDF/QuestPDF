using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class MultiColumnExamples
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
                    page.MaxSize(new PageSize(650, 650));
                    page.DefaultTextStyle(x => x.FontSize(12));
                    page.Margin(25);

                    page.Content()
                        .MultiColumn(multiColumn =>
                        {
                            multiColumn.Columns(3);
                            multiColumn.Spacing(25);
    
                            multiColumn
                                .Content()
                                .Column(column =>
                                {
                                    column.Spacing(15);

                                    foreach (var sectionId in Enumerable.Range(0, 3))
                                    {
                                        foreach (var textId in Enumerable.Range(0, 3))
                                            column.Item().Text(Placeholders.Paragraph()).Justify();

                                        column.Item().AspectRatio(21 / 9f).Image(Placeholders.Image);
                                    }
                                });
                        });
                });
            })
            .GenerateImages(x => "multicolumn-example.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.High, RasterDpi = 144 });
    }
    
    [Test]
    public void SpacerExample()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(450, 0));
                    page.MaxSize(new PageSize(450, 550));
                    page.DefaultTextStyle(x => x.FontSize(12));
                    page.Margin(25);

                    page.Content()
                        .MultiColumn(multiColumn =>
                        {
                            multiColumn.Columns(2);
                            multiColumn.Spacing(50);
    
                            multiColumn
                                .Spacer()
                                .AlignCenter()
                                .LineVertical(2)
                                .LineColor(Colors.Grey.Medium);
                            
                            multiColumn
                                .Content()
                                .Column(column =>
                                {
                                    column.Spacing(15);

                                    foreach (var textId in Enumerable.Range(0, 5))
                                        column.Item().Text(Placeholders.Paragraph()).Justify();
                                });
                        });
                });
            })
            .GenerateImages(x => "multicolumn-spacer.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.High, RasterDpi = 144 });
    }
    
    [Test]
    public void BalanceHeightWithExample()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(14));
                    page.Margin(30);

                    page.Content()
                        .MultiColumn(multiColumn =>
                        {
                            multiColumn.Spacing(30);
                            multiColumn.BalanceHeight();

                            multiColumn
                                .Content()
                                .Column(column =>
                                {
                                    column.Spacing(15);
                                    
                                    foreach (var textId in Enumerable.Range(0, 8))
                                        column.Item().Text(Placeholders.Paragraph()).Justify();
                                });
                        });
                });
            })
            .GenerateImages(x => "multicolumn-balance-height-with.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.High, RasterDpi = 144 });
    }
}