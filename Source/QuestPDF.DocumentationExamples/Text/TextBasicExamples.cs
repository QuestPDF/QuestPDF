using System.Security.Cryptography;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples.Text;

public class TextBasicExamples
{
    [Test]
    public void Basic()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(500, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content()
                        .Text("Sample text");
                });
            })
            .GenerateImages(x => "text-basic.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void BasicWithStyle()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(500, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(10);

                            column.Item()
                                .Element(CellStyle)
                                .Text("Text with blue color")
                                .FontColor(Colors.Blue.Darken1);

                            column.Item()
                                .Element(CellStyle)
                                .Text("Bold and underlined text")
                                .Bold()
                                .Underline();

                            column.Item()
                                .Element(CellStyle)
                                .Text("Centered small text")
                                .FontSize(12)
                                .AlignCenter();

                            static IContainer CellStyle(IContainer container) =>
                                container.Background(Colors.Grey.Lighten3).Padding(10);
                        });
                });
            })
            .GenerateImages(x => "text-basic-descriptor.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void Rich()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(500, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content()
                        .Text(text =>
                        {
                            text.AlignCenter();

                            text.Span("The ");
                            text.Span("chemical formula").Underline();
                            text.Span(" of ");
                            text.Span("sulfuric acid").BackgroundColor(Colors.Amber.Lighten3);
                            text.Span(" is H");
                            text.Span("2").Subscript();
                            text.Span("SO");
                            text.Span("4").Subscript();
                            text.Span(".");
                        });
                });
            })
            .GenerateImages(x => "text-rich.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void StyleInheritance()
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
                        .DefaultTextStyle(style => style.FontSize(20))
                        .Column(column =>
                        {
                            column.Spacing(10);
                            
                            column.Item().Text("Products").ExtraBold().Underline().DecorationThickness(2);
                            
                            column.Item().Text("Comments: " + Placeholders.Sentence());
                            
                            column.Item()
                                .DefaultTextStyle(style => style.FontSize(14))
                                .Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.ConstantColumn(30);
                                        columns.RelativeColumn(1);
                                        columns.RelativeColumn(2);
                                    });
                            
                                    table.Header(header =>
                                    {
                                        header.Cell().Element(Style).Text("ID");
                                        header.Cell().Element(Style).Text("Name");
                                        header.Cell().Element(Style).Text("Description");

                                        IContainer Style(IContainer container)
                                        {
                                            return container
                                                .Background(Colors.Grey.Lighten3)
                                                .BorderBottom(1)
                                                .PaddingHorizontal(5)
                                                .PaddingVertical(10)
                                                .DefaultTextStyle(x => x.Bold().FontColor(Colors.Blue.Medium));
                                        }
                                    });

                                    foreach (var i in Enumerable.Range(0, 5))
                                    {
                                        table.Cell().Element(Style).Text(i.ToString()).Bold();
                                        table.Cell().Element(Style).Text(Placeholders.Label());
                                        table.Cell().Element(Style).Text(Placeholders.Sentence());
                                    }
                                
                                    IContainer Style(IContainer container)
                                    {
                                        return container.Padding(5);
                                    }
                                });
                        });
                });
            })
            .GenerateImages(x => "text-style-inheritance.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void PageNumber()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A5);
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content()
                        .Extend()
                        .Placeholder();
                    
                    page.Footer()
                        .PaddingTop(25)
                        .AlignCenter()
                        .Text("3 / 10");
                        // .Text(text =>
                        // {
                        //     text.CurrentPageNumber();
                        //     text.Span(" / ");
                        //     text.TotalPages();
                        // });
                });
            })
            .GenerateImages(x => "text-page-number.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void PageNumberFormat()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A5);
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content()
                        .Text(text =>
                        {
                            text.CurrentPageNumber().Format(FormatWithLeadingZeros);
                        });
                    
                    static string FormatWithLeadingZeros(int? pageNumber)
                    {
                        const int expectedLength = 3;
                        pageNumber ??= 1;
                        return pageNumber.Value.ToString($"D{expectedLength}");
                    }
                });
            })
            .GenerateImages(x => "text-page-number-format.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void Hyperlink()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A6.Landscape());
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content()
                        .Text(text =>
                        {
                            var hyperlinkStyle = TextStyle.Default
                                .FontColor(Colors.Blue.Medium)
                                .Underline();
        
                            text.Span("To learn more about QuestPDF, please visit its ");
                            text.Hyperlink("homepage", "https://www.questpdf.com/").Style(hyperlinkStyle);
                            text.Span(", ");
                            text.Hyperlink("GitHub repository", "https://github.com/QuestPDF/QuestPDF").Style(hyperlinkStyle);
                            text.Span(" and ");
                            text.Hyperlink("NuGet package page", "https://www.nuget.org/packages/QuestPDF").Style(hyperlinkStyle);
                            text.Span(".");
                        });
                });
            })
            .GeneratePdf("text-hyperlink.pdf");
    }

    
}