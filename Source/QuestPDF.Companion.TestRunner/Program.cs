using QuestPDF;
using QuestPDF.Companion;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.ReportSample;
using QuestPDF.ReportSample.Layouts;

Settings.License = LicenseType.Professional;

while (true)
{
    Document
        .Create(container =>
        {
            container.Page(page =>
            {
                var clampText = Placeholders.LoremIpsum();
                
                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(column =>
                    {
                        foreach (var i in Enumerable.Range(0, 100_000))
                        {
                            column.Item()
                                .Text(Placeholders.Paragraph())
                                .FontSize(20)
                                .ClampLines(4, "1234567890");
                        }
                    });
            });
        })
        .GeneratePdf();
    
    Console.WriteLine("-");
}

//await RunGenericException();
//await RunLayoutError();
await RunSimpleDocument();
//await RunReportDocument();
//await RunDocumentWithMultiplePages();

Task RunGenericException()
{
    return Document
        .Create(container =>
        {
            container.Page(page =>
            {
                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(x =>
                    {
                        x.Spacing(20);

                        x.Item().Text(Placeholders.LoremIpsum());
                        x.Item().Hyperlink("questpdf.com").Image(Placeholders.Image(300, 200));

                        throw new Exception("New exception");
                    });
            });
        })
        .ShowInCompanionAsync();
}

Task RunLayoutError()
{
    return Document
        .Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(20));

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(x =>
                    {
                        x.Spacing(20);
                    
                        x.Item().Text(Placeholders.LoremIpsum());
                    
                        foreach (var i in Enumerable.Range(0, 15))
                        {
                            x.Item().Background(Colors.Grey.Lighten3).MaxWidth(200).Container().Width(100 + i * 10).Height(50).Text($"Item {i}");
                        }
                    });
            });
        })
        .ShowInCompanionAsync();
}

Task RunSimpleDocument()
{
    return Document
        .Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(20));
        
                page.Header()
                    .Text("Hello PDF!")
                    .SemiBold().FontSize(36).FontColor(Colors.Blue.Medium);
        
                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(x =>
                    {
                        x.Spacing(20);
                
                        x.Item().Text(Placeholders.LoremIpsum());
                        x.Item().Hyperlink("questpdf.com").Image(Placeholders.Image(200, 100));
                    });
        
                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                    });
            });
        })
        .ShowInCompanionAsync();
}

Task RunReportDocument()
{
    ImagePlaceholder.Solid = true;
    
    var model = DataSource.GetReport();
    var report = new StandardReport(model);
    return report.ShowInCompanionAsync();
}

Task RunDocumentWithMultiplePages()
{
    return Document
        .Create(document =>
        {
            foreach (var i in Enumerable.Range(10, 10))
            {
                document.Page(page =>
                {
                    page.Size(new PageSize(i * 20, i * 30));
                    page.Margin(20);
                    page.Content().Background(Placeholders.BackgroundColor());
                });
            }
        })
        .ShowInCompanionAsync();
}

Task RunMergedDocument()
{
    var document1 = Document
        .Create(container =>
        {
            container.Page(page =>
            {
                page.Content()
                    .Text("Page 1!")
                    .SemiBold().FontSize(36).FontColor(Colors.Blue.Medium);
            });
        });
    var document2 = Document
        .Create(container =>
        {
            container.Page(page =>
            {
                page.Content()
                    .Text("Page 2!")
                    .SemiBold().FontSize(36).FontColor(Colors.Blue.Medium);
            });
        });

    var mergedDocument = Document.Merge(document1, document2);

    return mergedDocument.ShowInCompanionAsync();
}
