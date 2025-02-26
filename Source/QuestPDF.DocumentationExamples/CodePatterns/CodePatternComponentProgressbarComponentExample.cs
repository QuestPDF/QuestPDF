
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples.CodePatterns;

public class CodePatternComponentProgressbarComponentExample
{
    [Test]
    public void Example()
    {
        var content = GenerateReport();
        File.WriteAllBytes("code-pattern-dynamic-component-progressbar.pdf", content);
    }
    
    public byte[] GenerateReport()
    {
        return Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(50);
                    page.DefaultTextStyle(x => x.FontSize(20));

                    page.Header().Column(column =>
                    {
                        column.Item()
                            .Text("MyBrick Set")
                            .FontSize(48).FontColor(Colors.Blue.Darken2).Bold();
                          
                        column.Item()
                            .Text("Building Instruction")
                            .FontSize(24);
                        
                        column.Item().Height(15);
                        
                        column.Item().Dynamic(new PageProgressbarComponent());
                    });
                        
                    page.Content().PaddingVertical(25).Column(column =>
                    {
                        column.Spacing(25);
                        
                        foreach (var i in Enumerable.Range(1, 30))
                        {
                            column.Item()
                                .Background(Colors.Grey.Lighten3)
                                .Height(Random.Shared.Next(4, 8) * 25)
                                .AlignCenter()
                                .AlignMiddle()
                                .Text($"Step {i}");
                        }
                    });

                    page.Footer().Dynamic(new PageNumberSideComponent());
                });
            })
            .GeneratePdf();
    }
}

public class PageProgressbarComponent : IDynamicComponent
{
    public DynamicComponentComposeResult Compose(DynamicContext context)
    {
        var content = context.CreateElement(element =>
        {
            var width = context.AvailableSize.Width * context.PageNumber / context.TotalPages;
                
            element
                .Background(Colors.Blue.Lighten3)
                .Height(5)
                .Width(width)
                .Background(Colors.Blue.Darken2);
        });

        return new DynamicComponentComposeResult
        {
            Content = content,
            HasMoreContent = false
        };
    }
}

public class PageNumberSideComponent : IDynamicComponent
{
    public DynamicComponentComposeResult Compose(DynamicContext context)
    {
        var content = context.CreateElement(element =>
        {
            element
                .Element(x => context.PageNumber % 2 == 0 ? x.AlignRight() : x.AlignLeft())
                .Text(text =>
                {
                    text.Span("Page ");
                    text.CurrentPageNumber();
                });
        });

        return new DynamicComponentComposeResult
        {
            Content = content,
            HasMoreContent = false
        };
    }
}