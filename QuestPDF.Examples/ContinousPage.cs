using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples
{
    public class ContinuousPageDocument : IDocument
    {
        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(20);
                page.ContinuousSize(150);
                
                page.Header().Text("Header");
                
                page.Content().PaddingVertical(10).Border(1).Padding(10).Column(column =>
                {
                    foreach (var index in Enumerable.Range(1, 100))
                        column.Item().Text($"Line {index}").FontColor(Placeholders.Color());
                });
                
                page.Footer().Text("Footer");
            });
        }
    }
    
    public class ContinuousPageExamples
    {
        [Test]
        public void ContinuousPage()
        {
            var path = "example.pdf";
            new ContinuousPageDocument().GeneratePdf(path);
            Process.Start("explorer", path);
        }
    }
}