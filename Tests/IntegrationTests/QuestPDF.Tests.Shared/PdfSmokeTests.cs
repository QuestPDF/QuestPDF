using System;
using System.Diagnostics;
using System.IO;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Tests.Shared;

public sealed class PdfSmokeTestOutput
{
    public PdfSmokeTestOutput(string skiaPdfPath, string qpdfPdfPath)
    {
        SkiaPdfPath = skiaPdfPath;
        QpdfPdfPath = qpdfPdfPath;
    }

    public string SkiaPdfPath { get; }
    public string QpdfPdfPath { get; }
}

public static class PdfSmokeTests
{
    private const string InlineSvg = "<svg width=\"120\" height=\"80\" viewBox=\"0 0 120 80\" xmlns=\"http://www.w3.org/2000/svg\"><rect width=\"120\" height=\"80\" rx=\"10\" fill=\"#E3F2FD\"/><circle cx=\"34\" cy=\"40\" r=\"18\" fill=\"#1976D2\"/><path d=\"M62 25h38v8H62zm0 18h30v8H62zm0 18h22v8H62z\" fill=\"#0D47A1\"/></svg>";
    private const string LatoFontFamily = "Lato";
    private const string ArabicFontFamily = "Noto Sans Arabic";
    private const string DefaultFileName = "questpdf-integration-smoke.pdf";

    private static readonly object ConfigurationLock = new();
    private static bool TraceListenerConfigured;
    private static bool QuestPdfConfigured;

    public static byte[] GeneratePdfBytes()
    {
        return GenerateQpdfPdfBytes();
    }

    public static byte[] GenerateSkiaPdfBytes()
    {
        ConfigureQuestPdf();
        var pdf = CreateDocument().GeneratePdf();
        PdfValidator.ValidateBytes(pdf, "Skia PDF bytes");
        return pdf;
    }

    public static byte[] GenerateQpdfPdfBytes()
    {
        var tempDirectory = Path.Combine(Path.GetTempPath(), "questpdf-integration-" + Guid.NewGuid().ToString("N"));

        try
        {
            Directory.CreateDirectory(tempDirectory);
            var pdfPath = GeneratePdfFiles(tempDirectory).QpdfPdfPath;
            return File.ReadAllBytes(pdfPath);
        }
        finally
        {
            if (Directory.Exists(tempDirectory))
                Directory.Delete(tempDirectory, recursive: true);
        }
    }

    public static byte[] GenerateXpsBytes()
    {
        ConfigureQuestPdf();
        var xps = CreateDocument().GenerateXps();
        PdfValidator.ValidateXpsBytes(xps, "XPS bytes");
        return xps;
    }

    public static string GeneratePdfFile(string outputDirectory, string fileName = DefaultFileName)
    {
        return GeneratePdfFiles(outputDirectory, GetSkiaPdfFileName(fileName), fileName).QpdfPdfPath;
    }

    public static PdfSmokeTestOutput GeneratePdfFiles(
        string outputDirectory,
        string skiaPdfFileName = "questpdf-integration-smoke-skia.pdf",
        string qpdfPdfFileName = DefaultFileName)
    {
        ConfigureQuestPdf();
        Directory.CreateDirectory(outputDirectory);

        var skiaPdfPath = Path.Combine(outputDirectory, skiaPdfFileName);
        var qpdfPdfPath = Path.Combine(outputDirectory, qpdfPdfFileName);
        var workingDirectory = Path.Combine(Path.GetTempPath(), "questpdf-integration-work-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workingDirectory);

        try
        {
            var appendixPdfPath = Path.Combine(workingDirectory, "appendix.pdf");

            CreateDocument().GeneratePdf(skiaPdfPath);
            PdfValidator.ValidateFile(skiaPdfPath);

            CreateAppendixDocument().GeneratePdf(appendixPdfPath);

            DocumentOperation
                .LoadFile(skiaPdfPath)
                .MergeFile(appendixPdfPath)
                .AddAttachment(new DocumentOperation.DocumentAttachment
                {
                    FilePath = ResourcePath("Attachments", "integration-note.txt"),
                    AttachmentName = "integration-note.txt",
                    Description = "Bundled integration test attachment"
                })
                .Linearize()
                .Save(qpdfPdfPath);
        }
        finally
        {
            if (Directory.Exists(workingDirectory))
                Directory.Delete(workingDirectory, recursive: true);
        }

        PdfValidator.ValidateFile(qpdfPdfPath);

        return new PdfSmokeTestOutput(skiaPdfPath, qpdfPdfPath);
    }

    public static string GenerateXpsFile(string outputDirectory, string fileName = "questpdf-integration-smoke.xps")
    {
        ConfigureQuestPdf();
        Directory.CreateDirectory(outputDirectory);

        var filePath = Path.Combine(outputDirectory, fileName);
        CreateDocument().GenerateXps(filePath);
        PdfValidator.ValidateXpsFile(filePath);

        return filePath;
    }

    private static string GetSkiaPdfFileName(string qpdfPdfFileName)
    {
        var extension = Path.GetExtension(qpdfPdfFileName);

        if (string.IsNullOrWhiteSpace(extension))
            return qpdfPdfFileName + "-skia";

        return Path.GetFileNameWithoutExtension(qpdfPdfFileName) + "-skia" + extension;
    }

    private static void ConfigureQuestPdf()
    {
        lock (ConfigurationLock)
        {
            if (QuestPdfConfigured)
                return;

            if (!TraceListenerConfigured)
            {
                Trace.Listeners.Add(new ConsoleTraceListener(useErrorStream: true));
                Trace.AutoFlush = true;
                TraceListenerConfigured = true;
            }

            QuestPDF.Settings.License = LicenseType.Community;
            QuestPDF.Settings.UseEnvironmentFonts = false;
            QuestPDF.Settings.CheckIfAllTextGlyphsAreAvailable = true;
            QuestPDF.Settings.FontDiscoveryPaths.Clear();
            QuestPDF.Settings.FontDiscoveryPaths.Add(AppContext.BaseDirectory);

            EnsureResourceFilesExist();
            RegisterFont("Lato-Regular.ttf");
            RegisterFont("Lato-Bold.ttf");
            RegisterFont("Lato-Italic.ttf");
            RegisterFont("NotoSansArabic-Regular.ttf");

            QuestPdfConfigured = true;
        }
    }

    private static void RegisterFont(string fileName)
    {
        using var fontStream = File.OpenRead(ResourcePath("Fonts", fileName));
        FontManager.RegisterFont(fontStream);
    }

    private static void EnsureResourceFilesExist()
    {
        var resourceFiles = new[]
        {
            ResourcePath("Fonts", "Lato-Regular.ttf"),
            ResourcePath("Fonts", "Lato-Bold.ttf"),
            ResourcePath("Fonts", "Lato-Italic.ttf"),
            ResourcePath("Fonts", "NotoSansArabic-Regular.ttf"),
            ResourcePath("Images", "questpdf-logo.png"),
            ResourcePath("Svg", "status-badge.svg"),
            ResourcePath("Svg", "font-resource.svg"),
            ResourcePath("Attachments", "integration-note.txt")
        };

        foreach (var file in resourceFiles)
        {
            if (!File.Exists(file))
                throw new InvalidOperationException("Integration test resource file is missing: " + file);
        }
    }

    private static string ResourcePath(params string[] segments)
    {
        var paths = new string[segments.Length + 2];
        paths[0] = AppContext.BaseDirectory;
        paths[1] = "Resources";
        Array.Copy(segments, 0, paths, 2, segments.Length);
        return Path.Combine(paths);
    }

    private static IDocument CreateDocument()
    {
        return Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(36);
                page.DefaultTextStyle(x => x.FontFamily(LatoFontFamily).FontSize(9).FontColor(Colors.Grey.Darken4));
                page.Header().Element(BuildDocumentHeader);

                page.Content()
                    .PaddingTop(18)
                    .Column(column =>
                    {
                        column.Spacing(14);
                        column.Item().Element(BuildSummaryPanel);
                        column.Item().Element(BuildStatusStrip);
                        column.Item().Element(container => BuildSection(container, "Capability matrix", BuildFeatureTable));
                        column.Item().Row(row =>
                        {
                            row.Spacing(12);
                            row.RelativeItem().Element(container => BuildSection(container, "Typography", BuildFontPanel));
                            row.RelativeItem().Element(container => BuildSection(container, "Bundled resources", BuildResourcePanel));
                        });
                        column.Item().Element(BuildMultiColumnContent);
                        column.Item().PageBreak();
                        column.Item().Element(BuildPaginationContent);
                    });

                page.Footer()
                    .BorderTop(1)
                    .BorderColor(Colors.Grey.Lighten2)
                    .PaddingTop(8)
                    .Text(text =>
                    {
                        text.Span("QuestPDF integration report").FontColor(Colors.Grey.Darken1);
                        text.Span("  /  ").FontColor(Colors.Grey.Lighten1);
                        text.Span("Page ").FontColor(Colors.Grey.Darken1);
                        text.CurrentPageNumber();
                        text.Span(" of ").FontColor(Colors.Grey.Darken1);
                        text.TotalPages();
                    });
            });
        });
    }

    private static IDocument CreateAppendixDocument()
    {
        return Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(PageSizes.A5);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontFamily(LatoFontFamily).FontSize(10).FontColor(Colors.Grey.Darken4));

                page.Content().Element(container => container
                    .Border(1)
                    .BorderColor(Colors.Blue.Lighten3)
                    .Padding(16)
                    .Column(column =>
                    {
                        column.Spacing(12);
                        column.Item().Text("qpdf appendix").FontSize(18).Bold().FontColor(Colors.Blue.Darken4);
                        column.Item().Text("This page is generated separately, then merged into the final PDF with an attachment and linearization step.");
                        column.Item().Height(64).Svg(ResourcePath("Svg", "status-badge.svg")).FitArea();
                        column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                        column.Item().Text("If this page is visible in the merged artifact, the qpdf native library was loaded and executed successfully.").FontColor(Colors.Grey.Darken2);
                    }));
            });
        });
    }

    private static void BuildDocumentHeader(IContainer container)
    {
        container
            .Background(Colors.Blue.Darken4)
            .Padding(16)
            .DefaultTextStyle(x => x.FontColor(Colors.White))
            .Row(row =>
            {
                row.ConstantItem(54)
                    .Height(54)
                    .Background(Colors.White)
                    .Padding(6)
                    .Image(ResourcePath("Images", "questpdf-logo.png"))
                    .FitArea();

                row.RelativeItem()
                    .PaddingLeft(14)
                    .Column(column =>
                    {
                        column.Spacing(4);
                        column.Item().Text("QuestPDF Package Integration").FontSize(20).Bold();
                        column.Item().Text("Deployment verification report").FontSize(10).FontColor(Colors.Blue.Lighten4);
                        column.Item().Text("Native libraries, packaged resources, font discovery, and document operations").FontSize(8).FontColor(Colors.Blue.Lighten3);
                    });

                row.ConstantItem(112)
                    .Height(54)
                    .Svg(InlineSvg)
                    .FitArea();
            });
    }

    private static void BuildSummaryPanel(IContainer container)
    {
        container
            .BorderLeft(4)
            .BorderColor(Colors.Green.Medium)
            .Background(Colors.Grey.Lighten5)
            .PaddingVertical(12)
            .PaddingHorizontal(14)
            .Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Spacing(5);
                    column.Item().Text("All checks executed from packaged output").FontSize(13).SemiBold().FontColor(Colors.Grey.Darken4);
                    column.Item().Text("This document intentionally uses only bundled fonts and resources. It also forces Skia rendering, SVG parsing, image loading, qpdf post-processing, page counters, links, and pagination in one deployed workflow.");
                });

                row.ConstantItem(130)
                    .AlignMiddle()
                    .Hyperlink("https://www.questpdf.com")
                    .Text("questpdf.com")
                    .FontColor(Colors.Blue.Darken2)
                    .SemiBold();
            });
    }

    private static void BuildStatusStrip(IContainer container)
    {
        container.Row(row =>
        {
            row.Spacing(8);
            row.RelativeItem().Element(card => BuildMetricCard(card, "Skia", "PDF + XPS", Colors.Blue.Darken2, Colors.Blue.Lighten5));
            row.RelativeItem().Element(card => BuildMetricCard(card, "qpdf", "Merge + attach", Colors.Green.Darken2, Colors.Green.Lighten5));
            row.RelativeItem().Element(card => BuildMetricCard(card, "Fonts", "Local only", Colors.Amber.Darken3, Colors.Amber.Lighten5));
            row.RelativeItem().Element(card => BuildMetricCard(card, "Resources", "Files + SVG", Colors.Cyan.Darken3, Colors.Cyan.Lighten5));
        });
    }

    private static void BuildMetricCard(IContainer container, string label, string value, Color accent, Color background)
    {
        container
            .Background(background)
            .Border(1)
            .BorderColor(Colors.Grey.Lighten2)
            .Padding(10)
            .Column(column =>
            {
                column.Spacing(4);
                column.Item().Text(label.ToUpperInvariant()).FontSize(7).SemiBold().FontColor(accent);
                column.Item().Text(value).FontSize(11).SemiBold().FontColor(Colors.Grey.Darken4);
            });
    }

    private static void BuildSection(IContainer container, string title, Action<IContainer> content)
    {
        container
            .Border(1)
            .BorderColor(Colors.Grey.Lighten2)
            .Column(column =>
            {
                column.Item()
                    .Background(Colors.Grey.Lighten4)
                    .PaddingVertical(7)
                    .PaddingHorizontal(10)
                    .Text(title)
                    .FontSize(10)
                    .SemiBold()
                    .FontColor(Colors.Grey.Darken4);

                column.Item().Padding(10).Element(content);
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
                header.Cell().Element(HeaderCell).AlignCenter().Text("Status");
            });

            AddRow(table, "Text", "Multiple spans, weights, colors, links, counters, and pagination.", "PASS");
            AddRow(table, "Fonts", "Environment fonts disabled; bundled Lato and Noto Sans Arabic are registered.", "PASS");
            AddRow(table, "Images", "Raster image loaded from a packaged resource path.", "PASS");
            AddRow(table, "SVG", "Inline SVG, SVG from file, and SVG with a font resource are rendered.", "PASS");
            AddRow(table, "qpdf", "Final PDF is merged, attached, and linearized after generation.", "PASS");
        });
    }

    private static void BuildFontPanel(IContainer container)
    {
        container
            .Border(1)
            .BorderColor(Colors.Grey.Lighten2)
            .Padding(12)
            .Column(column =>
            {
                column.Spacing(8);
                column.Item().Text("Lato regular").FontSize(8).SemiBold().FontColor(Colors.Blue.Darken2);
                column.Item().Text("Zażółć gęślą jaźń, 0123456789.").FontFamily(LatoFontFamily).FontSize(12);
                column.Item().Text("Lato bold and italic").FontSize(8).SemiBold().FontColor(Colors.Blue.Darken2);
                column.Item().Text("Deterministic font selection.").FontFamily(LatoFontFamily).Bold().Italic();
                column.Item().Text("Noto Sans Arabic").FontSize(8).SemiBold().FontColor(Colors.Blue.Darken2);
                column.Item().ContentFromRightToLeft().Text("مرحبا بالعالم").FontFamily(ArabicFontFamily).FontSize(14);
            });
    }

    private static void BuildResourcePanel(IContainer container)
    {
        container
            .Column(column =>
            {
                column.Spacing(7);

                column.Item().Height(54).Row(row =>
                {
                    row.Spacing(8);
                    row.RelativeItem().Background(Colors.Grey.Lighten5).Padding(5).Image(ResourcePath("Images", "questpdf-logo.png")).FitArea();
                    row.RelativeItem().Background(Colors.Grey.Lighten5).Padding(5).Svg(ResourcePath("Svg", "status-badge.svg")).FitArea();
                });

                column.Item().Height(58).Background(Colors.Grey.Lighten5).Padding(5).Svg(ResourcePath("Svg", "font-resource.svg")).FitArea();
            });
    }

    private static IContainer HeaderCell(IContainer container)
    {
        return container
            .Background(Colors.Blue.Darken4)
            .DefaultTextStyle(x => x.FontColor(Colors.White).SemiBold())
            .PaddingVertical(7)
            .PaddingHorizontal(8);
    }

    private static void AddRow(TableDescriptor table, string area, string scenario, string status)
    {
        table.Cell().Element(BodyCell).Text(area);
        table.Cell().Element(BodyCell).Text(scenario);
        table.Cell().Element(BodyCell).AlignCenter().Text(status).FontColor(Colors.Green.Darken2).SemiBold();
    }

    private static IContainer BodyCell(IContainer container)
    {
        return container
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Lighten2)
            .PaddingVertical(7)
            .PaddingHorizontal(8);
    }

    private static void BuildMultiColumnContent(IContainer container)
    {
        container.Element(section => BuildSection(section, "Layout workload", content =>
            content.MultiColumn(multiColumn =>
            {
                multiColumn.Columns(2);
                multiColumn.Spacing(14);
                multiColumn.Content().Text(text =>
                {
                    text.Span("Multi-column composition. ").Bold().FontColor(Colors.Blue.Darken3);

                    for (var i = 0; i < 16; i++)
                        text.Span("The deployed application lays out fluent document content while native dependencies and packaged resources are resolved from application output. ");
                });
            })));
    }

    private static void BuildPaginationContent(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(10);
            column.Item().Text("Pagination workload").FontSize(17).Bold().FontColor(Colors.Blue.Darken4);
            column.Item().Text("Repeated rows force page breaks and verify that final document structure remains valid after native rendering and qpdf post-processing.").FontColor(Colors.Grey.Darken2);

            for (var i = 1; i <= 36; i++)
            {
                var index = i;

                column.Item()
                    .Border(1)
                    .BorderColor(Colors.Grey.Lighten2)
                    .Background(index % 2 == 0 ? Colors.Grey.Lighten5 : Colors.White)
                    .Padding(9)
                    .Row(row =>
                    {
                        row.ConstantItem(32)
                            .Background(Colors.Blue.Lighten5)
                            .AlignCenter()
                            .AlignMiddle()
                            .Text(index.ToString("00"))
                            .SemiBold()
                            .FontColor(Colors.Blue.Darken3);

                        row.RelativeItem()
                            .PaddingLeft(10)
                            .Text("Repeated layout row used to force page breaks and validate final document structure after publish.");
                    });
            }
        });
    }
}
