// See https://aka.ms/new-console-template for more information

using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;

QuestPDF.Settings.EnableCaching = true;
QuestPDF.Settings.EnableDebugging = true;

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
                .Text("Hello PDF!")
                .SemiBold().FontSize(36).FontColor(Colors.Blue.Medium);
                        
            page.Content()
                .PaddingVertical(1, Unit.Centimetre)
                .Column(x =>
                {
                    x.Spacing(20);
                                
                    x.Item().Text(Placeholders.LoremIpsum());
                    x.Item().Image(Placeholders.Image(200, 100));

                    foreach (var i in Enumerable.Range(0, 50))
                    {
                        x.Item()
                            .Height(80).Width(100 + i * 5)
                            .Background(Colors.Grey.Lighten3)
                            .AlignCenter().AlignMiddle()
                            .Text(i);
                    }

                    //x.Item().Width(1000);
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

