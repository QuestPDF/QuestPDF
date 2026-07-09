using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class ShowEntireExamples
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
                        .Decoration(decoration =>
                        {
                            var terms = new[]
                            {
                                ("Function", "A reusable block of code designed to perform a specific task. Functions take input parameters, process them, and return results, making code modular, readable, and maintainable. They are an essential component of all programming languages."),
                                ("Recursion", "A programming technique where a function calls itself in order to solve a problem by breaking it down into smaller, similar subproblems. Recursion is often used for complex algorithms, such as searching, sorting, and tree traversal."),
                                ("Framework", "A pre-built collection of code, tools, and best practices that provides a structured foundation for developing software. Frameworks simplify development by handling common functionalities, such as database access, user authentication, and UI rendering."),
                                ("Package", "A self-contained collection of code, typically consisting of functions, classes, and modules, that provides specific functionality. Packages help organize large projects and allow developers to reuse and distribute their code easily."),
                            };
                            
                            decoration.Before().Text("Terms and their definitions:").FontSize(24).Bold().Underline();
                            
                            decoration.Content().PaddingTop(15).Column(column =>
                            {
                                column.Spacing(15);
                                
                                foreach (var term in terms)
                                {
                                    column.Item()
                                        .ShowEntire()
                                        .Text(text =>
                                        {
                                            text.Span(term.Item1).Bold().FontColor(Colors.Blue.Darken2);
                                            text.Span($" - {term.Item2}");
                                        });
                                }
                            });
                        });
                });
            })
            .GenerateImages(x => $"show-entire-with-{x}.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
}