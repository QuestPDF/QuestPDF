using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class SectionExamples
{
    [Test]
    public void Example()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A5.Landscape());
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content()
                        .Column(column =>
                        {
                            var terms = new[]
                            {
                                ("Bit", "The smallest unit of data in computing, representing either a 0 or a 1. Multiple bits are combined to form bytes, which are used to store larger data values."),
                                ("Byte", "A unit of digital information that consists of 8 bits. A byte is commonly used to store a single character of text, such as a letter or a number, in computer memory."),
                                ("Binary", "A number system that uses only two digits, 0 and 1, which are the fundamental building blocks of computer operations. Computers process and store all data in binary format, including text, images, and instructions."),
                                ("Array", "A data structure that stores a fixed-size sequence of elements, all of the same type, in a contiguous block of memory. Arrays allow quick access to elements using an index and are commonly used to manage collections of data.")
                            };

                            // title
                            column.Item().Extend().AlignMiddle().AlignCenter().Text("Programming Glossary").FontSize(32).Bold();
                            column.Item().PageBreak();
                            
                            // table of contents
                            column.Item().PaddingBottom(25).Text("Table of Contents").FontSize(24).Bold().Underline();
                            
                            foreach (var term in terms)
                            {
                                column.Item()
                                    .PaddingBottom(10)
                                    .SectionLink($"term-{term}")
                                    .Text(text =>
                                    {
                                        text.Span("Term ");
                                        text.Span(term.Item1).Bold();
                                        text.Span(" on page ");
                                        text.BeginPageNumberOfSection($"term-{term}");
                                    });
                            }
                            
                            // content
                            foreach (var term in terms)
                            {
                                column.Item().PageBreak();
                                
                                column.Item()
                                    .Section($"term-{term}")
                                    .Text(text =>
                                    {
                                        text.Span(term.Item1).Bold().FontColor(Colors.Blue.Darken2);
                                        text.Span(" - ");
                                        text.Span(term.Item2);
                                    });
                            }
                        });
                });
            })
            .GeneratePdf("sections.pdf");
    }
}