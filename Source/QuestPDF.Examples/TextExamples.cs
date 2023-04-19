using System;
using System.Linq;
using System.Text;
using NUnit.Framework;
using QuestPDF.Elements.Text;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Examples
{
    public class TextExamples
    {
        [Test]
        public void SimpleText()
        {
            RenderingTest
                .Create()
                .PageSize(500, 100)
                
                .ProduceImages()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(5)
                        .MinimalBox()
                        .Border(1)
                        .Padding(10)
                        .Text(Placeholders.Paragraph());
                });
        }
        
        [Test]
        public void SimpleTextBlock()
        {
            RenderingTest
                .Create()
                .PageSize(600, 300)
                
                .ProduceImages()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(5)
                        .MinimalBox()
                        .Border(1)
                        .MaxWidth(300)
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
        public void TextWeight()
        {
            RenderingTest
                .Create()
                .PageSize(500, 500)
                .ProduceImages()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(20)
                        .MinimalBox()
                        .Border(1)
                        .Padding(20)
                        .Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontFamily(Fonts.Calibri).FontSize(20));

                            text.Line("Thin").Thin();
                            text.Line("ExtraLight").ExtraLight();
                            text.Line("Light").Light();
                            text.Line("NormalWeight").NormalWeight();
                            text.Line("Medium").Medium();
                            text.Line("SemiBold").SemiBold();
                            text.Line("Bold").Bold();
                            text.Line("ExtraBold").ExtraBold();
                            text.Line("Black").Black();
                            text.Line("ExtraBlack").ExtraBlack();
                        });
                });
        }
        
        [Test]
        public void LineHeight()
        {
            RenderingTest
                .Create()
                .PageSize(500, 700)
                .ProduceImages()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(20)
                        .Column(column =>
                        {
                            var lineHeights = new[] { 0.8f, 1f, 1.5f };
                            var paragraph = Placeholders.Paragraph();

                            foreach (var lineHeight in lineHeights)
                            {
                                column
                                    .Item()
                                    .Border(1)
                                    .Padding(10)
                                    .Text(paragraph)
                                    .FontSize(16)
                                    .LineHeight(lineHeight);
                            }
                        });
                });
        }

        [Test]
        public void LetterSpacing()
        {
            RenderingTest
                .Create()
                .PageSize(500, 700)
                .ProduceImages()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(20)
                        .Column(column =>
                        {
                            var letterSpacing = new[] { -0.1f, 0f, 0.2f };
                            var paragraph = Placeholders.Sentence().ToUpper();

                            foreach (var spacing in letterSpacing)
                            {
                                column
                                    .Item()
                                    .Border(1)
                                    .Padding(10)
                                    .Column(nestedColumn =>
                                    {
                                        nestedColumn.Item()
                                                    .Text(paragraph)
                                                    .FontSize(16)
                                                    .LetterSpacing(spacing);

                                        nestedColumn.Item()
                                                    .Text($"Letter spacing of {spacing} em")
                                                    .FontSize(10)
                                                    .Italic()
                                                    .FontColor(Colors.Blue.Medium);
                                    });
                                
                            }
                        });
                });
        }

        [Test]
        public void LetterSpacing_Arabic()
        {
            RenderingTest
                .Create()
                .PageSize(500, 700)
                .ProduceImages()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(50)
                        .Column(column =>
                        {
                            var letterSpacing = new[] { -0.1f, 0f, 0.2f };
                            var paragraph = "ينا الألم. في بعض الأحيان ونظراً للالتزامات التي يفرضها علينا";
                            foreach (var spacing in letterSpacing)
                            {
                                column
                                   .Item()
                                   .Border(1)
                                   .Padding(10)
                                   .Column(nestedColumn =>
                                   {
                                       nestedColumn.Item()
                                                   .Text(paragraph)
                                                   .FontSize(16)
                                                   .FontFamily(Fonts.Calibri)
                                                   .LetterSpacing(spacing);

                                       nestedColumn.Item()
                                                   .Text($"Letter spacing of {spacing} em")
                                                   .FontSize(10)
                                                   .Italic()
                                                   .FontColor(Colors.Blue.Medium);
                                   });
                            }
                        });
                });
        }


        [Test]
        public void LetterSpacing_Unicode()
        {
            RenderingTest
                .Create()
                .PageSize(500, 700)
                .ProduceImages()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(50)
                        .Column(column =>
                        {
                            var letterSpacing = new[] { 0f, 0.5f };
                            
                            
                            
                            var paragraph = "Ţ̴̡̧̤̮̺̤̗͎̱̹͙͎͖͂̿̓́̉̊̀̍͜h̵̞̘͇̾̎̏̅į̵̹̖͔͉̰̎̉̄̐̏͑͂̅̃̃͘͝s̷͓͉̭̭̯̬̥̻̰̩̦̑̀̀͌́̒̍̒̌̇͛̀͛́̎ ̷̡̡̟͕̳̺̝̼͇͔̬̟̖͍̈́̽͜͝͝i̶͔͚̟̊̐͛́͛̄̌ṡ̸̡̤̪͙͍̥͙̟̼̝̰̥͈̿̓̄̿̓͠ ̶̢̦̙͍̯̖̱̰̯͕͔͎̯̝̎͑t̸͖̲̱̼̎͐̎̉̾̎̾̌̅̔̏͘ȩ̶̝̫̙͓̙̣̔̀̌̔̋̂̑̈́̏̀̈͘̕͜͝s̸̫̝̮̻̼͐̅̄̎̎̑͝ț̷̨̢̨̻͈̮̞̆͗̓͊̃̌͂̑̉̕̕͜͝͝";

                            
                            
                            foreach (var spacing in letterSpacing)
                            {
                                column.Item()
                                      .Text($"Letter spacing of {spacing} em")
                                      .FontSize(10)
                                      .Italic()
                                      .FontColor(Colors.Blue.Medium);

                                column.Item()
                                      .PaddingVertical(50)
                                      .Text(paragraph)
                                      .FontSize(16)
                                      .FontFamily(Fonts.Calibri)
                                      .LetterSpacing(spacing);
                            }
                        });
                });
        }

        [Test]
        public void SuperscriptSubscript_Simple()
        {
            RenderingTest
                .Create()
                .PageSize(500, 500)
                .ProduceImages()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(20)
                        .MinimalBox()
                        .Border(1)
                        .Padding(20)
                        .Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(20));
                            text.ParagraphSpacing(10);

                            var highlight = TextStyle.Default.BackgroundColor(Colors.Green.Lighten3);

                            text.Span("E=mc").Style(highlight);
                            text.Span("2").Superscript().Style(highlight);
                            text.Span(" is the equation of mass–energy equivalence.");

                            text.EmptyLine();
                            
                            text.Span("H").Style(highlight);
                            text.Span("2").Subscript().Style(highlight);
                            text.Span("O").Style(highlight);
                            text.Span(" is the chemical formula for water.");
                        });
                });
        }

        [Test]
        public void SuperscriptSubscript_Effects()
        {
            RenderingTest
               .Create()
               .PageSize(800, 400)
               .ProduceImages()
               .ShowResults()
               .Render(container =>
               {
                   container
                        .Padding(25)
                        .DefaultTextStyle(x => x.FontSize(30))
                        .Column(column =>
                        {
                            column.Spacing(25);
                            
                            column.Item().Text(text =>
                            {
                                text.DefaultTextStyle(x => x.Underline());
                                
                                text.Span("Underline of the superscript (E = mc");
                                text.Span("2").Superscript();
                                text.Span(") should be at the same height as for normal text.");
                            });
                            
                            column.Item().Text(text =>
                            {
                                text.DefaultTextStyle(x => x.Underline());
                                
                                text.Span("Underline of the subscript(H");
                                text.Span("2").Subscript();
                                text.Span("O) should be slightly lower than a normal text.");
                            });
                            
                            column.Item().Text(text =>
                            {
                                text.DefaultTextStyle(x => x.Strikethrough());
                                
                                text.Span("Strikethrough of both superscript (E=mc");
                                text.Span("2").Superscript();
                                text.Span(") and subscript(H");
                                text.Span("2").Subscript();
                                text.Span("O) should be visible in the middle of the text.");
                            });
                        });
               });
        }

        [Test]
        public void ParagraphSpacing()
        {
            RenderingTest
                .Create()
                .PageSize(500, 500)
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
                            text.Line(Placeholders.Paragraph());
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
                            text.DefaultTextStyle(TextStyle.Default.FontSize(20).BackgroundColor(Colors.Red.Lighten4));
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

                                text.Hyperlink("Please visit QuestPDF website. ", "https://www.questpdf.com");
                                
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
                            column.Item().Text((string) null);

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
                            column.Item().Text((string) null);

                            column.Item().Text(text =>
                            {
                                text.DefaultTextStyle(x => x.BackgroundColor(Colors.Red.Lighten3).FontSize(24));
                                
                                text.Span("       " + Placeholders.LoremIpsum());
                                text.Span(" 012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789         ").WrapAnywhere();
                            });
                        });
                    });
                });
        }
        
        [Test]
        public void TextShaping_Unicode()
        {
            RenderingTest
                .Create()
                .PageSize(600, 100)
                
                .ProduceImages()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(35)
                        .MinimalBox()
                        .Background(Colors.Grey.Lighten2)
                        .Text(text =>
                        {
                            text.DefaultTextStyle(TextStyle.Default.FontSize(20));
                            
                            text.Span("Complex Unicode structure: ");
                            
                            
                            text.Span("T̶̖̔͆͆̽̔ḩ̷̼̫̐̈́̀͜͝͝ì̶͇̤͓̱̣͇͓͉̎s̵̡̟̹͍̜͉̗̾͛̈̐́͋͂͝͠ͅ ̴̨͙͍͇̭̒͗̀́͝ì̷̡̺͉̼̏̏̉̌͝s̷͍͙̗̰̖͙̈̑̂̔͑͊̌̓̊̇͜ ̶̛̼͚͊̅͘ṭ̷̨̘̣̙̖͉͌̏̂̅͑̄̽̕͝ȅ̶̲̲̙̭͈̬̣͔̝͔̈́͝s̸̢̯̪̫͓̭̮̓̀͆͜ț̸̢͉̞̥̤̏̌̓͝").FontFamily(Fonts.Calibri).FontColor(Colors.Red.Medium);
                            
                            
                            text.Span(".");
                        });
                });
        }
        
        [Test]
        public void TextShaping_Arabic()
        {
            RenderingTest
                .Create()
                .PageSize(250, 100)
                
                .ProduceImages()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(25)
                        .MinimalBox()
                        .Background(Colors.Grey.Lighten2)
                        .Text("خوارزمية ترتيب")
                        .FontFamily(Fonts.Calibri)
                        .FontSize(30);
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
                        page.DefaultTextStyle(x => x
                            .Fallback(y => y.FontFamily("Segoe UI Emoji")
                                .Fallback(y => y.FontFamily("Microsoft YaHei"))));

                        page.Size(PageSizes.A4);

                        page.Content().Text(t =>
                        {
                            t.Line("This is normal text.");
                            t.EmptyLine();
                            
                            t.Line("Following line should use font fallback:");
                            t.Line("中文文本");
                            t.EmptyLine();
                            
                            t.Line("The following line contains a mix of known and unknown characters.");
                            t.Line("Mixed line: This 中文 is 文文 a mixed 本 本 line 本 中文文本!");
                            t.EmptyLine();

                            t.Line("Emojis work out of the box because of font fallback: 😊😅🥳👍❤😍👌");
                        });
                    });
                });
        }
        
        [Test]
        public void WordWrappingStability()
        {
            // instruction: check if any characters repeat when performing the word-wrapping algorithm
            
            RenderingTest
                .Create()
                .PageSize(PageSizes.A4)
                .ProducePdf()
                .ShowResults()
                .Render(container =>
                {
                    var text = "Lorem ipsum dolor sit amet consectetuer";
                    
                    container
                        .Padding(20)
                        .Column(column =>
                        {
                            column.Spacing(10);

                            foreach (var width in Enumerable.Range(25, 200))
                            {
                                column
                                    .Item()
                                    .MaxWidth(width)
                                    .Background(Colors.Grey.Lighten3)
                                    .Text(text);
                            }
                        });
                });
        }
        
        [Test]
        public void AdvancedLanguagesSupport()
        {
            RenderingTest
                .Create()
                .PageSize(new PageSize(400, 400))
                .ProduceImages()
                .ShowResults()
                .Render(container =>
                {
                    var text = "في المعلوماتية أو الرياضيات، خوارزمية الترتيب هي خوارزمية تمكن من تنظيم مجموعة عناصر حسب ترتيب محدد.";
                    
                    container
                        .Padding(20)
                        .ContentFromRightToLeft()
                        .Text(text)
                        .FontFamily(Fonts.Calibri)
                        .FontSize(22);
                });
        }
        
        [Test]
        public void WordWrappingWhenRightToLeft()
        {
            RenderingTest
                .Create()
                .PageSize(new PageSize(1000, 500))
                .ProducePdf()
                .ShowResults()
                .Render(container =>
                {
                    var text = "في المعلوماتية أو الرياضيات، خوارزمية الترتيب هي خوارزمية تمكن من تنظيم مجموعة عناصر حسب ترتيب محدد.";
                    
                    container
                        .Padding(25)
                        .ContentFromRightToLeft()
                        .Column(column =>
                        {
                            column.Spacing(20);
                            
                            foreach (var size in new[] { 36, 34, 32, 30, 15 })
                            {
                                column
                                    .Item()
                                    .ShowEntire()
                                    .MaxWidth(size * 25)
                                    .Background(Colors.Grey.Lighten3)
                                    .MinimalBox()
                                    .Background(Colors.Grey.Lighten2)
                                    .Text(text)
                                    .FontSize(20)
                                    .FontFamily("Segoe UI");
                            }
                        });
                });
        }
        
        [Test]
        public void ForcingTextDirection()
        {
            RenderingTest
                .Create()
                .PageSize(new PageSize(1000, 500))
                .ProduceImages()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(10)
                        .DefaultTextStyle(x => x.FontSize(24).FontFamily("Calibri"))
                        .Column(column =>
                        {
                            column.Spacing(10);
                            
                            var word = "الجوريتم";
                            var definition = "algorithm in Arabic";

                            var text = $"{word} - {definition}";
                            
                            // text direction is automatically detected using the first word
                            column.Item().Text(text);
                            
                            // it is possible to force specific content direction
                            column.Item().Text(text).DirectionFromLeftToRight();
                            column.Item().Text(text).DirectionFromRightToLeft();

                            // to combine text in various content directions, split it into segments
                            column.Item().Text(text =>
                            {
                                text.Span(word);
                                text.Span(" - ");
                                text.Span(definition);
                            });
                        });
                });
        }
        
        [Test]
        public void DetectSpanPositionExample()
        {
            RenderingTest
                .Create()
                .PageSize(new PageSize(650, 800))
                .ProduceImages()
                .ShowResults()
                .Render(container =>
                {
                    var fontSize = 20;
                    
                    var paint = new SKPaint
                    {
                        Color = SKColors.Red,
                        TextSize = fontSize
                    };
                    
                    var fontMetrics = paint.FontMetrics;

                    var start = 0f;
                    var end = 0f;
                    
                    // corner case: what if text is paged? clamp start and end?

                    container
                        .Padding(25)
                        .DefaultTextStyle(x => x.FontSize(fontSize).FontFamily("Calibri"))
                        .Layers(layers =>
                        {
                            layers.PrimaryLayer().Text(text =>
                            {
                                text.Span(Placeholders.Paragraph());
                                text.Span(" - ");
                                
                                // record start
                                text.Element().Width(0).Height(0)
                                    .Canvas((canvas, size) => start = canvas.TotalMatrix.TransY / canvas.TotalMatrix.ScaleY);
                                
                                text.Span(Placeholders.LoremIpsum()).BackgroundColor(Colors.Red.Lighten4);
                                
                                // record end
                                text.Element().Width(0).Height(0)
                                    .Canvas((canvas, size) => end = canvas.TotalMatrix.TransY / canvas.TotalMatrix.ScaleY);
                            
                                text.Span(" - ");
                                text.Span(Placeholders.Paragraph());
                            });
                            
                            layers.Layer().Canvas((canvas, size) =>
                            {
                                canvas.Save();
      
                                canvas.Translate(-canvas.TotalMatrix.TransX / canvas.TotalMatrix.ScaleX, -canvas.TotalMatrix.TransY / canvas.TotalMatrix.ScaleY);
                                canvas.DrawRect(10, start + fontMetrics.Ascent, 5, end - start + (fontMetrics.Bottom - fontMetrics.Ascent), paint);
                                
                                canvas.Restore();
                            });
                        });
                });
        }
        
        [Test]
        public void InconsistentLineHeightWhenUsingNewLineTest()
        {
            RenderingTest
                .Create()
                .PageSize(PageSizes.A4)
                .ProduceImages()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(20)
                        .Background(Colors.Grey.Lighten4)
                        .Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(16));
                            
                            text.Line(Placeholders.Paragraph());
                            text.Line("");
                            text.Line(Placeholders.Paragraph());
                            
                            text.Line(Placeholders.Label()).FontSize(48);
                            
                            text.Line(Placeholders.Paragraph());
                            text.Line("");
                            text.Line(Placeholders.Paragraph());
                        });
                });
        }
        
        [Test]
         public void FontFallback_Nested()
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
                        page.Size(PageSizes.A6.Landscape());
                        
                        page.DefaultTextStyle(x => x
                            .FontSize(24)
                            .Bold()
                            .Fallback(y => y
                                .FontFamily("Microsoft YaHei")
                                .Underline()
                                .BackgroundColor(Colors.Red.Lighten2)));

                        page.Content().Text(text =>
                        {
                            text.Line("Normal 中文文本 text.");
                            text.Line("Background 中文文本 text.").NormalWeight().BackgroundColor(Colors.Green.Lighten2);
                            text.Line("Background 中文文本 text.").Strikethrough().Underline(false);
                            text.Line("Background 中文文本 text.").Italic();
                        });
                    });
                });
        }
    }
}