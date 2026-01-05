using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;
using QuestPDF.ReportSample;
using QuestPDF.ReportSample.Layouts;

//ImagePlaceholder.Solid = true;

// var model = DataSource.GetReport();
// var report = new StandardReport(model);
// report.ShowInPreviewer();
//
// return;

var loremIpsum = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. Curabitur pretium tincidunt lacus. Nulla gravida orci a odio. Nullam varius, turpis et commodo pharetra, est eros bibendum elit, nec luctus magna felis sollicitudin mauris. Integer in mauris eu nibh euismod gravida. Duis ac tellus et risus vulputate vehicula. Donec lobortis risus a elit. Etiam tempor. Ut ullamcorper, ligula eu tempor congue, eros est euismod turpis, id tincidunt sapien risus a quam. Maecenas fermentum consequat mi. Donec fermentum. Pellentesque malesuada nulla a mi. Duis sapien sem, aliquet sed, vulputate eget, feugiat non, lacus. Morbi tristique senectus et netus et malesuada fames ac turpis egestas. Proin pharetra nonummy pede. Mauris et orci. Aenean nec lorem.";

Document
    .Create(container =>
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(2, Unit.Centimetre);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontSize(12));

            page.Header()
                .Text("Text Justify Example")
                .SemiBold().FontSize(28).FontColor(Colors.Blue.Darken2);

            page.Content()
                .PaddingVertical(1, Unit.Centimetre)
                .Column(x =>
                {
                    x.Spacing(20);

                    // Left Align Example
                    x.Item().Text("Left Aligned (Default):").Bold().FontSize(14);
                    x.Item().Text(text =>
                    {
                        text.AlignLeft();
                        text.Span(loremIpsum);
                    });

                    // Center Align Example
                    x.Item().Text("Center Aligned:").Bold().FontSize(14);
                    x.Item().Text(text =>
                    {
                        text.AlignCenter();
                        text.Span(loremIpsum);
                    });

                    // Right Align Example
                    x.Item().Text("Right Aligned:").Bold().FontSize(14);
                    x.Item().Text(text =>
                    {
                        text.AlignRight();
                        text.Span(loremIpsum);
                    });

                    // Justify Align Example
                    x.Item().Text("Justified:").Bold().FontSize(14);
                    x.Item().Text(text =>
                    {
                        text.AlignJustify();
                        text.Span(loremIpsum);
                    });

                    // Another Justify Example with multiple paragraphs
                    x.Item().Text("Justified with Multiple Paragraphs:").Bold().FontSize(14);
                    x.Item().Text(text =>
                    {
                        text.AlignJustify();
                        text.ParagraphSpacing(10);
                        
                        text.Line("Paragraph 1: " + loremIpsum);
                        text.Line("Paragraph 2: Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo. Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt.");
                        text.Line("Paragraph 3: At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis praesentium voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi sint occaecati cupiditate non provident, similique sunt in culpa qui officia deserunt mollitia animi, id est laborum et dolorum fuga.");
                    });
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