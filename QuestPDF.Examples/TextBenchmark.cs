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
                .RenderDocument(x => ComposeBook(x, chapters));
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
                    .RenderDocument(x => ComposeBook(x, chapters));
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
        
        private void ComposeBook(IDocumentContainer container, ICollection<BookChapter> chapters)
        {
            var subtitleStyle = TextStyle.Default.Size(24).SemiBold().Color(Colors.Blue.Medium);
            var normalStyle = TextStyle.Default.Size(14);

            container.Page(page =>
            {
                page.Margin(50);
                
                page.Content().Column(column =>
                {
                    column.Item().Element(Title);
                    column.Item().PageBreak();
                    column.Item().Element(TableOfContents);
                    column.Item().PageBreak();

                    Chapters(column);

                    column.Item().Element(Acknowledgements);
                });
                
                page.Footer().Element(Footer);
            });

            void Title(IContainer container)
            {
                container
                    .Extend()
                    .PaddingBottom(200)
                    .AlignBottom()
                    .Column(column =>
                    {
                        column.Item().Text("Quo Vadis").FontSize(72).Bold().FontColor(Colors.Blue.Darken2);
                        column.Item().Text("Henryk Sienkiewicz").FontSize(24).FontColor(Colors.Grey.Darken2);
                    });
            }

            void TableOfContents(IContainer container)
            {
                container.Column(column =>
                {
                    SectionTitle(column, "Spis treści");
                    
                    foreach (var chapter in chapters)
                    {
                        column.Item().InternalLink(chapter.Title).Row(row =>
                        {
                            row.RelativeItem().Text(chapter.Title).Style(normalStyle);
                            row.ConstantItem(100).AlignRight().Text(text => text.BeginPageNumberOfSection(chapter.Title).Style(normalStyle));
                        });
                    }
                });
            }

            void Chapters(ColumnDescriptor column)
            {
                foreach (var chapter in chapters)
                {
                    column.Item().Element(container => Chapter(container, chapter.Title, chapter.Content));
                }
            }
            
            void Chapter(IContainer container, string title, string content)
            {
                container.Column(column =>
                {
                    SectionTitle(column, title);
  
                    column.Item().Text(text =>
                    {
                        text.ParagraphSpacing(5);
                        text.Span(content).Style(normalStyle);
                    });
                    
                    column.Item().PageBreak();
                });
            }

            void Acknowledgements(IContainer container)
            {
                container.Column(column =>
                {
                    SectionTitle(column, "Podziękowania");
                    
                    column.Item().Text(text =>
                    {
                        text.DefaultTextStyle(normalStyle);
                        
                        text.Span("Ten dokument został wygenerowany na podstawie książki w formacie TXT opublikowanej w serwisie ");
                        text.Hyperlink("wolnelektury.pl", "https://wolnelektury.pl/").FontColor(Colors.Blue.Medium).Underline();
                        text.Span(". Dziękuję za wspieranie polskiego czytelnictwa!");
                    });
                });
            }

            void SectionTitle(ColumnDescriptor column, string text)
            {
                column.Item().Location(text).Text(text).Style(subtitleStyle);
                column.Item().PaddingTop(10).PaddingBottom(50).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).ExtendHorizontal();
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