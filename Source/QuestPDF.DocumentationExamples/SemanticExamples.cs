using System.Text.Json;
using System.Text.Json.Serialization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class SemanticExamples
{
    [Test]
    public void HeaderAndFooter()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(600, 250));
                    page.DefaultTextStyle(x => x.FontSize(16));
                    page.Margin(25);

                    page.Content()
                        .Border(1)
                        .BorderColor(Colors.Grey.Lighten1)
                        .Table(table =>
                        {
                            table.ApplySemanticTags();
                            
                            var pageSizes = new List<(string name, double width, double height)>()
                            {
                                ("Letter (ANSI A)", 8.5f, 11),
                                ("Legal", 8.5f, 14),
                                ("Ledger (ANSI B)", 11, 17),
                                ("Tabloid (ANSI B)", 17, 11),
                                ("ANSI C", 22, 17),
                                ("ANSI D", 34, 22),
                                ("ANSI E", 44, 34)
                            };

                            const int inchesToPoints = 72;

                            IContainer DefaultCellStyle(IContainer container, string backgroundColor)
                            {
                                return container
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Lighten1)
                                    .Background(backgroundColor)
                                    .PaddingVertical(5)
                                    .PaddingHorizontal(10)
                                    .AlignCenter()
                                    .AlignMiddle();
                            }

                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();

                                columns.ConstantColumn(80);
                                columns.ConstantColumn(80);

                                columns.ConstantColumn(80);
                                columns.ConstantColumn(80);
                            });

                            table.Header(header =>
                            {
                                // please be sure to call the 'header' handler!

                                header.Cell().RowSpan(2).Element(CellStyle).ExtendHorizontal().AlignLeft()
                                    .Text("Document type").Bold();

                                header.Cell().ColumnSpan(2).Element(CellStyle).Text("Inches").Bold();
                                header.Cell().ColumnSpan(2).Element(CellStyle).Text("Points").Bold();

                                header.Cell().Element(CellStyle).Text("Width");
                                header.Cell().Element(CellStyle).Text("Height");

                                header.Cell().Element(CellStyle).Text("Width");
                                header.Cell().Element(CellStyle).Text("Height");

                                // you can extend existing styles by creating additional methods
                                IContainer CellStyle(IContainer container) =>
                                    DefaultCellStyle(container, Colors.Grey.Lighten3);
                            });

                            foreach (var page in pageSizes)
                            {
                                table.Cell().Element(CellStyle).ExtendHorizontal().AlignLeft().Text(page.name);

                                // inches
                                table.Cell().Element(CellStyle).Text(page.width);
                                table.Cell().Element(CellStyle).Text(page.height);

                                // points
                                table.Cell().Element(CellStyle).Text(page.width * inchesToPoints);
                                table.Cell().Element(CellStyle).Text(page.height * inchesToPoints);

                                IContainer CellStyle(IContainer container) =>
                                    DefaultCellStyle(container, Colors.White).ShowOnce();
                            }
                        });
                });
            })
            .GeneratePdf();
    }
    
    
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
                                            .SemanticHeader1()
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
                                                        .SemanticHeader2()
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
                                                                    .SemanticHeader3()
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
            .GeneratePdf();
    }
}