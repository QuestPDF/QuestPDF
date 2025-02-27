using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class EnsureSpaceExamples
{
    [Test]
    public void Example()
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
                            column.Item().Text(Placeholders.LoremIpsum()).FontColor(Colors.Grey.Medium).Light();
                            column.Item().Height(20); 
                            
                            column.Item()
                                .EnsureSpace()
                                .Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.ConstantColumn(40);
                                        columns.RelativeColumn();
                                    });

                                    foreach (var i in Enumerable.Range(1, 12))
                                    {
                                        table.Cell().Text($"{i}.");
                                        table.Cell().Text(Placeholders.Sentence());
                                    }
                                });
                        });
                });
            })
            .GeneratePdf("ensure-space-enabled.pdf");
    }
}