using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;

namespace QuestPDF.Examples
{
    [TestFixture]
    public class MergeDocumentsExamples
    {
        [Test]
        public void Merge_ContinousPageNumbers()
        {
            var mergedDocument = Document.Merge(
                CreateDocument("Document 1"),
                CreateDocument("Document 2"),
                CreateDocument("Document 3"))
                .ContinuousPageNumbers();

            RenderingTest
                .Create()
                .ProducePdf()
                .ShowResults()
                .Render(mergedDocument);
        }

        [Test]
        public void Merge_SeperatePageNumbers()
        {
            var mergedDocument = Document.Merge(
                CreateDocument("Document 1"),
                CreateDocument("Document 2"),
                CreateDocument("Document 3"))
                .SeperatePageNumbers();

            RenderingTest
                .Create()
                .ProducePdf()
                .ShowResults()
                .Render(mergedDocument);
        }


        private static Document CreateDocument(string content)
        {
            return Document.Create(d =>
            {
                d.Page(p =>
                {
                    p.Content().AlignMiddle().AlignCenter().Column(c =>
                    {
                        c.Item().Text(content).FontSize(40);
                        c.Item().PageBreak();
                        c.Item().Text(content).FontSize(40);
                    });

                    p.Footer().AlignCenter().PaddingVertical(20).Text(t =>
                    {
                        t.CurrentPageNumber();
                        t.Span(" / ");
                        t.TotalPages();
                    });
                });
            });
        }

    }
}
