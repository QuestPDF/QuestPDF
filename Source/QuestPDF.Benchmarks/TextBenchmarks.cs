using BenchmarkDotNet.Attributes;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Benchmarks;

[Config(typeof(Config))]
public class TextBenchmarks
{
    private List<BookChapter>? _chapters;
    
    public TextBenchmarks()
    {
        Settings.License = LicenseType.Community;
    }

    [GlobalSetup]
    public void Setup()
    {
        _chapters = GetBookChapters().ToList();
    }

    [Benchmark]
    public void Bench()
    {
        RenderingTest
            .Create()
            .PageSize(PageSizes.A4)
            .ProducePdf()
            .RenderDocument(x => ComposeBook(x, _chapters!));
    }

    private record BookChapter(string Title, string Content);
        
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

        for(var index = 0; index < chapterPointers.Count - 1; index++)
        {
            var chapter = chapterPointers[index];
                
            var title = book[chapter];
                
            var lineFrom = chapterPointers[index];
            var lineTo = chapterPointers[index + 1] - 1;

            var lines = book.Skip(lineFrom + 1).Take(lineTo - lineFrom).Where(x => !string.IsNullOrWhiteSpace(x));
            var content = string.Join("\n", lines);

            yield return new BookChapter(title, content);
        }
    }
    
    private void ComposeBook(IDocumentContainer container, ICollection<BookChapter> chapters)
    {
        var subtitleStyle = TextStyle.Default.FontSize(24).SemiBold().FontColor(Colors.Blue.Medium);
        var normalStyle = TextStyle.Default.FontSize(14);

        container.Page(page =>
        {
            page.Margin(50);
            
            page.Content().PaddingVertical(10).Column(column =>
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

        void Title(IContainer cont)
        {
            cont
                .Extend()
                .PaddingBottom(200)
                .AlignBottom()
                .Column(column =>
                {
                    column.Item().Text("Quo Vadis").FontSize(72).Bold().FontColor(Colors.Blue.Darken2);
                    column.Item().Text("Henryk Sienkiewicz").FontSize(24).FontColor(Colors.Grey.Darken2);
                });
        }

        void TableOfContents(IContainer cont)
        {
            cont.Column(column =>
            {
                SectionTitle(column, "Spis treści");
                
                foreach (var chapter in chapters)
                {
                    column.Item().SectionLink(chapter.Title).Row(row =>
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
                column.Item().Element(cont => Chapter(cont, chapter.Title, chapter.Content));
            }
        }
        
        void Chapter(IContainer cont, string title, string content)
        {
            cont.Column(column =>
            {
                SectionTitle(column, title);
                column.Item().Text(content).ParagraphFirstLineIndentation(16).ParagraphSpacing(8).Justify();
                column.Item().PageBreak();
            });
        }

        void Acknowledgements(IContainer cont)
        {
            cont.Column(column =>
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
            column.Item().Section(text).Text(text).Style(subtitleStyle);
            column.Item().PaddingTop(10).PaddingBottom(50).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).ExtendHorizontal();
        }
        
        void Footer(IContainer cont)
        {
            cont
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