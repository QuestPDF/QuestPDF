using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples.CodePatterns;

public class CodePatternDocumentStructureExample
{
    [Test]
    public void Example()
    {
        var content = GenerateReport();
        File.WriteAllBytes("code-pattern-document-structure.pdf", content);
    }

    public byte[] GenerateReport()
    {
        return Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A5);
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content()
                        .PaddingBottom(15)
                        .Column(column =>
                        {
                            column.Item().Element(ReportTitle);
                            column.Item().PageBreak();
                            column.Item().Element(RedSection);
                            column.Item().PageBreak();
                            column.Item().Element(GreenSection);
                            column.Item().PageBreak();
                            column.Item().Element(BlueSection);
                        });

                    page.Footer().AlignCenter().Text(text => text.CurrentPageNumber());
                });
            })
            .GeneratePdf();
    }

    private void ReportTitle(IContainer container)
    {
        container.Extend()
            .AlignCenter()
            .AlignMiddle()
            .Text("Multi-section report")
            .FontSize(48)
            .Bold();
    }
    
    private void RedSection(IContainer container)
    {
        container.Grid(grid =>
        {
            grid.Columns(3);
            grid.Spacing(15);
            
            grid.Item(3 ).Text("Red section")
                .FontColor(Colors.Red.Darken2).FontSize(32).Bold();

            grid.Item(3).Text(Placeholders.Paragraph()).Light();

            foreach (var i in Enumerable.Range(0, 6))
                grid.Item().AspectRatio(4 / 3f).Background(Colors.Red.Lighten4);
        });
    }
    
    private void GreenSection(IContainer container)
    {
        container.Grid(grid =>
        {
            grid.Columns(3);
            grid.Spacing(15);
            
            grid.Item(3).Text("Green section")
                .FontColor(Colors.Green.Darken2).FontSize(32).Bold();

            grid.Item(3).Text(Placeholders.Paragraph()).Light();

            foreach (var i in Enumerable.Range(0, 12))
                grid.Item().AspectRatio(4 / 3f).Background(Colors.Green.Lighten4);
        });
    }
    
    private void BlueSection(IContainer container)
    {
        container.Grid(grid =>
        {
            grid.Columns(3);
            grid.Spacing(15);
            
            grid.Item(3).Text("Blue section")
                .FontColor(Colors.Blue.Darken2).FontSize(32).Bold();

            grid.Item(3).Text(Placeholders.Paragraph()).Light();

            foreach (var i in Enumerable.Range(0, 18))
                grid.Item().AspectRatio(4 / 3f).Background(Colors.Blue.Lighten4);
        });
    }
}