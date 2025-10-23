using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.ConformanceTests;

class ConformanceTestDocument : IDocument
{
    private TextStyle TextStyleHeader1 = TextStyle.Default.FontSize(24).Bold().FontColor(Colors.Blue.Darken4);
    private TextStyle TextStyleHeader2 = TextStyle.Default.FontSize(18).Bold().FontColor(Colors.Blue.Darken2);
    private TextStyle TextStyleHeader3 = TextStyle.Default.FontSize(14).Bold().FontColor(Colors.Blue.Medium);

    public DocumentMetadata GetMetadata()
    {
        return new DocumentMetadata
        {
            Language = "en-US",
            Title = "Conformance Test"
        };
    }

    public DocumentSettings GetSettings()
    {
        return new DocumentSettings
        {
            PDFA_Conformance = PDFA_Conformance.PDFA_3A,
            PDFUA_Conformance = PDFUA_Conformance.PDFUA_1,
        };
    }

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(40);

            page.Header()
                .Column(column =>
                {
                    column.Item()
                        .SemanticHeader1()
                        .Text("Conformance Test")
                        .Style(TextStyleHeader1);
                });

            page.Content()
                .PaddingVertical(20)
                .Column(column =>
                {
                    column.Item()
                        .SemanticSection()
                        .Element(TableOfContentsSection);
                    
                    column.Item().PageBreak();
                    
                    column.Item()
                        .SemanticSection()
                        .Section("section-text")
                        .Element(TextSection);
                    
                    column.Item().PageBreak();
                    
                    column.Item()
                        .SemanticSection()
                        .Section("section-styled-box")
                        .Element(StyledBoxSection);
                    
                    column.Item().PageBreak();
                    
                    column.Item()
                        .SemanticSection()
                        .Section("section-styled-line")
                        .Element(StyledLineSection);
                    
                    column.Item().PageBreak();
                    
                    column.Item()
                        .SemanticSection()
                        .Section("section-image")
                        .Element(ImageSection);
                    
                    column.Item().PageBreak();
                    
                    column.Item()
                        .SemanticSection()
                        .Section("section-svg")
                        .Element(SvgSection);
                    
                    column.Item().PageBreak();
                    
                    column.Item()
                        .SemanticSection()
                        .Section("section-list")
                        .Element(ListSection);
                    
                    column.Item().PageBreak();
                    
                    column.Item()
                        .SemanticSection()
                        .Section("section-table-table")
                        .Element(SimpleTableSection);
                    
                    column.Item().PageBreak();
                    
                    column.Item()
                        .SemanticSection()
                        .Section("section-table-vertical-headers")
                        .Element(TableWithVerticalHeadersSection);
                    
                    column.Item().PageBreak();
                    
                    column.Item()
                        .SemanticSection()
                        .Section("section-table-horizontal-headers")
                        .Element(TableWithHorizontalHeadersSection);
                    
                    column.Item().PageBreak();
                    
                    column.Item()
                        .SemanticSection()
                        .Section("section-table-spanning-cells")
                        .Element(TableWithSpanningCellsSection);
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
    }
    
    private void TableOfContentsSection(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(6);

            column.Item()
                .SemanticHeader2()
                .Text("Table of Contents")
                .Style(TextStyleHeader2);

            column.Item()
                .SemanticTableOfContents()
                .Column(toc =>
                {
                    void TocItem(string title, string sectionId)
                    {
                        toc.Item()
                            .SemanticTableOfContentsItem()
                            .Row(row =>
                            {
                                row.RelativeItem()
                                    .SemanticLink($"Go to {title}")
                                    .Text(text =>
                                    {
                                        text.SectionLink(title, sectionId).Underline();
                                    });

                                row.ConstantItem(80)
                                    .AlignRight()
                                    .Text(text =>
                                    {
                                        text.Span("page ");
                                        text.BeginPageNumberOfSection(sectionId);
                                    });
                            });
                    }

                    TocItem("Text", "section-text");
                    TocItem("Styled Box", "section-styled-box");
                    TocItem("Styled Line", "section-styled-line");
                    TocItem("Images", "section-image");
                    TocItem("SVG Content", "section-svg");
                    TocItem("List", "section-list");
                    TocItem("Simple Table", "section-table-table");
                    TocItem("Table With Vertical Headers", "section-table-vertical-headers");
                    TocItem("Table With Horizontal Headers", "section-table-horizontal-headers");
                    TocItem("Table With Spanning Cells", "section-table-spanning-cells");
                });
        });
    }
    
    private void TextSection(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(10);
            
            column.Item()
                .SemanticHeader2()
                .Text("Text")
                .Style(TextStyleHeader2);

            column.Item()
                .SemanticParagraph()
                .Text(text =>
                {
                    text.Span("This is a semantic paragraph with ");
                    text.Span("bold").Style(TextStyle.Default.Bold());
                    text.Span(" and ");
                    text.Span("italic").Style(TextStyle.Default.Italic());
                    text.Span(" styles. ");
                    text.Span(Placeholders.Sentence());
                });

            column.Item()
                .SemanticParagraph()
                .Text(text =>
                {
                    text.Span(Placeholders.Paragraph());
                });

            column.Item()
                .SemanticHeader3()
                .Text("Multilingual and Links")
                .Style(TextStyleHeader3);

            column.Item()
                .SemanticLanguage("pl-PL")
                .SemanticParagraph()
                .Text("Zażółć gęślą jaźń");

            column.Item()
                .SemanticBlockQuotation()
                .PaddingLeft(10)
                .BorderLeft(2)
                .BorderColor(Colors.Grey.Lighten2)
                .SemanticParagraph()
                .Text(text => text.Span("\"A block-level quotation illustrating semantic tagging.\""));

            column.Item()
                .SemanticParagraph()
                .SemanticLink("QuestPDF website")
                .Text(t =>
                {
                    t.Span("Visit ");
                    t.Hyperlink("questpdf.com", "https://www.questpdf.com");
                    t.Span(" for documentation.");
                });

            column.Item()
                .SemanticParagraph()
                .Text(t =>
                {
                    t.Span("Inline code example: ");
                    t.Element(e => e.SemanticCode()
                        .Background(Colors.Grey.Lighten3)
                        .PaddingHorizontal(4)
                        .PaddingVertical(2)
                        .Text("var x = 1;")
                    );
                });
        });
    }
    
    private void StyledBoxSection(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(10);
            
            column.Item()
                .SemanticHeader2()
                .Text("Styled Box")
                .Style(TextStyleHeader2);

            column.Item()
                .SemanticDivision()
                .Background(Colors.Grey.Lighten4)
                .Border(1)
                .BorderColor(Colors.Grey.Darken2)
                .Padding(12)
                .Column(box =>
                {
                    box.Spacing(6);
                    box.Item().Text(Placeholders.Paragraph());
                    box.Item().Text(Placeholders.Sentence());
                    box.Item()
                        .SemanticCaption()
                        .Text("Figure 1. A styled division used as a callout/caption.");
                });
        });
    }
    
    private void StyledLineSection(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(10);
            
            column.Item()
                .SemanticHeader2()
                .Text("Styled Line")
                .Style(TextStyleHeader2);

            column.Item()
                .SemanticDivision()
                .Column(lines =>
                {
                    lines.Spacing(6);

                    lines.Item().SemanticCaption().Text("Thin solid line");
                    lines.Item().LineHorizontal(1).LineColor(Colors.Blue.Medium);

                    lines.Item().SemanticCaption().Text("Dashed gradient line");
                    lines.Item().LineHorizontal(3)
                        .LineDashPattern(new float[] { 3, 3 })
                        .LineGradient(new[] { Colors.Red.Medium, Colors.Orange.Medium });
                });
        });
    }
    
    private void ImageSection(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(10);
            
            column.Item()
                .SemanticHeader2()
                .Text("Images")
                .Style(TextStyleHeader2);

            var imageData = Placeholders.Image(480, 180);

            column.Item()
                .SemanticImage("A generated placeholder raster image")
                .Border(1)
                .BorderColor(Colors.Grey.Lighten1)
                .Padding(2)
                .Image(imageData)
                .FitArea();

            column.Item()
                .SemanticCaption()
                .Text("Figure 2. Placeholder image with alt text and caption.");
        });
    }
    
    private void SvgSection(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(10);
            
            column.Item()
                .SemanticHeader2()
                .Text("SVG Content")
                .Style(TextStyleHeader2);

            var svg = "<svg xmlns='http://www.w3.org/2000/svg' width='400' height='120' viewBox='0 0 400 120'>" +
                      "<rect x='5' y='5' width='390' height='110' rx='12' ry='12' fill='#E3F2FD' stroke='#64B5F6' stroke-width='2'/>" +
                      "<circle cx='80' cy='60' r='35' fill='#90CAF9' stroke='#1E88E5' stroke-width='2'/>" +
                      "<text x='140' y='68' font-family='Arial' font-size='20' fill='#0D47A1'>Scalable Vector Graphic</text>" +
                      "</svg>";

            column.Item()
                .SemanticFigure("An example SVG graphic")
                .Border(1)
                .BorderColor(Colors.Grey.Lighten1)
                .Padding(2)
                .Svg(svg)
                .FitArea();

            column.Item()
                .SemanticCaption()
                .Text("Figure 3. Simple inline SVG with a caption.");
        });
    }

    private void ListSection(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(10);
            
            column.Item()
                .SemanticHeader2()
                .Text("List")
                .Style(TextStyleHeader2);

            // Simple bulleted list with nested items and proper semantics
            column.Item()
                .SemanticList()
                .Column(list =>
                {
                    // Item 1
                    list.Item()
                        .SemanticListItem()
                        .Row(row =>
                        {
                            row.ConstantItem(16)
                                .AlignCenter()
                                .SemanticListLabel()
                                .Text("•");

                            row.RelativeItem()
                                .SemanticListItemBody()
                                .SemanticParagraph()
                                .Text(Placeholders.Sentence());
                        });

                    // Item 2 with nested list
                    list.Item()
                        .SemanticListItem()
                        .Column(item =>
                        {
                            item.Item()
                                .Row(row =>
                                {
                                    row.ConstantItem(16)
                                        .AlignCenter()
                                        .SemanticListLabel()
                                        .Text("•");

                                    row.RelativeItem()
                                        .SemanticListItemBody()
                                        .SemanticParagraph()
                                        .Text("Parent item with a nested list:");
                                });

                            // nested list
                            item.Item()
                                .PaddingLeft(20)
                                .SemanticList()
                                .Column(nested =>
                                {
                                    nested.Item()
                                        .SemanticListItem()
                                        .Row(r =>
                                        {
                                            r.ConstantItem(16).AlignCenter().SemanticListLabel().Text("–");
                                            r.RelativeItem().SemanticListItemBody().SemanticParagraph().Text(Placeholders.Sentence());
                                        });

                                    nested.Item()
                                        .SemanticListItem()
                                        .Row(r =>
                                        {
                                            r.ConstantItem(16).AlignCenter().SemanticListLabel().Text("–");
                                            r.RelativeItem().SemanticListItemBody().SemanticParagraph().Text(Placeholders.Sentence());
                                        });
                                });
                        });

                    // Item 3
                    list.Item()
                        .SemanticListItem()
                        .Row(row =>
                        {
                            row.ConstantItem(16).AlignCenter().SemanticListLabel().Text("•");
                            row.RelativeItem().SemanticListItemBody().SemanticParagraph().Text(Placeholders.Sentence());
                        });
                });
        });
    }
    
    private void SimpleTableSection(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(10);
            
            column.Item()
                .SemanticHeader2()
                .Text("Simple Table")
                .Style(TextStyleHeader2);

            column.Item()
                .SemanticTable()
                .Border(1)
                .BorderColor(Colors.Grey.Lighten2)
                .Padding(2)
                .Table(table =>
                {
                    table.ApplySemanticTags();
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(3);
                        cols.RelativeColumn(1);
                        cols.RelativeColumn(1);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Padding(6).Background(Colors.Grey.Lighten3).Text("Product").Style(TextStyle.Default.Bold());
                        header.Cell().Padding(6).Background(Colors.Grey.Lighten3).AlignRight().Text("Qty").Style(TextStyle.Default.Bold());
                        header.Cell().Padding(6).Background(Colors.Grey.Lighten3).AlignRight().Text("Price").Style(TextStyle.Default.Bold());
                    });

                    for (int i = 0; i < 6; i++)
                    {
                        table.Cell().Padding(6).Text(Placeholders.Label());
                        table.Cell().Padding(6).AlignRight().Text((i + 1).ToString());
                        table.Cell().Padding(6).AlignRight().Text(Placeholders.Price());
                    }
                });
        });
    }
    
    private void TableWithVerticalHeadersSection(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(10);
            
            column.Item()
                .SemanticHeader2()
                .Text("Table With Vertical Headers")
                .Style(TextStyleHeader2);

            column.Item()
                .SemanticTable()
                .Border(1)
                .BorderColor(Colors.Grey.Lighten2)
                .Padding(2)
                .Table(table =>
                {
                    table.ApplySemanticTags();
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(2); // row headers
                        cols.RelativeColumn(1);
                        cols.RelativeColumn(1);
                        cols.RelativeColumn(1);
                    });

                    // top header row
                    table.Header(header =>
                    {
                        header.Cell().Padding(6).Background(Colors.Grey.Lighten3).Text("");
                        header.Cell().Padding(6).Background(Colors.Grey.Lighten3).AlignCenter().Text("Q1").Style(TextStyle.Default.Bold());
                        header.Cell().Padding(6).Background(Colors.Grey.Lighten3).AlignCenter().Text("Q2").Style(TextStyle.Default.Bold());
                        header.Cell().Padding(6).Background(Colors.Grey.Lighten3).AlignCenter().Text("Q3").Style(TextStyle.Default.Bold());
                    });

                    string[] regions = { "North", "South", "East", "West" };
                    foreach (var region in regions)
                    {
                        table.Cell().AsSemanticHorizontalHeader().Padding(6).Background(Colors.Grey.Lighten4).Text(region).Style(TextStyle.Default.Bold());
                        table.Cell().Padding(6).AlignRight().Text(Placeholders.Integer());
                        table.Cell().Padding(6).AlignRight().Text(Placeholders.Integer());
                        table.Cell().Padding(6).AlignRight().Text(Placeholders.Integer());
                    }
                });
        });
    }
    
    private void TableWithHorizontalHeadersSection(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(10);
            
            column.Item()
                .SemanticHeader2()
                .Text("Table With Horizontal Headers")
                .Style(TextStyleHeader2);

            column.Item()
                .SemanticTable()
                .Border(1)
                .BorderColor(Colors.Grey.Lighten2)
                .Padding(2)
                .Table(table =>
                {
                    table.ApplySemanticTags();
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(2); // metric
                        cols.RelativeColumn(1);
                        cols.RelativeColumn(1);
                        cols.RelativeColumn(1);
                    });

                    table.Header(header =>
                    {
                        // first header row
                        header.Cell().Row(1).Column(1).ColumnSpan(4).Padding(6).Background(Colors.Grey.Lighten3).AlignCenter().Text("Quarterly Sales").Style(TextStyle.Default.Bold());
                        // second header row
                        header.Cell().Row(2).Column(1).Padding(6).Background(Colors.Grey.Lighten3).Text("Metric").Style(TextStyle.Default.Bold());
                        header.Cell().Row(2).Column(2).Padding(6).Background(Colors.Grey.Lighten3).AlignCenter().Text("Q1").Style(TextStyle.Default.Bold());
                        header.Cell().Row(2).Column(3).Padding(6).Background(Colors.Grey.Lighten3).AlignCenter().Text("Q2").Style(TextStyle.Default.Bold());
                        header.Cell().Row(2).Column(4).Padding(6).Background(Colors.Grey.Lighten3).AlignCenter().Text("Q3").Style(TextStyle.Default.Bold());
                    });

                    string[] metrics = { "Revenue", "Units", "Growth" };
                    foreach (var m in metrics)
                    {
                        table.Cell().Padding(6).Text(m);
                        table.Cell().Padding(6).AlignRight().Text(Placeholders.Integer());
                        table.Cell().Padding(6).AlignRight().Text(Placeholders.Integer());
                        table.Cell().Padding(6).AlignRight().Text(Placeholders.Integer());
                    }
                });
        });
    }
    
    private void TableWithSpanningCellsSection(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(10);
            
            column.Item()
                .SemanticHeader2()
                .Text("Table With Spanning Cells")
                .Style(TextStyleHeader2);

            column.Item()
                .SemanticTable()
                .Border(1)
                .BorderColor(Colors.Grey.Lighten2)
                .Padding(2)
                .Table(table =>
                {
                    table.ApplySemanticTags();
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(2); // Category / Row header
                        cols.RelativeColumn(1);
                        cols.RelativeColumn(1);
                        cols.RelativeColumn(1);
                    });

                    // Complex header: Category | Metrics (Min, Max) | Total
                    table.Header(header =>
                    {
                        header.Cell().Row(1).Column(1).RowSpan(2).Padding(6).Background(Colors.Grey.Lighten3).Text("Category").Style(TextStyle.Default.Bold());
                        header.Cell().Row(1).Column(2).ColumnSpan(2).Padding(6).Background(Colors.Grey.Lighten3).AlignCenter().Text("Metrics").Style(TextStyle.Default.Bold());
                        header.Cell().Row(1).Column(4).RowSpan(2).Padding(6).Background(Colors.Grey.Lighten3).AlignCenter().Text("Total").Style(TextStyle.Default.Bold());

                        header.Cell().Row(2).Column(2).Padding(6).Background(Colors.Grey.Lighten3).AlignCenter().Text("Min").Style(TextStyle.Default.Bold());
                        header.Cell().Row(2).Column(3).Padding(6).Background(Colors.Grey.Lighten3).AlignCenter().Text("Max").Style(TextStyle.Default.Bold());
                    });

                    // Body with row spans
                    string[] categories = { "Hardware", "Software" };
                    foreach (var cat in categories)
                    {
                        // Category spans two subrows
                        table.Cell().RowSpan(2).AsSemanticHorizontalHeader().Padding(6).Background(Colors.Grey.Lighten4).Text(cat).Style(TextStyle.Default.Bold());
                        // first subrow
                        table.Cell().Padding(6).AlignRight().Text(Placeholders.Integer());
                        table.Cell().Padding(6).AlignRight().Text(Placeholders.Integer());
                        table.Cell().Padding(6).AlignRight().Text(Placeholders.Integer());
                        // second subrow
                        table.Cell().Padding(6).AlignRight().Text(Placeholders.Integer());
                        table.Cell().Padding(6).AlignRight().Text(Placeholders.Integer());
                        table.Cell().Padding(6).AlignRight().Text(Placeholders.Integer());
                    }
                });
        });
    }
}

