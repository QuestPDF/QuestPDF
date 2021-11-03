using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.ReportSample.Layouts
{
    public class DifferentHeadersTemplate : IDocument
    {
        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Margin(40);

                    page.Size(PageSizes.A4);

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().Element(ComposeFooter);
                });
        }

        private void ComposeHeader(IContainer container)
        {
            container.Background(Colors.Grey.Lighten3).Border(1).Stack(stack =>
            {
                stack.Item().ShowOnce().Padding(5).AlignMiddle().Row(row =>
                {
                    row.RelativeColumn(2).AlignMiddle().Text("PRIMARY HEADER", TextStyle.Default.Color(Colors.Grey.Darken3).Size(30).Bold());
                    row.RelativeColumn(1).AlignRight().Box().AlignMiddle().Background(Colors.Blue.Darken2).Padding(30);
                });
                stack.Item().SkipOnce().Padding(5).Row(row =>
                {
                    row.RelativeColumn(2).Text("SECONDARY HEADER", TextStyle.Default.Color(Colors.Grey.Darken3).Size(30).Bold());
                    row.RelativeColumn(1).AlignRight().Box().Background(Colors.Blue.Lighten4).Padding(15);
                });
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.Stack(stack =>
            {
                stack.Item().PaddingVertical(80).Text("First");
                stack.Item().PageBreak();
                stack.Item().PaddingVertical(80).Text("Second");
                stack.Item().PageBreak();
                stack.Item().PaddingVertical(80).Text("Third");
                stack.Item().PageBreak();
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.Background(Colors.Grey.Lighten3).Stack(stack =>
            {
                stack.Item().ShowOnce().Background(Colors.Grey.Lighten3).Row(row =>
                {
                    row.RelativeColumn().Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                    row.RelativeColumn().AlignRight().Text("Footer for header");
                });

                stack.Item().SkipOnce().Background(Colors.Grey.Lighten3).Row(row =>
                {
                    row.RelativeColumn().Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                    row.RelativeColumn().AlignRight().Text("Footer for every page except header");
                });
            });
        }
    }
}