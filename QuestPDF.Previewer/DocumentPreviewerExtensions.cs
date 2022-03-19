using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Previewer;

public static class DocumentPreviewerExtensions
{
    public static void ShowInPreviewer(this IDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        var builder = WebApplication.CreateBuilder();
        var app = builder.Build();
        
        var pdfDocumentCache = GeneratePdf(document);
        var refreshFlag = false;

        static byte[] GenerateDocumentAboutException(Exception exception)
        {
            return Document.Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Inch);
                    page.PageColor(Colors.Red.Lighten4);
                    page.DefaultTextStyle(x => x.FontSize(16));

                    page.Header()
                        .BorderBottom(2)
                        .BorderColor(Colors.Red.Medium)
                        .PaddingBottom(5)
                        .Text("Ooops! Something went wrong...").FontSize(28).FontColor(Colors.Red.Medium).Bold();

                    page.Content().PaddingVertical(20).Column(column =>
                    {
                        var currentException = exception;

                        while (currentException != null)
                        {
                            column.Item().Text(exception.GetType().Name).FontSize(20).SemiBold();
                            column.Item().Text(exception.Message).FontSize(14);
                            column.Item().PaddingTop(10).Text(exception.StackTrace).FontSize(10).Light();

                            currentException = currentException.InnerException;

                            if (currentException != null)
                                column.Item().PaddingVertical(15).LineHorizontal(2).LineColor(Colors.Red.Medium);
                        }
                    });
                });
            }).GeneratePdf();
        }

        static byte[] GeneratePdf(IDocument document)
        {
            try
            {
                return document.GeneratePdf();
            }
            catch(Exception exception)
            {
                return GenerateDocumentAboutException(exception);
            }
        }

        HotReloadManager.OnApplicationChanged += () =>
        {
            pdfDocumentCache = GeneratePdf(document);
            refreshFlag = true;
        };
  
        app.MapGet("/", () =>
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "QuestPDF.Previewer.index.html";
        
            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);
            var result = reader.ReadToEnd();
            return Results.Content(result, "text/html");
        });
        
        app.MapGet("/render", () =>
        {
            refreshFlag = false;
            return Results.File(pdfDocumentCache, "application/pdf");
        });
        
        app.MapGet("/listen", async () =>
        {
            foreach (var i in Enumerable.Range(0, 1000))
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100));

                if (!refreshFlag)
                    continue;

                return Results.Text("true");
            }

            return Results.Text("false");
        });

        app.Lifetime.ApplicationStarted.Register(() =>
        {
            var openBrowserProcess = new Process()
            {
                StartInfo = new()
                {
                    UseShellExecute = true,
                    FileName = app.Urls.First()
                }
            };

            openBrowserProcess.Start();
        });

        app.Run();
    }
}