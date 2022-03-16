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
            page.DefaultTextStyle(x => x.FontSize(20));

            page.Header()
                .Text("Hot Reload!")
                .SemiBold().FontSize(36).FontColor(Colors.Blue.Darken2);

            page.Content()
                .PaddingVertical(1, Unit.Centimetre)
                .Column(x =>
                {
                    x.Spacing(20);

                    x.Item().Text("VS: Start in debug mode with 'Hot Reload on Save' enabled.");
                    x.Item().Text("VSCode: Use dotnet watch to instantly see your changes in the previewer.");

                    x.Item().Text("Try it out here.");
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