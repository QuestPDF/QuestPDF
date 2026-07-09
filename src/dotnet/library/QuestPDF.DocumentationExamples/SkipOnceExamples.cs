using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class SkipOnceExamples
{
    [Test]
    public void Example()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(500, 500);
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content()
                        .Column(column =>
                        {
                            var terms = new[]
                            {
                                ("Repository", "A centralized storage location for source code and related files, typically managed using version control systems like Git. Repositories allow multiple developers to collaborate on projects, track changes, and maintain version history."),
                                ("Version Control", "A system that tracks changes to code over time, enabling developers to collaborate efficiently, revert to previous versions, and maintain a structured development workflow. Popular version control tools include Git, Mercurial, and Subversion."),
                                ("Abstraction", "A programming concept that hides complex implementation details and exposes only the necessary parts. Abstraction helps simplify code and allows developers to focus on high-level design rather than low-level implementation details."),
                                ("Namespace", "A container that groups related identifiers, such as variables, functions, and classes, to prevent naming conflicts in a program. Namespaces are commonly used in large projects to organize code efficiently."),
                            };
                            
                            column.Spacing(15);
                            
                            foreach (var term in terms)
                            {
                                column.Item().Decoration(decoration =>
                                {
                                    decoration.Before()
                                        .DefaultTextStyle(x => x.FontSize(24).Bold().FontColor(Colors.Blue.Darken2))
                                        .Column(innerColumn =>
                                        {
                                            innerColumn.Item().ShowOnce().Text(term.Item1);
                                            
                                            innerColumn.Item().SkipOnce().Text(text =>
                                            {
                                                text.Span(term.Item1);
                                                text.Span(" (continued)").Light().Italic();
                                            });
                                        });

                                    decoration.Content().Text(term.Item2);
                                });
                            }
                        });
                });
            })
            .GenerateImages(x => $"skip-once-{x}.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
}