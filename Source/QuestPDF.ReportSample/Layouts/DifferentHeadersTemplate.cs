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
            container.Background(Colors.Grey.Lighten3).Border(1).Column(column =>
            {
                column.Item().ShowOnce().Padding(5).AlignMiddle().Row(row =>
                {
                    row.RelativeItem(2).AlignMiddle().Text("PRIMARY HEADER").FontColor(Colors.Grey.Darken3).FontSize(30).Bold();
                    row.RelativeItem(1).AlignRight().MinimalBox().AlignMiddle().Background(Colors.Blue.Darken2).Padding(30);
                });
                column.Item().SkipOnce().Padding(5).Row(row =>
                {
                    row.RelativeItem(2).Text("SECONDARY HEADER").FontColor(Colors.Grey.Darken3).FontSize(30).Bold();
                    row.RelativeItem(1).AlignRight().MinimalBox().Background(Colors.Blue.Lighten4).Padding(15);
                });
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().PaddingVertical(80).Text("First");
                column.Item().PageBreak();
                column.Item().PaddingVertical(80).Text("Second");
                column.Item().PageBreak();
                column.Item().PaddingVertical(80).Text("Third");
                column.Item().PageBreak();
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.Background(Colors.Grey.Lighten3).Column(column =>
            {
                column.Item().ShowOnce().Background(Colors.Grey.Lighten3).Row(row =>
                {
                    row.RelativeItem().Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                    row.RelativeItem().AlignRight().Text("Footer for header");
                });

                column.Item().SkipOnce().Background(Colors.Grey.Lighten3).Row(row =>
                {
                    row.RelativeItem().Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                    row.RelativeItem().AlignRight().Text("Footer for every page except header");
                });
            });
        }
    }
}