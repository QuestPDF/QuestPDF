using System.Linq;
using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples
{
    public class TextExamples
    {
        [Test]
        public void TextElements()
        {
            RenderingTest
                .Create()
                .PageSize(PageSizes.A4)
                .FileName()
                .ProducePdf()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(20)
                        .Padding(10)
                        .Box()
                        .Border(1)
                        .Padding(5)
                        .Padding(10)
                        .Text(text =>
                        {
                            text.DefaultTextStyle(TextStyle.Default);
                            text.AlignLeft();
                            text.ParagraphSpacing(10);

                            text.Span(Placeholders.LoremIpsum());

                            text.Line("This is target text that does not show up. > This is a short sentence that will be wrapped into second line hopefully, right? <");
                        });
                });
        }

        [Test]
        public void SpaceIssue()
        {
            RenderingTest
                .Create()
                .PageSize(PageSizes.A4)
                .FileName()
                .ProducePdf()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(20)
                        .Padding(10)
                        .Box()
                        .Border(1)
                        .Padding(5)
                        .Padding(10)
                        .Text(text =>
                        {
                            text.DefaultTextStyle(TextStyle.Default);
                            text.AlignLeft();
                            text.ParagraphSpacing(10);

                            text.Span(Placeholders.LoremIpsum());

                            text.EmptyLine();

                            text.Span("This text is a normal text, ");
                            text.Span("this is a bold text, ", TextStyle.Default.Bold());
                            text.Span("this is a red and underlined text, ", TextStyle.Default.Color(Colors.Red.Medium).Underlined());
                            text.Span("and this is slightly bigger text.", TextStyle.Default.Size(16));

                            text.EmptyLine();

                            text.Span("The new text element also supports injecting custom content between words: ");
                            text.Element().PaddingBottom(-10).Height(16).Width(32).Image(Placeholders.Image);
                            text.Span(".");

                            text.EmptyLine();

                            text.Span("This is page number ");
                            text.CurrentPageNumber();
                            text.Span(" out of ");
                            text.TotalPages();

                            text.EmptyLine();

                            text.ExternalLocation("Please visit QuestPDF website", "https://www.questpdf.com");

                            text.EmptyLine();

                            text.Span(Placeholders.Paragraphs());
                            
                            
                            text.EmptyLine();

                            text.Span(Placeholders.Paragraphs(), TextStyle.Default.Italic());
                            
                            text.Line("This is target text that does not show up. " + Placeholders.Paragraph());
                        });
                });
        }

        [Test]
        public void HugeList()
        {
            RenderingTest
                .Create()
                .PageSize(PageSizes.A4)
                .FileName()
                .ProducePdf()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(20)
                        .Padding(10)
                        .Box()
                        .Border(1)
                        .Padding(5)
                        .Padding(10)
                        .Text(text =>
                        {
                            text.DefaultTextStyle(TextStyle.Default);
                            text.AlignLeft();
                            text.ParagraphSpacing(10);

                            text.Span("This text is a normal text, ");
                            text.Span("this is a bold text, ", TextStyle.Default.Bold());
                            text.Span("this is a red and underlined text, ", TextStyle.Default.Color(Colors.Red.Medium).Underlined());
                            text.Span("and this is slightly bigger text.", TextStyle.Default.Size(16));
                            
                            text.Span("The new text element also supports injecting custom content between words: ");
                            text.Element().PaddingBottom(-10).Height(16).Width(32).Image(Placeholders.Image);
                            text.Span(".");
                            
                            text.EmptyLine();
                            
                            foreach (var i in Enumerable.Range(1, 100))
                            {
                                text.Line($"{i}: {Placeholders.Paragraph()}");
                                
                                text.EmptyLine();

                                text.ExternalLocation("Please visit QuestPDF website", "https://www.questpdf.com");
                                
                                text.Span("This is page number ");
                                text.CurrentPageNumber();
                                text.Span(" out of ");
                                text.TotalPages();
                            }
                        });
                });
        }
    }
}