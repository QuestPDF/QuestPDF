using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class HyperlinkExamples
{
    [Test]
    public void ElementExample()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.ContinuousSize(400);
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(25);
                            
                            column.Item()
                                .Text("Clicking the NuGet logo will redirect you to the NuGet website.");

                            column.Item()
                                .Width(150)
                                .Hyperlink("https://www.nuget.org/")
                                .Svg("Resources/nuget-logo.svg");
                        });
                });
            })
            .GeneratePdf("hyperlink-element.pdf");
    }
    
    [Test]
    public void InsideTextExample()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.ContinuousSize(300);
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content()
                        .Text(text =>
                        {
                            text.Span("Click ");
                            text.Hyperlink("here", "https://www.nuget.org/").Underline().FontColor(Colors.Blue.Darken2);
                            text.Span(" to visit the official NuGet website.");
                        });
                });
            })
            .GeneratePdf("hyperlink-text.pdf");
    }
}