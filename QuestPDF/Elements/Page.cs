using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Page : IComponent
    {
        public Element Header { get; set; } = Empty.Instance;
        public Element Content { get; set; } = Empty.Instance;
        public Element Footer { get; set; } = Empty.Instance;

        public void Compose(IContainer container)
        {
            container.Decoration(decoration =>
            {
                decoration.Header().Element(Header);
                decoration.Content().Extend().Element(Content);
                decoration.Footer().Element(Footer);
            });
        }
    }
}