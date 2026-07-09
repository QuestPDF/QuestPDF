using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class EnsureSpaceExamples
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
                                .EnsureSpace(100)
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
                                        table.Cell().ShowEntire().Text(Placeholders.Sentence());
                                    }
                                });
                        });
                });
            })
            .GeneratePdf("ensure-space-enabled.pdf");
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
            .GeneratePdf("ensure-space-disabled.pdf");
    }
}