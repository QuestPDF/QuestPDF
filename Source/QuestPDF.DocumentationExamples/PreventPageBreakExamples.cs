using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace QuestPDF.DocumentationExamples;

public class PreventPageBreakExamples
{
    [Test]
    public void EnabledExample()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A5);
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(30);
                    
                    page.Content()
                        .Column(column =>
                        {
                            column.Item().Height(400).Background(Colors.Grey.Lighten3);
                            column.Item().Height(30);

                            column.Item()
                                .PreventPageBreak()
                                .Text(text =>
                                {
                                    text.ParagraphSpacing(15);
                                    
                                    text.Span("Optimizing Content Placement").Bold().FontColor(Colors.Blue.Darken2).FontSize(24);
                                    text.Span("\n");
                                    text.Span("By carefully determining where to place a page break, you can avoid awkward text separations and maintain readability. Thoughtful formatting improves the overall user experience, making complex topics easier to digest.");
                                });
                        }); 
                });
            })
            .GeneratePdf("prevent-page-break-enabled.pdf");
    }
    
    [Test]
    public void DisabledExample()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A5);
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(30);
                    
                    page.Content()
                        .Column(column =>
                        {
                            column.Item().Height(400).Background(Colors.Grey.Lighten3);
                            column.Item().Height(30);

                            column.Item()
                                .Text(text =>
                                {
                                    text.ParagraphSpacing(15);
                                    
                                    text.Span("Optimizing Content Placement").Bold().FontColor(Colors.Blue.Darken2).FontSize(24);
                                    text.Span("\n");
                                    text.Span("By carefully determining where to place a page break, you can avoid awkward text separations and maintain readability. Thoughtful formatting improves the overall user experience, making complex topics easier to digest.");
                                });
                        });
                });
            })
            .GeneratePdf("prevent-page-break-disabled.pdf");
    }
}