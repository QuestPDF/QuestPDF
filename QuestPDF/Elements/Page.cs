using System;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Page : IComponent
    {
        public Element? Header { get; set; }
        public Element? Content { get; set; }
        public Element? Footer { get; set; }

        public void Compose(IContainer container)
        {
            container.Section(section =>
            {
                section.Header().Element(Header ?? new Empty());
                section.Content().Extend().Element(Content ?? new Empty());
                section.Footer().Element(Footer ?? new Empty());
            });
        }
    }
}