using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace QuestPDF.Examples
{
    [TestFixture]
    public class MergedDocumentSectionLinksTests
    {
        [Test]
        public void Merge_ContinuousPageNumbers()
        {
            var mergedDocument = Document.Merge(
                    CreateDocument("Document 1"),
                    CreateDocument("Document 2"),
                    CreateDocument("Document 3"))
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
            var mergedDocument = Document.Merge(
                    CreateDocument("Document 1"),
                    CreateDocument("Document 2"),
                    CreateDocument("Document 3"))
                .UseOriginalPageNumbers();

            RenderingTest
                .Create()
                .ProducePdf()
                .ShowResults()
                .Render(mergedDocument);
        }

        private static Document CreateDocument(string content)
        {
            return Document.Create(document =>
            {
                document.Page(page =>
                {
                    page.Content()
                        .AlignMiddle()
                        .AlignCenter()
                        .Column(column =>
                        {
                            column.Item().Text(content).FontSize(40);
                            
                            column.Item().PageBreak();
                            
                            column.Item().Text(content).FontSize(40);
                            column.Item().AlignCenter().SectionLink("next").Text("Next page").FontSize(16).Underline().FontColor(Colors.Blue.Medium);
                            
                            column.Item().PageBreak();
                            
                            column.Item().Text(content).FontSize(40);
                            column.Item().AlignCenter().Section("next").Text("Next page").FontSize(16).FontColor(Colors.Green.Medium);
                        });
                    
                    page.Footer()
                        .AlignCenter()
                        .PaddingVertical(20)
                        .Text(text =>
                        {
                            text.CurrentPageNumber();
                            text.Span(" / ");
                            text.TotalPages();
                        });
                });
            });
        }
    }
}