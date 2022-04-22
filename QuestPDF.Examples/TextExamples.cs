using System;
using System.Linq;
using System.Text;
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
        public void SimpleTextBlock()
        {
            RenderingTest
                .Create()
                .PageSize(500, 300)
                
                .ProduceImages()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(5)
                        .MinimalBox()
                        .Border(1)
                        .Padding(10)
                        .Text(text =>
                        {
                            text.DefaultTextStyle(TextStyle.Default.FontSize(20));
                            text.Span("This is a normal text, followed by an ");
                            text.Span("underlined red text").FontColor(Colors.Red.Medium).Underline();
                            text.Span(".");
                        });
                });
        }
        
        [Test]
        public void ParagraphSpacing()
        {
            RenderingTest
                .Create()
                .PageSize(500, 300)
                .ProduceImages()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(5)
                        .MinimalBox()
                        .Border(1)
                        .Padding(10)
                        .Text(text =>
                        {
                            text.ParagraphSpacing(10);
    
                            foreach (var i in Enumerable.Range(1, 3))
                            {
                                text.Span($"Paragraph {i}: ").SemiBold();
                                text.Line(Placeholders.Paragraph());
                            }
                        });
                });
        }
        
        [Test]
        public void CustomElement()
        {
            RenderingTest
                .Create()
                .PageSize(500, 200)
                .ProduceImages()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(5)
                        .MinimalBox()
                        .Border(1)
                        .Padding(10)
                        .Text(text =>
                        {
                            text.DefaultTextStyle(TextStyle.Default.FontSize(20));
                            text.Span("This is a random image aligned to the baseline: ");
                            
                            text.Element()
                                .PaddingBottom(-6)
                                .Height(24)
                                .Width(48)
                                .Image(Placeholders.Image);
                            
                            text.Span(".");
                        });
                });
        }
        
        [Test]
        public void TextElements()
        {
            RenderingTest
                .Create()
                .PageSize(PageSizes.A4)
                .ProducePdf()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(20)
                        .Padding(10)
                        .MinimalBox()
                        .Border(1)
                        .Padding(5)
                        .Padding(10)
                        .Text(text =>
                        {
                            text.DefaultTextStyle(TextStyle.Default);
                            text.AlignLeft();
                            text.ParagraphSpacing(10);

                            text.Line(Placeholders.LoremIpsum());

                            text.Span($"This is target text that should show up. {DateTime.UtcNow:T} > This is a short sentence that will be wrapped into second line hopefully, right? <").Underline();
                        });
                });
        }
        
        [Test]
        public void Textcolumn()
        {
            RenderingTest
                .Create()
                .PageSize(PageSizes.A4)
                .ProducePdf()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(20)
                        .Padding(10)
                        .MinimalBox()
                        .Border(1)
                        .Padding(5)
                        .Padding(10)
                        .Text(text =>
                        {
                            text.DefaultTextStyle(TextStyle.Default);
                            text.AlignLeft();
                            text.ParagraphSpacing(10);
                            
                            foreach (var i in Enumerable.Range(1, 100))
                                text.Line($"{i}: {Placeholders.Paragraph()}");
                        });
                });
        }

        [Test]
        public void SpaceIssue()
        {
            RenderingTest
                .Create()
                .PageSize(PageSizes.A4)
                .ProducePdf()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(20)
                        .Padding(10)
                        .MinimalBox()
                        .Border(1)
                        .Padding(5)
                        .Padding(10)
                        .Text(text =>
                        {
                            text.DefaultTextStyle(x => x.Bold());
                            
                            text.DefaultTextStyle(TextStyle.Default);
                            text.AlignLeft();
                            text.ParagraphSpacing(10);

                            text.Span(Placeholders.LoremIpsum());

                            text.EmptyLine();

                            text.Span("This text is a normal text, ");
                            text.Span("this is a bold text, ").Bold();
                            text.Span("this is a red and underlined text, ").FontColor(Colors.Red.Medium).Underline();
                            text.Span("and this is slightly bigger text.").FontSize(16);

                            text.EmptyLine();

                            text.Span("The new text element also supports injecting custom content between words: ");
                            text.Element().PaddingBottom(-4).Height(16).Width(32).Image(Placeholders.Image);
                            text.Span(".");

                            text.EmptyLine();

                            text.Span("This is page number ");
                            text.CurrentPageNumber();
                            text.Span(" out of ");
                            text.TotalPages();

                            text.EmptyLine();

                            text.Hyperlink("Please visit QuestPDF website", "https://www.questpdf.com");

                            text.EmptyLine();

                            text.Span(Placeholders.Paragraphs());
                            
                            
                            text.EmptyLine();

                            text.Span(Placeholders.Paragraphs()).Italic();
                            
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
                .ProducePdf()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(20)
                        .Padding(10)
                        .MinimalBox()
                        .Border(1)
                        .Padding(5)
                        .Padding(10)
                        .Text(text =>
                        {
                            text.DefaultTextStyle(TextStyle.Default.FontSize(20));
                            text.AlignLeft();
                            text.ParagraphSpacing(10);

                            text.Span("This text is a normal text, ");
                            text.Span("this is a bold text, ").Bold();
                            text.Span("this is a red and underlined text, ").FontColor(Colors.Red.Medium).Underline();
                            text.Span("and this is slightly bigger text.").FontSize(16);
                            
                            text.Span("The new text element also supports injecting custom content between words: ");
                            text.Element().PaddingBottom(-4).Height(16).Width(32).Image(Placeholders.Image);
                            text.Span(".");
                            
                            text.EmptyLine();
                            
                            foreach (var i in Enumerable.Range(1, 100))
                            {
                                text.Line($"{i}: {Placeholders.Paragraph()}");

                                text.Hyperlink("Please visit QuestPDF website", "https://www.questpdf.com");
                                
                                text.Span("This is page number ");
                                text.CurrentPageNumber();
                                text.Span(" out of ");
                                text.TotalPages();
                                
                                text.EmptyLine();
                            }
                        });
                });
        }
        
        [Test]
        public void MeasureIssueWhenSpaceAtLineEnd()
        {
            // issue 135
            
            RenderingTest
                .Create()
                .ProduceImages()
                .ShowResults()
                .RenderDocument(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(50);
                        page.PageColor(Colors.White);

                        page.Size(PageSizes.A4);

                        page.Content().Text("This is a specially crafted sentence with a specially chosen length for demonstration of the bug that occurs ;;;;;. ").FontSize(11).BackgroundColor(Colors.Red.Lighten3);
                    });
                });
        }
        
        [Test]
        public void EmptyText()
        {
            // issue 135
            
            RenderingTest
                .Create()
                .ProduceImages()
                .ShowResults()
                .RenderDocument(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(50);
                        page.PageColor(Colors.White);

                        page.Size(PageSizes.A4);

                        page.Content().Text("         ").FontSize(11).BackgroundColor(Colors.Red.Lighten3);
                    });
                });
        }
        
        [Test]
        public void Whitespaces()
        {
            // issue 135
            
            RenderingTest
                .Create()
                .ProduceImages()
                .ShowResults()
                .RenderDocument(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(50);
                        page.PageColor(Colors.White);

                        page.Size(PageSizes.A4);

                        page.Content().Text("     x     ").FontSize(11).BackgroundColor(Colors.Red.Lighten3);
                    });
                });
        }
        
        [Test]
        public void DrawingNullTextShouldNotThrowException()
        {
            RenderingTest
                .Create()
                .ProduceImages()
                .ShowResults()
                .RenderDocument(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(50);
                        page.PageColor(Colors.White);

                        page.Size(PageSizes.A4);

                        page.Content().Column(column =>
                        {
                            column.Item().Text(null);

                            column.Item().Text(text =>
                            {
                                text.Span(null);
                                text.Line(null);
                                text.Hyperlink(null, "http://www.questpdf.com");
                                text.TotalPages().Format(x => null);
                            });
                        });
                    });
                });
        }
        
        [Test]
        public void BreakingLongWord()
        {
            RenderingTest
                .Create()
                .ProduceImages()
                .ShowResults()
                .RenderDocument(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(50);
                        page.PageColor(Colors.White);

                        page.Size(PageSizes.A4);

                        page.Content().Column(column =>
                        {
                            column.Item().Text(null);

                            column.Item().Text(text =>
                            {
                                text.DefaultTextStyle(x => x.BackgroundColor(Colors.Red.Lighten3).FontSize(24));
                                
                                text.Span("       " + Placeholders.LoremIpsum());
                                text.Span(" 0123456789012345678901234567890123456789012345678901234567890123456789         ").WrapAnywhere();
                            });
                        });
                    });
                });
        }

        [Test]
        public void FontFallback()
        {
            RenderingTest
                .Create()
                .ProduceImages()
                .ShowResults()
                .RenderDocument(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(50);
                        page.PageColor(Colors.White);

                        page.Size(PageSizes.A4);

                        page.Content().Text(t =>
                        {
                            t.Line("This is normal text.");
                            t.EmptyLine();

                            t.Line("Following line should use font fallback:");
                            t.Line("中文文本");
                            t.EmptyLine();

                            t.Line("The following line contains a mix of known and unknown characters.");
                            t.Line("Mixed line: Thi中s 文is文文 a mixed本 本 line 本 中文文本!");
                            t.EmptyLine();

                            t.Line("Emojis work out of the box because of font fallback: 😊😅🥳👍❤😍👌");
                        });
                    });
                });
        }
    }
}