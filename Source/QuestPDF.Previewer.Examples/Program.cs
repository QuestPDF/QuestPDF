using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;

//ImagePlaceholder.Solid = true;

// var model = DataSource.GetReport();
// var report = new StandardReport(model);
// report.ShowInPreviewer();
//
// return;

Document
    .Create(container =>
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(2, Unit.Centimetre);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontSize(20));

            page.Header()
                .Text("Hot Reload!")
                .SemiBold().FontSize(36).FontColor(Colors.Blue.Darken2);

            page.Content()
                .PaddingVertical(1, Unit.Centimetre)
                .Column(x =>
                {
                    x.Spacing(20);

                    x.Item().Table(t =>
                    {
                        t.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn();
                            c.RelativeColumn(3);
                        });

                        t.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("Visual Studio");
                        t.Cell().Border(1).Padding(5).Text("Start in debug mode with 'Hot Reload on Save' enabled.");
                        t.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("Command line");
                        t.Cell().Border(1).Padding(5).Text("Run 'dotnet watch'.");
                    });

                    x.Item().Text("Modify this line and the preview should show your changes instantly.");
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
    .ShowInPreviewer();