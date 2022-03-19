using System.Diagnostics;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;

Document
    .Create(container =>
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(2, Unit.Centimetre);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontSize(14));
                        
            page.Header()
                .Text("Hello PDF!")
                .SemiBold().FontSize(36).FontColor(Colors.Blue.Medium);

            page.Content()
                .PaddingVertical(1, Unit.Centimetre)
                .Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(30);
                        columns.RelativeColumn();
                        columns.ConstantColumn(75);
                        columns.ConstantColumn(75);
                        columns.ConstantColumn(75);
                    });

                    table.Header(header =>
                    {
                        var textStyle = TextStyle.Default.SemiBold();

                        header.Cell().Element(DefaultCellStyle).Text("#").Style(textStyle);
                        header.Cell().Element(DefaultCellStyle).Text("Item name").Style(textStyle);
                        header.Cell().Element(DefaultCellStyle).AlignRight().Text("Price").Style(textStyle);
                        header.Cell().Element(DefaultCellStyle).AlignRight().Text("Count").Style(textStyle);
                        header.Cell().Element(DefaultCellStyle).AlignRight().Text("Total").Style(textStyle);

                        static IContainer DefaultCellStyle(IContainer container)
                        {
                            return container.PaddingBottom(5).BorderBottom(1).PaddingBottom(5);
                        }
                    });

                    foreach(var i in Enumerable.Range(1, 100))
                    {
                        var price = Placeholders.Random.Next(100, 999) / 100f;
                        var count = Placeholders.Random.Next(1, 10);
                        var total = price * count;

                        table.Cell().Element(DefaultCellStyle).Text(i);
                        table.Cell().Element(DefaultCellStyle).Text(Placeholders.Label());
                        table.Cell().Element(DefaultCellStyle).AlignRight().Text($"{price:N2} $");
                        table.Cell().Element(DefaultCellStyle).AlignRight().Text(count);
                        table.Cell().Element(DefaultCellStyle).AlignRight().Text($"{total:N2} $");

                        static IContainer DefaultCellStyle(IContainer container)
                        {
                            return container.PaddingBottom(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten1).PaddingBottom(5);
                        }
                    }
                });
                        
            page.Footer()
                .AlignCenter()
                .Text(x =>
                {
                    x.Span("Page ");
                    x.CurrentPageNumber();
                    x.Span(" of ");
                    x.TotalPages();
                });
        });
    })
    .ShowInPreviewer();