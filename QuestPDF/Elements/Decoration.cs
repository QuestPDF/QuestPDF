using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Decoration : IComponent
    {
        public Element Header { get; set; } = Empty.Instance;
        public Element Content { get; set; } = Empty.Instance;
        public Element Footer { get; set; } = Empty.Instance;

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