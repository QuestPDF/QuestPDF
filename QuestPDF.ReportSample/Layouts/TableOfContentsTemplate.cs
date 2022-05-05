using System.Collections.Generic;
using System.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.ReportSample.Layouts
{
    public class TableOfContentsTemplate : IComponent
    {
        private List<ReportSection> Sections { get; }

        public TableOfContentsTemplate(List<ReportSection> sections)
        {
            Sections = sections;
        }
        
        public void Compose(IContainer container)
        {
            container
                .Decoration(decoration =>
                {
                    decoration
                        .Before()
                        .PaddingBottom(5)
                        .Text("Table of contents")
                        .Style(Typography.Headline);

                    decoration.Content().Column(column =>
                    {
                        column.Spacing(5);
                        
                        for (var i = 0; i < Sections.Count; i++)
                            column.Item().Element(c => DrawLink(c, i+1, Sections[i].Title));

                        column.Item().Element(c => DrawLink(c, Sections.Count+1, "Photos"));
                    });
                });
        }

        private void DrawLink(IContainer container, int number, string locationName)
        {
            container
                .SectionLink(locationName)
                .Row(row =>
                {
                    row.ConstantItem(25).Text($"{number}.");
                    row.RelativeItem().Text(locationName);
                    row.ConstantItem(150).AlignRight().Text(text =>
                    {
                        text.BeginPageNumberOfSection(locationName);
                        text.Span(" - ");
                        text.EndPageNumberOfSection(locationName);

                        var lengthStyle = TextStyle.Default.FontColor(Colors.Grey.Medium);
                        
                        text.Span(" (").Style(lengthStyle);
                        text.TotalPagesWithinSection(locationName).Style(lengthStyle).Format(x => x == 1 ? "1 page long" : $"{x} pages long");
                        text.Span(")").Style(lengthStyle);
                    });
                });
        }
    }
}
