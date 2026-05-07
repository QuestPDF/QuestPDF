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
//await RunDocumentWithMultiplePages();
//await RunTextLayoutMeasurerDocument();
return;

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

Task RunTextLayoutMeasurerDocument()
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
                        x.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.ConstantColumn(120);
                                columns.ConstantColumn(120);
                            });

                            table.Cell()
                                .Border(1)
                                .PaddingLeft(30)
                                .Text("Text");
                            table.Cell()
                                .Border(1)
                                .AlignCenter()
                                .AlignMiddle()
                                .Text("Line Count");
                            table.Cell()
                                .Border(1)
                                .PaddingRight(5)
                                .AlignRight()
                                .AlignMiddle()
                                .Text("Line Height");

                            var style = TextStyle
                                .Default
                                .FontSize(20)
                                .FontFamily("Arial")
                                .EnableFontFeature(FontFeatures.StandardLigatures);

                            string[] rows =
                            [
                                "Hello World",
                                "Hello World Hello AAA1",
                                "Hello World Hello AAAA"
                            ];

                            foreach (var text in rows)
                            {
                                const float textWidth = 241.6f; // Quest Pdf Companion UI
                                var lineCount = TextLayoutMeasurer.GetLineCount(text, style, textWidth, 30f);
                                var lineHeight = TextLayoutMeasurer.GetHeight(text, style, textWidth, 30f);

                                table.Cell()
                                    .Border(1)
                                    .PaddingLeft(30)
                                    .Text(text)
                                    .Style(style);

                                table.Cell()
                                    .Border(1)
                                    .AlignCenter()
                                    .AlignMiddle()
                                    .Text(lineCount.ToString());

                                table.Cell()
                                    .Border(1)
                                    .PaddingRight(5)
                                    .AlignRight()
                                    .AlignMiddle()
                                    .Text($"{lineHeight:F2}");
                            }
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
        .ShowInCompanionAsync();
}