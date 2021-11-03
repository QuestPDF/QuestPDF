using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples
{
    public class TextBenchmark
    {
        [Test]
        public void Generate()
        {
            var chapters = GetBookChapters().ToList();
            
            RenderingTest
                .Create()
                .PageSize(PageSizes.A4)
                .ProducePdf()
                .ShowResults()
                .Render(x => ComposeBook(x, chapters));
        }
        
        [Test]
        public void Benchmark()
        {
            var chapters = GetBookChapters().ToList();
  
            var results = PerformTest(16).ToList();
 
            Console.WriteLine($"Min: {results.Min():F}");
            Console.WriteLine($"Max: {results.Max():F}");
            Console.WriteLine($"Avg: {results.Average():F}");
            
            void GenerateDocument()
            {
                RenderingTest
                    .Create()
                    .PageSize(PageSizes.A4)
                        .ProducePdf()
                    .Render(x => ComposeBook(x, chapters));
            }

            IEnumerable<float> PerformTest(int attempts)
            {
                foreach (var i in Enumerable.Range(0, attempts))
                {
                    var timer = new Stopwatch();
                
                    timer.Start();
                    GenerateDocument();
                    timer.Stop();

                    Console.WriteLine($"Attempt {i}: {timer.ElapsedMilliseconds:F}");
                    yield return timer.ElapsedMilliseconds;
                }
            }
        }

        class BookChapter
        {
            public string Title { get; set; }
            public string Content { get; set; }
        }
        
        private static IEnumerable<BookChapter> GetBookChapters()
        {
            var book = File.ReadAllLines("quo-vadis.txt");
            
            var chapterPointers = book
                .Select((line, index) => new
                {
                    LineNumber = index,
                    Text = line
                })
                .Where(x => x.Text.Length < 50 && x.Text.Contains("Rozdział") || x.Text.Contains("-----"))
                .Select(x => x.LineNumber)
                .ToList();

            foreach (var index in Enumerable.Range(0, chapterPointers.Count - 1))
            {
                var chapter = chapterPointers[index];
                    
                var title = book[chapter];
                    
                var lineFrom = chapterPointers[index];
                var lineTo = chapterPointers[index + 1] - 1;

                var lines = book.Skip(lineFrom + 1).Take(lineTo - lineFrom).Where(x => !string.IsNullOrWhiteSpace(x));
                var content = string.Join(Environment.NewLine, lines);

                yield return new BookChapter
                {
                    Title = title,
                    Content = content
                };
            }
        }
        
        private void ComposeBook(IContainer container, ICollection<BookChapter> chapters)
        {
            var subtitleStyle = TextStyle.Default.Size(24).SemiBold().Color(Colors.Blue.Medium);
            var normalStyle = TextStyle.Default.Size(14);
            
            ComposePage(container);

            void ComposePage(IContainer container)
            {
                container
                    .Padding(50)
                    .Decoration(decoration =>
                    {
                        decoration
                            .Content()
                            .Stack(stack =>
                            {
                                stack.Item().Element(Title);
                                stack.Item().PageBreak();
                                stack.Item().Element(TableOfContents);
                                stack.Item().PageBreak();

                                Chapters(stack);

                                stack.Item().Element(Acknowledgements);
                            });

                        decoration.Footer().Element(Footer);
                    });
            }
            
            void Title(IContainer container)
            {
                container
                    .Extend()
                    .PaddingBottom(200)
                    .AlignBottom()
                    .Stack(stack =>
                    {
                        stack.Item().Text("Quo Vadis", TextStyle.Default.Size(72).Bold().Color(Colors.Blue.Darken2));
                        stack.Item().Text("Henryk Sienkiewicz", TextStyle.Default.Size(24).Color(Colors.Grey.Darken2));
                    });
            }

            void TableOfContents(IContainer container)
            {
                container.Stack(stack =>
                {
                    SectionTitle(stack, "Spis treści");
                    
                    foreach (var chapter in chapters)
                    {
                        stack.Item().InternalLink(chapter.Title).Row(row =>
                        {
                            row.RelativeColumn().Text(chapter.Title, normalStyle);
                            row.ConstantColumn(100).AlignRight().Text(text => text.PageNumberOfLocation(chapter.Title, normalStyle));
                        });
                    }
                });
            }

            void Chapters(StackDescriptor stack)
            {
                foreach (var chapter in chapters)
                {
                    stack.Item().Element(container => Chapter(container, chapter.Title, chapter.Content));
                }
            }
            
            void Chapter(IContainer container, string title, string content)
            {
                container.Stack(stack =>
                {
                    SectionTitle(stack, title);
  
                    stack.Item().Text(text =>
                    {
                        text.ParagraphSpacing(5);
                        text.Span(content, normalStyle);
                    });
                    
                    stack.Item().PageBreak();
                });
            }

            void Acknowledgements(IContainer container)
            {
                container.Stack(stack =>
                {
                    SectionTitle(stack, "Podziękowania");
                    
                    stack.Item().Text(text =>
                    {
                        text.DefaultTextStyle(normalStyle);
                        
                        text.Span("Ten dokument został wygenerowany na podstawie książki w formacie TXT opublikowanej w serwisie ");
                        text.ExternalLocation("wolnelektury.pl", "https://wolnelektury.pl/", normalStyle.Color(Colors.Blue.Medium).Underline());
                        text.Span(". Dziękuję za wspieranie polskiego czytelnictwa!");
                    });
                });
            }

            void SectionTitle(StackDescriptor stack, string text)
            {
                stack.Item().Location(text).Text(text, subtitleStyle);
                stack.Item().PaddingTop(10).PaddingBottom(50).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).ExtendHorizontal();
            }
            
            void Footer(IContainer container)
            {
                container
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });
            }
        }
    }
}