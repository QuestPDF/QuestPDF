using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Decoration : IComponent
    {
        public Element Header { get; set; } = new Empty();
        public Element Content { get; set; } = new Empty();
        public Element Footer { get; set; } = new Empty();

        public void Compose(IContainer container)
        {
            container.Element(new SimpleDecoration
            {
                Type = DecorationType.Prepend,
                DecorationElement = Header,
                ContentElement = new SimpleDecoration
                {
                    Type = DecorationType.Append,
                    ContentElement = Content,
                    DecorationElement = Footer
                }
            });
        }
    }
}