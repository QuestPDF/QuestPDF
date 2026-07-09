using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class RepeatExamples
{
    [Test]
    public void Example()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(600, 0));
                    page.MaxSize(new PageSize(600, 600));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content()
                        .Decoration(decoration =>
                        {
                            var terms = new[]
                            {
                                ("Algorithm", "A precise set of instructions that defines a process for solving a specific problem or performing a computation. Algorithms are the foundation of programming and are used to optimize tasks efficiently."),
                                ("Bug", "An error, flaw, or unintended behavior in a program that causes it to produce incorrect or unexpected results. Debugging is the process of identifying, analyzing, and fixing these issues to improve software reliability."),
                                ("Variable", "A named storage location in memory that holds a value, which can be modified during program execution. Variables make code dynamic and flexible by allowing data manipulation and retrieval."),
                                ("Compilation", "The process of transforming human-readable source code into machine code (binary instructions) that a computer can execute. This process is performed by a compiler and often includes syntax checks, optimizations, and linking dependencies.")
                            };
                            
                            decoration.Before().Text("Terms and their definitions:").Bold();
                            
                            decoration.Content().PaddingTop(15).Column(column =>
                            {
                                foreach (var term in terms)
                                {
                                    column.Item().Row(row =>
                                    {
                                        row.RelativeItem(2)
                                            .Border(1)
                                            .Background(Colors.Grey.Lighten3)
                                            .Padding(15)
                                            .Repeat()
                                            .Text(term.Item1);
                                    
                                        row.RelativeItem(3)
                                            .Border(1)
                                            .Padding(15)
                                            .Text(term.Item2);
                                    });
                                }
                            });
                        });
                });
            })
            .GenerateImages(x => $"repeat-with-{x}.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
}