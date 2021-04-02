using System;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Section : IComponent
    {
        public Element Header { get; set; } = new Empty();
        public Element Content { get; set; } = new Empty();
        public Element Footer { get; set; } = new Empty();

        public void Compose(IContainer container)
        {
            container.Element(new Decoration
            {
                Type = DecorationType.Prepend,
                DecorationElement = Header,
                ContentElement = new Decoration
                {
                    Type = DecorationType.Append,
                    ContentElement = Content,
                    DecorationElement = Footer
                }
            });
        }
    }
}