using QuestPDF;
using QuestPDF.Companion;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.ReportSample;
using QuestPDF.ReportSample.Layouts;

Settings.License = LicenseType.Professional;

//await RunGenericException();
//await RunLayoutError();
await RunSimpleDocument();
//await RunReportDocument();

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
                        x.Item().Hyperlink("questpdf.com").Image(Placeholders.Image(200, 200));

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
