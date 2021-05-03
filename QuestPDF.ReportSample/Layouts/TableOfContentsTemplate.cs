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
                .Decoration(section =>
                {
                    section
                        .Header()
                        .PaddingBottom(5)
                        .Text("Table of contents", Typography.Headline);

                    section.Content().Stack(stack =>
                    {
                        stack.Spacing(5);
                        
                        for (var i = 0; i < Sections.Count; i++)
                            stack.Item(c => DrawLink(c, i+1, Sections[i].Title));

                        stack.Item(c => DrawLink(c, Sections.Count+1, "Photos"));
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
                });
        }
    }
}