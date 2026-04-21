using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class ShowLastExamples
{
    [Test]
    public void Example()
    {
        var items = new[]
        {
            ("Web Development Package", 3, 150.00m),
            ("UI/UX Design Review", 1, 450.00m),
            ("Database Optimization", 2, 200.00m),
            ("API Integration Module", 4, 125.00m),
            ("Security Audit", 1, 600.00m),
            ("Performance Testing", 2, 175.00m),
            ("Cloud Migration Support", 3, 300.00m),
            ("Documentation Update", 5, 80.00m),
            ("Training Sessions", 2, 250.00m),
            ("Maintenance Plan (Monthly)", 6, 100.00m),
            ("Web Development Package", 3, 150.00m),
            ("UI/UX Design Review", 1, 450.00m),
            ("Database Optimization", 2, 200.00m),
            ("API Integration Module", 4, 125.00m),
            ("Security Audit", 1, 600.00m),
            ("Performance Testing", 2, 175.00m),
            ("Cloud Migration Support", 3, 300.00m),
            ("Documentation Update", 5, 80.00m),
            ("Training Sessions", 2, 250.00m),
            ("Maintenance Plan (Monthly)", 6, 100.00m),
        };

        var total = items.Sum(x => x.Item2 * x.Item3);

        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(400, 380);
                    page.DefaultTextStyle(x => x.FontSize(16));
                    page.Margin(25);

                    page.Content()
                        .Shrink()
                        .Background(Colors.Grey.Lighten4)
                        .Decoration(decoration =>
                        {
                            decoration.Before()
                                .Column(column =>
                                {
                                    column.Item()
                                        .ShowOnce()
                                        .PaddingBottom(10)
                                        .Text("Invoice #2025-0042").FontSize(22).Bold();

                                    column.Item()
                                        .SkipOnce()
                                        .PaddingBottom(10)
                                        .Text("Invoice #2025-0042 [continued]").FontSize(18).Bold();

                                    column.Item()
                                        .BorderBottom(1)
                                        .BorderColor(Colors.Grey.Medium)
                                        .PaddingBottom(5)
                                        .Row(row =>
                                        {
                                            row.RelativeItem(3).Text("Item").SemiBold();
                                            row.RelativeItem(1).AlignRight().Text("Qty").SemiBold();
                                            row.RelativeItem(1).AlignRight().Text("Price").SemiBold();
                                            row.RelativeItem(1).AlignRight().Text("Total").SemiBold();
                                        });
                                });

                            decoration.Content()
                                .ExtendHorizontal()
                                .Column(column =>
                                {
                                    foreach (var item in items)
                                    {
                                        column.Item()
                                            .PaddingVertical(4)
                                            .Row(row =>
                                            {
                                                row.RelativeItem(3).Text(item.Item1);
                                                row.RelativeItem(1).AlignRight().Text($"{item.Item2}");
                                                row.RelativeItem(1).AlignRight().Text($"${item.Item3:N0}");
                                                row.RelativeItem(1).AlignRight().Text($"${item.Item2 * item.Item3:N0}");
                                            });
                                    }
                                });

                            decoration.After()
                                .PaddingTop(5)
                                .Column(column =>
                                {
                                    // column.Item()
                                    //     .SkipLast()
                                    //     .Text("Continued on next page...")
                                    //     .Italic()
                                    //     .FontSize(12)
                                    //     .FontColor(Colors.Grey.Darken1);

                                    column.Item()
                                        .ShowLast()
                                        .BorderTop(2)
                                        .BorderColor(Colors.Black)
                                        .PaddingTop(5)
                                        .Row(row =>
                                        {
                                            row.RelativeItem().Text("Grand Total").FontSize(18).Bold();
                                            row.RelativeItem().AlignRight().Text($"${total:N2}").FontSize(18).Bold();
                                        });
                                });
                        });
                });
            })
            .GeneratePdfAndShow();
        // .GenerateImages(x => $"show-last-{x}.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
}
