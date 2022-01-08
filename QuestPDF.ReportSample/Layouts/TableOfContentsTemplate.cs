using System.Collections.Generic;
using QuestPDF.Fluent;
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
                        .Header()
                        .PaddingBottom(5)
                        .Text("Table of contents", Typography.Headline);

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
                .InternalLink(locationName)
                .Row(row =>
                {
                    row.ConstantItem(25).Text($"{number}.");
                    row.RelativeItem().Text(locationName);
                    row.ConstantItem(150).AlignRight().Text(text => text.PageNumberOfLocation(locationName));
                });
        }
    }
}