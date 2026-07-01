using System;
using System.Diagnostics;
using System.IO;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Tests.Shared;

public static class PdfSmokeTests
{
    private const string InlineSvg = "<svg width=\"120\" height=\"80\" viewBox=\"0 0 120 80\" xmlns=\"http://www.w3.org/2000/svg\"><rect width=\"120\" height=\"80\" rx=\"10\" fill=\"#E3F2FD\"/><circle cx=\"34\" cy=\"40\" r=\"18\" fill=\"#1976D2\"/><path d=\"M62 25h38v8H62zm0 18h30v8H62zm0 18h22v8H62z\" fill=\"#0D47A1\"/></svg>";
    private static bool TraceListenerConfigured;

    public static byte[] GeneratePdfBytes()
    {
        ConfigureQuestPdf();
        return CreateDocument().GeneratePdf();
    }

    public static string GeneratePdfFile(string outputDirectory, string fileName = "questpdf-integration-smoke.pdf")
    {
        ConfigureQuestPdf();
        Directory.CreateDirectory(outputDirectory);

        var filePath = Path.Combine(outputDirectory, fileName);
        CreateDocument().GeneratePdf(filePath);
        PdfValidator.ValidateFile(filePath);

        return filePath;
    }

    private static void ConfigureQuestPdf()
    {
        if (!TraceListenerConfigured)
        {
            Trace.Listeners.Add(new ConsoleTraceListener(useErrorStream: true));
            Trace.AutoFlush = true;
            TraceListenerConfigured = true;
        }

        QuestPDF.Settings.License = LicenseType.Community;
    }

    private static IDocument CreateDocument()
    {
        return Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(36);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header()
                    .BorderBottom(1)
                    .BorderColor(Colors.Grey.Lighten1)
                    .PaddingBottom(10)
                    .Row(row =>
                    {
                        row.RelativeItem().Column(column =>
                        {
                            column.Item().Text("QuestPDF package integration").FontSize(20).Bold();
                            column.Item().Text("Publish output smoke test").FontColor(Colors.Grey.Darken2);
                        });

                        row.ConstantItem(120).Height(80).Svg(InlineSvg).FitArea();
                    });

                page.Content()
                    .PaddingVertical(18)
                    .Column(column =>
                    {
                        column.Spacing(12);
                        column.Item().Element(BuildSummaryPanel);
                        column.Item().Element(BuildFeatureTable);
                        column.Item().Element(BuildMultiColumnContent);
                        column.Item().PageBreak();
                        column.Item().Element(BuildPaginationContent);
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Page ");
                        text.CurrentPageNumber();
                        text.Span(" of ");
                        text.TotalPages();
                    });
            });
        });
    }

    private static void BuildSummaryPanel(IContainer container)
    {
        container
            .Background(Colors.Blue.Lighten5)
            .Border(1)
            .BorderColor(Colors.Blue.Lighten2)
            .Padding(12)
            .Column(column =>
            {
                column.Spacing(6);
                column.Item().Text("What this document exercises").SemiBold();
                column.Item().Text("Text shaping, tables, SVG rendering, pagination, links, columns, borders, backgrounds, and dynamic layout in a published application.");
                column.Item().Hyperlink("https://www.questpdf.com").Text("QuestPDF documentation link").FontColor(Colors.Blue.Darken2);
            });
    }

    private static void BuildFeatureTable(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(80);
                columns.RelativeColumn();
                columns.ConstantColumn(80);
            });

            table.Header(header =>
            {
                header.Cell().Element(HeaderCell).Text("Area");
                header.Cell().Element(HeaderCell).Text("Scenario");
                header.Cell().Element(HeaderCell).AlignRight().Text("Status");
            });

            AddRow(table, "Text", "Multiple text spans, font weights, colors, and page counters", "OK");
            AddRow(table, "Layout", "Rows, columns, tables, padding, borders, and pagination", "OK");
            AddRow(table, "SVG", "Inline SVG parsed and rendered by native dependencies", "OK");
            AddRow(table, "Native", "PDF generated through Skia and qpdf libraries selected during publish", "OK");
        });
    }

    private static IContainer HeaderCell(IContainer container)
    {
        return container
            .Background(Colors.Grey.Darken3)
            .DefaultTextStyle(x => x.FontColor(Colors.White).SemiBold())
            .PaddingVertical(6)
            .PaddingHorizontal(8);
    }

    private static void AddRow(TableDescriptor table, string area, string scenario, string status)
    {
        table.Cell().Element(BodyCell).Text(area);
        table.Cell().Element(BodyCell).Text(scenario);
        table.Cell().Element(BodyCell).AlignRight().Text(status);
    }

    private static IContainer BodyCell(IContainer container)
    {
        return container
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Lighten2)
            .PaddingVertical(6)
            .PaddingHorizontal(8);
    }

    private static void BuildMultiColumnContent(IContainer container)
    {
        container.MultiColumn(multiColumn =>
        {
            multiColumn.Columns(2);
            multiColumn.Spacing(12);
            multiColumn.Content().Text(text =>
            {
                text.Span("Multi-column smoke content. ").Bold();

                for (var i = 0; i < 18; i++)
                    text.Span("QuestPDF lays out fluent document content while native PDF dependencies are loaded from the published output. ");
            });
        });
    }

    private static void BuildPaginationContent(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(8);
            column.Item().Text("Pagination workload").FontSize(16).Bold();

            for (var i = 1; i <= 36; i++)
            {
                var index = i;

                column.Item()
                    .Border(1)
                    .BorderColor(Colors.Grey.Lighten2)
                    .Padding(8)
                    .Row(row =>
                    {
                        row.ConstantItem(32)
                            .AlignCenter()
                            .AlignMiddle()
                            .Text(index.ToString("00"))
                            .SemiBold();

                        row.RelativeItem()
                            .Text("Repeated layout row used to force page breaks and validate final document structure after publish.");
                    });
            }
        });
    }
}
