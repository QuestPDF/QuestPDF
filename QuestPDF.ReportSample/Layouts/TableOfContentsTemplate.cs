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

                    decoration.Content().Stack(stack =>
                    {
                        stack.Spacing(5);
                        
                        for (var i = 0; i < Sections.Count; i++)
                            stack.Item().Element(c => DrawLink(c, i+1, Sections[i].Title));

                        stack.Item().Element(c => DrawLink(c, Sections.Count+1, "Photos"));
                    });
                });
        }

        private void DrawLink(IContainer container, int number, string locationName)
        {
            container
                .InternalLink(locationName)
                .Row(row =>
                {
                    row.ConstantColumn(25).Text($"{number}.", Typography.Normal);
                    row.RelativeColumn().Text(locationName, Typography.Normal);
                    row.ConstantColumn(150).AlignRight().Text(text => text.PageNumberOfLocation(locationName, Typography.Normal));
                });
        }
    }
}