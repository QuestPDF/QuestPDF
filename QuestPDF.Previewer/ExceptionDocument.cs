using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Previewer;

public class ExceptionDocument : IDocument
{
    private Exception Exception { get; }
    
    public ExceptionDocument(Exception exception)
    {
        Exception = exception;
    }
    
    public DocumentMetadata GetMetadata()
    {
        return DocumentMetadata.Default;
    }

    public void Compose(IDocumentContainer document)
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
                var currentException = Exception;

                while (currentException != null)
                {
                    column.Item().Text(currentException.GetType().Name).FontSize(20).SemiBold();
                    column.Item().Text(currentException.Message).FontSize(14);
                    column.Item().PaddingTop(10).Text(currentException.StackTrace).FontSize(10).Light();

                    currentException = currentException.InnerException;

                    if (currentException != null)
                        column.Item().PaddingVertical(15).LineHorizontal(2).LineColor(Colors.Red.Medium);
                }
            });
        });
    }
}