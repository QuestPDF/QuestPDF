using System.Text.Json;
using System.Text.Json.Serialization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class SemanticExamples
{
    public class BookTermModel
    {
        public string Term { get; set; }
        public string Description { get; set; }
        public string FirstLevelCategory { get; set; }
        public string SecondLevelCategory { get; set; }
        public string ThirdLevelCategory { get; set; }
    }
    
    [Test]
    public async Task GenerateBook()
    {
        QuestPDF.Settings.EnableCaching = false;
        QuestPDF.Settings.EnableDebugging = false;
        
        var serializerSettings = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        };
        
        var bookData = await File.ReadAllTextAsync("Resources/semantic-book-content.json");
        var terms = JsonSerializer.Deserialize<ICollection<BookTermModel>>(bookData, serializerSettings);
        var categories = terms
            .GroupBy(x => x.FirstLevelCategory)
            .Select(x => new
            {
                Category = x.Key,
                Terms = x
                    .GroupBy(y => y.SecondLevelCategory)
                    .Select(y => new
                    {
                        Category = y.Key,
                        Terms = y
                            .GroupBy(z => z.ThirdLevelCategory)
                            .Select(z => new
                            {
                                Category = z.Key,
                                Terms = z.ToList()
                            })
                            .ToList()
                    })
                    .ToList()
            })
            .ToList();
        
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(50);
                    page.PageColor(Colors.White);

                    page.Header()
                        .Text("Programming Terms")
                        .Bold()
                        .FontSize(36);
                    
                    page.Content()
                        .PaddingVertical(24)
                        .SemanticDocument()
                        .Column(column =>
                        {
                            foreach (var category1 in categories)
                            {
                                column.Item()
                                    .SemanticSection()
                                    .EnsureSpace(100)
                                    .Column(column =>
                                    {
                                        column.Spacing(24);
                                        
                                        column.Item()
                                            .PaddingBottom(8)
                                            .SemanticHeader1(category1.Category)
                                            .Text(category1.Category)
                                            .FontSize(24)
                                            .FontColor(Colors.Blue.Darken4)
                                            .Bold();

                                        foreach (var category2 in category1.Terms)
                                        {
                                            column.Item()
                                                .SemanticSection()
                                                .EnsureSpace(100)
                                                .Column(column =>
                                                {
                                                    column.Spacing(8);
                                                    
                                                    column.Item()
                                                        .PaddingBottom(8)
                                                        .SemanticHeader2(category2.Category)
                                                        .Text(category2.Category)
                                                        .FontSize(20)
                                                        .FontColor(Colors.Blue.Darken2)
                                                        .Bold();

                                                    foreach (var category3 in category2.Terms)
                                                    {
                                                        column.Item()
                                                            .SemanticSection()
                                                            .EnsureSpace(100)
                                                            .Column(column =>
                                                            {
                                                                column.Spacing(8);
                                                                
                                                                column.Item()
                                                                    .PaddingBottom(8)
                                                                    .SemanticHeader3(category3.Category)
                                                                    .Text(category3.Category)
                                                                    .FontSize(16)
                                                                    .FontColor(Colors.Blue.Medium)
                                                                    .Bold();

                                                                foreach (var term in category3.Terms)
                                                                {
                                                                    column.Item()
                                                                        .SemanticParagraph()
                                                                        .Text(text =>
                                                                        {
                                                                            text.Span(term.Term).Bold();
                                                                            text.Span(" - ");
                                                                            text.Span(term.Description);
                                                                        });
                                                                }
                                                            });
                                                    }
                                                });
                                        }
                                    });
                                
                                column.Item().PageBreak();
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Page ");
                            text.CurrentPageNumber();
                            text.Span(" of ");
                            text.TotalPages();
                        });
                });
            })
            .WithMetadata(new DocumentMetadata()
            {
                Title = "Programming Terms",
                Language = "en-US"
            })
            .GeneratePdfAndShow();
    }
}