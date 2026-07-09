using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Tests.Shared;

public static class PdfSmokeTests
{
    private const string InlineSvg = "<svg width=\"120\" height=\"64\" viewBox=\"0 0 120 64\" xmlns=\"http://www.w3.org/2000/svg\"><rect x=\"1\" y=\"1\" width=\"118\" height=\"62\" fill=\"#F8FAFC\" stroke=\"#94A3B8\"/><path d=\"M24 38l12-16 10 12 8-7 18 21H18z\" fill=\"#2563EB\"/><circle cx=\"86\" cy=\"24\" r=\"9\" fill=\"#10B981\"/></svg>";
    private const string LatoFontFamily = "Lato";
    private const string ArabicFontFamily = "Noto Sans Arabic";

    public const string SkiaPdfFileName = "skia.pdf";
    public const string QpdfPdfFileName = "qpdf.pdf";
    public const string XpsFileName = "skia.xps";

    private static readonly object ConfigurationLock = new();
    private static bool QuestPdfConfigured;

    public static void GenerateAllSupportedFiles(string outputDirectory)
    {
        GeneratePdfFiles(outputDirectory);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            GenerateXpsFile(outputDirectory);
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
            GeneratePdfFiles(tempDirectory);
            return File.ReadAllBytes(Path.Combine(tempDirectory, QpdfPdfFileName));
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

    public static void GeneratePdfFiles(string outputDirectory)
    {
        ConfigureQuestPdf();
        Directory.CreateDirectory(outputDirectory);

        var skiaPdfPath = Path.Combine(outputDirectory, SkiaPdfFileName);
        var qpdfPdfPath = Path.Combine(outputDirectory, QpdfPdfFileName);
        var workingDirectory = Path.Combine(Path.GetTempPath(), "questpdf-integration-work-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workingDirectory);

        try
        {
            var basePdfPath = Path.Combine(workingDirectory, "base.pdf");
            var appendixPdfPath = Path.Combine(workingDirectory, "appendix.pdf");

            CreateDocument().GeneratePdf(skiaPdfPath);
            PdfValidator.ValidateFile(skiaPdfPath);

            File.Copy(skiaPdfPath, basePdfPath);
            CreateAppendixDocument().GeneratePdf(appendixPdfPath);

            DocumentOperation
                .LoadFile(basePdfPath)
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
    }

    public static string GenerateXpsFile(string outputDirectory)
    {
        ConfigureQuestPdf();
        Directory.CreateDirectory(outputDirectory);

        var filePath = Path.Combine(outputDirectory, XpsFileName);
        CreateDocument().GenerateXps(filePath);
        PdfValidator.ValidateXpsFile(filePath);

        return filePath;
    }

    private static void ConfigureQuestPdf()
    {
        lock (ConfigurationLock)
        {
            if (QuestPdfConfigured)
                return;

            Trace.Listeners.Add(new TextWriterTraceListener(Console.Error));
            Trace.AutoFlush = true;

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
        return Path.Combine(AppContext.BaseDirectory, "Resources", Path.Combine(segments));
    }

    private static IDocument CreateDocument()
    {
        return Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.MarginHorizontal(42);
                page.MarginVertical(34);
                page.DefaultTextStyle(x => x.FontFamily(LatoFontFamily).FontSize(9).FontColor(Colors.Grey.Darken3));
                page.Header().Element(BuildDocumentHeader);

                page.Content()
                    .PaddingTop(16)
                    .Column(column =>
                    {
                        column.Spacing(14);
                        column.Item().Element(BuildPurposeSection);
                        column.Item().Element(container => BuildSection(container, "Test scenarios", "Environment-sensitive behaviors that must work after publish/deployment.", BuildFeatureTable));
                        column.Item().Row(row =>
                        {
                            row.Spacing(18);
                            row.RelativeItem().Element(container => BuildSection(container, "Font resources", "Expected: all text uses bundled fonts only; glyph validation must pass.", BuildFontPanel));
                            row.RelativeItem().Element(container => BuildSection(container, "Image and SVG resources", "Expected: all resources resolve from the application output.", BuildResourcePanel));
                        });
                        column.Item().Element(container => BuildSection(container, "Document operations", "Expected: QuestPDF generates the base file, qpdf merges an appendix, attaches a resource file, and linearizes the final PDF.", BuildDocumentOperations));
                        column.Item().PageBreak();
                        column.Item().Element(BuildPaginationContent);
                    });

                page.Footer()
                    .BorderTop(1)
                    .BorderColor(Colors.Grey.Lighten2)
                    .PaddingTop(8)
                    .Text(text =>
                    {
                        text.Span("QuestPDF integration smoke test").FontColor(Colors.Grey.Darken1);
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
                    .Column(column =>
                    {
                        column.Spacing(10);
                        column.Item().Text("qpdf appendix").FontSize(15).SemiBold().FontColor(Colors.Grey.Darken4);
                        column.Item().Text("This page is generated as a separate PDF and then merged into qpdf.pdf.");
                        column.Item().Text("Expected result: if this appendix is present, the qpdf native library was loaded and the merge operation completed.");
                        column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                        column.Item().Text("The qpdf output also contains a bundled text attachment and is saved through the linearization path.").FontColor(Colors.Grey.Darken2);
                    }));
            });
        });
    }

    private static void BuildDocumentHeader(IContainer container)
    {
        container
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Lighten2)
            .PaddingBottom(10)
            .Row(row =>
            {
                row.ConstantItem(38)
                    .Height(38)
                    .Image(ResourcePath("Images", "questpdf-logo.png"))
                    .FitArea();

                row.RelativeItem()
                    .PaddingLeft(12)
                    .Column(column =>
                    {
                        column.Spacing(4);
                        column.Item().Text("QuestPDF integration smoke test").FontSize(16).SemiBold().FontColor(Colors.Grey.Darken4);
                        column.Item().Text("Native libraries, packaged resources, fonts, and document operations.").FontSize(9).FontColor(Colors.Grey.Darken1);
                    });
            });
    }

    private static void BuildPurposeSection(IContainer container)
    {
        container
            .Column(column =>
            {
                column.Spacing(6);
                column.Item().Text("Purpose").FontSize(11).SemiBold().FontColor(Colors.Grey.Darken4);
                column.Item().Text("This file is produced by the deployed test application. It is intentionally small and predictable, but it touches the parts of QuestPDF that are most likely to fail when packaging or runtime probing is wrong.");
                column.Item().Text(text =>
                {
                    text.Span("Expected result: ").SemiBold();
                    text.Span("PDF generation succeeds without system fonts, without external resource paths, and with native dependencies resolved from the application output.");
                });
            });
    }

    private static void BuildSection(IContainer container, string title, string subtitle, Action<IContainer> content)
    {
        container
            .BorderTop(1)
            .BorderColor(Colors.Grey.Lighten2)
            .PaddingTop(10)
            .Column(column =>
            {
                column.Spacing(8);
                column.Item().Text(title).FontSize(11).SemiBold().FontColor(Colors.Grey.Darken4);
                column.Item().Text(subtitle).FontSize(8).FontColor(Colors.Grey.Darken1);
                column.Item().Element(content);
            });
    }

    private static void BuildFeatureTable(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(82);
                columns.RelativeColumn();
                columns.RelativeColumn();
            });

            table.Header(header =>
            {
                header.Cell().Element(HeaderCell).Text("Area");
                header.Cell().Element(HeaderCell).Text("Test input");
                header.Cell().Element(HeaderCell).Text("Expected behavior");
            });

            AddRow(table, "Skia", "Generate the base PDF and, on Windows, an XPS file.", "Native Skia assets are loaded from the deployed application.");
            AddRow(table, "qpdf", "Merge a generated appendix, add a bundled attachment, and save qpdf.pdf.", "Native qpdf assets are loaded and document post-processing succeeds.");
            AddRow(table, "Fonts", "Disable environment fonts and register bundled Lato plus Noto Sans Arabic.", "Glyph checks pass and text renders without depending on the host system.");
            AddRow(table, "Resources", "Load PNG, SVG file, inline SVG, and SVG referencing a font file.", "All resources resolve from the packaged output directory.");
            AddRow(table, "Layout", "Render a table, page footer counters, multi-column text, and repeated rows.", "The document remains valid across page breaks and common layout primitives.");
        });
    }

    private static void BuildFontPanel(IContainer container)
    {
        container
            .Column(column =>
            {
                column.Spacing(7);
                column.Item().Text("Lato regular").FontSize(8).SemiBold().FontColor(Colors.Grey.Darken2);
                column.Item().Text("Zażółć gęślą jaźń, 0123456789.").FontFamily(LatoFontFamily).FontSize(12);
                column.Item().Text("Lato bold italic").FontSize(8).SemiBold().FontColor(Colors.Grey.Darken2);
                column.Item().Text("Expected: deterministic bundled font selection.").FontFamily(LatoFontFamily).Bold().Italic();
                column.Item().Text("Noto Sans Arabic").FontSize(8).SemiBold().FontColor(Colors.Grey.Darken2);
                column.Item().ContentFromRightToLeft().Text("مرحبا بالعالم").FontFamily(ArabicFontFamily).FontSize(14);
            });
    }

    private static void BuildResourcePanel(IContainer container)
    {
        container
            .Column(column =>
            {
                column.Spacing(8);

                column.Item().Text("Raster image and SVG file").FontSize(8).SemiBold().FontColor(Colors.Grey.Darken2);
                column.Item().Height(46).Row(row =>
                {
                    row.Spacing(8);
                    row.RelativeItem().Padding(3).Image(ResourcePath("Images", "questpdf-logo.png")).FitArea();
                    row.RelativeItem().Padding(3).Svg(ResourcePath("Svg", "status-badge.svg")).FitArea();
                });

                column.Item().Text("Inline SVG").FontSize(8).SemiBold().FontColor(Colors.Grey.Darken2);
                column.Item().Height(38).Padding(3).Svg(InlineSvg).FitArea();

                column.Item().Text("SVG with font path").FontSize(8).SemiBold().FontColor(Colors.Grey.Darken2);
                column.Item().Height(48).Padding(3).Svg(ResourcePath("Svg", "font-resource.svg")).FitArea();
            });
    }

    private static IContainer HeaderCell(IContainer container)
    {
        return container
            .Background(Colors.Grey.Lighten4)
            .DefaultTextStyle(x => x.FontColor(Colors.Grey.Darken3).SemiBold())
            .PaddingVertical(7)
            .PaddingHorizontal(8);
    }

    private static void AddRow(TableDescriptor table, string area, string scenario, string expected)
    {
        table.Cell().Element(BodyCell).Text(area);
        table.Cell().Element(BodyCell).Text(scenario);
        table.Cell().Element(BodyCell).Text(expected);
    }

    private static IContainer BodyCell(IContainer container)
    {
        return container
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Lighten2)
            .PaddingVertical(7)
            .PaddingHorizontal(8);
    }

    private static void BuildDocumentOperations(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(7);
            column.Item().Text(text =>
            {
                text.Span("Generated files: ").SemiBold();
                text.Span(SkiaPdfFileName + " is produced directly by QuestPDF/Skia. ");
                text.Span(QpdfPdfFileName + " is produced by qpdf after merge, attachment, and linearization.");
            });

            column.Item().Text(text =>
            {
                text.Span("Attachment: ").SemiBold();
                text.Span("integration-note.txt is embedded from the bundled Resources directory.");
            });

            column.Item().MultiColumn(multiColumn =>
            {
                multiColumn.Columns(2);
                multiColumn.Spacing(14);
                multiColumn.Content().Text(text =>
                {
                    text.Span("Layout coverage: ").SemiBold();

                    for (var i = 0; i < 8; i++)
                        text.Span("The deployed application lays out fluent document content while native dependencies and packaged resources are resolved from application output. ");
                });
            });
        });
    }

    private static void BuildPaginationContent(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(10);
            column.Item().Text("Pagination workload").FontSize(15).SemiBold().FontColor(Colors.Grey.Darken4);
            column.Item().Text("Expected: page breaks, repeated elements, and footer counters are preserved in both generated PDF variants.").FontColor(Colors.Grey.Darken2);

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
                            .Background(Colors.Grey.Lighten4)
                            .AlignCenter()
                            .AlignMiddle()
                            .Text(index.ToString("00"))
                            .SemiBold()
                            .FontColor(Colors.Grey.Darken3);

                        row.RelativeItem()
                            .PaddingLeft(10)
                            .Text("Repeated layout row used to force page breaks and validate final document structure after publish.");
                    });
            }
        });
    }
}
