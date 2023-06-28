using System.Linq;
using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples
{
    [TestFixture]
    public class MergedDocumentExamples
    {
        [Test]
        public void Merge_ContinuousPageNumbers()
        {
            var mergedDocument = Document
                .Merge(
                    GenerateReport("Short Document 1", 5),
                    GenerateReport("Medium Document 2", 10),
                    GenerateReport("Long Document 3", 15))
                .UseContinuousPageNumbers();

            RenderingTest
                .Create()
                .ProducePdf()
                .ShowResults()
                .Render(mergedDocument);
        }

        [Test]
        public void Merge_SeparatePageNumbers()
        {
            var mergedDocument = Document
                .Merge(
                    GenerateReport("Short Document 1", 5),
                    GenerateReport("Medium Document 2", 10),
                    GenerateReport("Long Document 3", 15))
                .UseOriginalPageNumbers();

            RenderingTest
                .Create()
                .ProducePdf()
                .ShowResults()
                .Render(mergedDocument);
        }

        private static Document GenerateReport(string title, int itemsCount)
        {
            return Document.Create(document =>
            {
                document.Page(page =>
                {
                    page.Margin(0.5f, Unit.Inch);
                    page.Size(PageSizes.A5);
                    
                    page.Header()
                        .Text(title)
                        .Bold()
                        .FontSize(24)
                        .FontColor(Colors.Blue.Accent2);
                    
                    page.Content()
                        .PaddingVertical(20)
                        .Column(column =>
                        {
                            column.Spacing(10);

                            foreach (var i in Enumerable.Range(1, itemsCount))
                            {
                                column
                                    .Item()
                                    .Width(200)
                                    .Height(50)
                                    .Background(Colors.Grey.Lighten3)
                                    .AlignMiddle()
                                    .AlignCenter()
                                    .Text($"Item {i}")
                                    .FontSize(16);
                            }
                        });
                    
                    page.Footer()
                        .AlignCenter()
                        .PaddingVertical(20)
                        .Text(text =>
                        {
                            text.DefaultTextStyle(TextStyle.Default.FontSize(16));
                            
                            text.CurrentPageNumber();
                            text.Span(" / ");
                            text.TotalPages();
                        });
                });
            });
        }
    }
}