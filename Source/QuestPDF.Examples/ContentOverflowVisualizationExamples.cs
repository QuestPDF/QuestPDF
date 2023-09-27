using System.Linq;
using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples;

public class ContentOverflowVisualizationExamples
{
    [Test]
    public void DrawOverflow()
    {
        RenderingTest
            .Create()
            .ShowResults()
            .ProducePdf()
            .EnableDebugging()
            .RenderDocument(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(24);
                    page.DefaultTextStyle(TextStyle.Default.FontSize(16));

                    page.Header().Text("Document header").FontSize(24).Bold().FontColor(Colors.Blue.Accent2);

                    page.Content().PaddingVertical(24).Column(column =>
                    {
                        column.Spacing(16);

                        foreach (var size in Enumerable.Range(20, 20))
                            column.Item().Width(size * 10).Height(40).Background(Colors.Grey.Lighten3);
                        
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("Will it work?").FontSize(20);
                            row.RelativeItem().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Height(100).ShowEntire().Text(Placeholders.LoremIpsum()).FontSize(20);
                        });
                        
                        foreach (var size in Enumerable.Range(20, 20))
                            column.Item().Width(size * 10).Height(40).Background(Colors.Grey.Lighten3);
                    });
                    
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });
                });
            });
    }
    
    [Test]
    public void DrawOverflowRealExample()
    {
        var image = Placeholders.Image(400, 300);
 
        RenderingTest
            .Create()
            .ShowResults()
            .PageSize(PageSizes.A4)
            .ProducePdf()
            .EnableDebugging()
            .Render(container =>
            {
                container.Column(column =>
                {
                    foreach (var i in Enumerable.Range(0, 50))
                        column.Item().Height(30).Width(i * 5 + 100).Background(Placeholders.BackgroundColor());
                   
                    column.Item()
                        .Padding(24)

                        // constrain area to square 200 x 200
                        .Width(200)
                        .Height(200)
                        .Background(Colors.Grey.Lighten3)

                        // draw image that fits height (and therefore will overflow)
                        //.ContentOverflowDebugArea()
                        .Image(image)
                        .FitHeight();
                    
                    foreach (var i in Enumerable.Range(0, 50))
                        column.Item().Height(30).Width(i * 5 + 100).Background(Placeholders.BackgroundColor());
                });
            });
    }
    
    [Test]
    public void DrawOverflowSimpleExample()
    {
        var image = Placeholders.Image(400, 300);
        
        RenderingTest
            .Create()
            .ShowResults()
            .PageSize(PageSizes.A4)
            .EnableDebugging()
            .ProducePdf()
            .Render(container =>
            {
                container
                    .Padding(24)

                    // constrain area to square 200 x 200
                    .Width(200)
                    .Height(200)
                    .Background(Colors.Grey.Lighten3)

                    // draw image that fits height (and therefore will overflow)
                    //.ContentOverflowDebugArea()
                    .Image(image)
                    .FitHeight();
            });
    }
    
    [Test]
    public void DrawOverflowCases()
    {
        RenderingTest
            .Create()
            .ShowResults()
            .PageSize(PageSizes.A4)
            .EnableDebugging()
            .ProducePdf()
            .Render(container =>
            {
                container.Padding(24).Row(row =>
                {
                    row.Spacing(50);
                    
                    row.RelativeItem().ContentFromLeftToRight().Element(GenerateOverflowPatterns);
                    row.RelativeItem().ContentFromRightToLeft().Element(GenerateOverflowPatterns);
                });

                void GenerateOverflowPatterns(IContainer container)
                {
                    container.Column(column =>
                    {
                        column.Spacing(50);

                        column
                            .Item()
                            .Element(DrawTestcaseArea)
                            
                            .Width(50)
                            .Height(150)
                            
                            .Text("Test");
                        
                        column
                            .Item()
                            .Element(DrawTestcaseArea)
                        
                            .Width(150)
                            .Height(50)
                            
                            .Text("Test");
                    
                        column
                            .Item()
                            .Element(DrawTestcaseArea)

                            .Width(200)
                            .Height(150)
                            
                            .Text("Test");

                        IContainer DrawTestcaseArea(IContainer container)
                        {
                            return container
                                .Height(200)
                                .Background(Colors.Grey.Lighten4)

                                .Width(100)
                                .Height(100)
                                .Background(Colors.Grey.Lighten1);
                        }
                    });
                }
            });
    }
}