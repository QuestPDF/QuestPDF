using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples
{
    public class TextBenchmark
    { 
        [Test]
        public void Benchmark()
        {
            var subtitleStyle = TextStyle.Default.Size(24).SemiBold().Color(Colors.Blue.Medium);
            var normalStyle = TextStyle.Default.Size(14);
            
            var chapters = GetChapters().ToList();
  
            var results = PerformTest(16).ToList();
 
            Console.WriteLine($"Min: {results.Min():F}");
            Console.WriteLine($"Max: {results.Max():F}");
            Console.WriteLine($"Avg: {results.Average():F}");

            IEnumerable<(string title, string content)> GetChapters()
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
                    
                    var lines = book.Skip(lineFrom + 1).Take(lineTo - lineFrom);
                    var content = string.Join(Environment.NewLine, lines);

                    yield return (title, content);
                }
            }
            
            void GenerateDocument()
            {
                RenderingTest
                    .Create()
                    .PageSize(PageSizes.A4)
                    .FileName()
                    .ProducePdf()
                    .Render(ComposePage);
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
                    stack.Item().Text("Table of contents", subtitleStyle);
                    stack.Item().PaddingTop(10).PaddingBottom(50).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).ExtendHorizontal();
                    
                    foreach (var chapter in chapters)
                    {
                        stack.Item().InternalLink(chapter.title).Row(row =>
                        {
                            row.RelativeColumn().Text(chapter.title);
                            row.ConstantColumn(100).AlignRight().Text(text => text.PageNumberOfLocation(chapter.title, normalStyle));
                        });
                    }
                });
            }

            void Chapters(StackDescriptor stack)
            {
                foreach (var chapter in chapters)
                {
                    stack.Item().Element(container => Chapter(container, chapter.title, chapter.content));
                }
            }
            
            void Chapter(IContainer container, string title, string content)
            {
                container.Stack(stack =>
                {
                    stack.Item().Location(title).Text(title, subtitleStyle);
                    stack.Item().PaddingTop(10).PaddingBottom(50).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).ExtendHorizontal();
  
                    stack.Item().Text(text =>
                    {
                        text.ParagraphSpacing(10);
                        text.Span(content, normalStyle);
                    });
                    
                    stack.Item().PageBreak();
                });
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