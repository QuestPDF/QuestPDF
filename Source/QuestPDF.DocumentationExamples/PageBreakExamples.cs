using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class PageBreakExamples
{
    [Test]
    public void Example()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(300, 450);
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content()
                        .PaddingTop(15)
                        .Column(column =>
                        {
                            var terms = new[]
                            {
                                ("Garbage Collection", "An automatic memory management feature in many programming languages that identifies and removes unused objects to free up memory, preventing memory leaks."),
                                ("Constructor", "A special method in object-oriented programming that is automatically called when an object is created. It initializes the object's properties and sets up any necessary resources."),
                                ("Dependency", "A software component or external library that a program relies on to function correctly. Dependencies can include third-party modules, frameworks, or system-level packages that provide additional functionality without requiring developers to write everything from scratch.")
                            };
                            
                            column.Item()
                                .Extend()
                                .AlignCenter().AlignMiddle()
                                .Text("Programming dictionary").FontSize(24).Bold();
                            
                            foreach (var term in terms)
                            {
                                column.Item().PageBreak();
                                column.Item().Element(c => GeneratePage(c, term.Item1, term.Item2));
                            }

                            static void GeneratePage(IContainer container, string term, string definition)
                            {
                                container.Text(text =>
                                {
                                    text.Span(term).Bold().FontColor(Colors.Blue.Darken2);
                                    text.Span($" - {definition}");
                                });
                            }
                        });
                });
            })
            .GeneratePdf("page-break.pdf");
    }
}