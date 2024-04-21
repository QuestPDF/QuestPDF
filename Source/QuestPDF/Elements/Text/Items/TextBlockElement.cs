using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements.Text.Items
{
    internal sealed class TextBlockElement : ITextBlockItem
    {
        public Element Element { get; set; } = Empty.Instance;
        public Size ElementSize { get; set; } = Size.Zero;
        public TextInjectedElementAlignment Alignment { get; set; } = TextInjectedElementAlignment.AboveBaseline;
        public int ParagraphBlockIndex { get; set; }

        public void ConfigureElement(IPageContext pageContext, ICanvas canvas)
        {
            Element.VisitChildren(x => (x as IStateResettable)?.ResetState());
            Element.InjectDependencies(pageContext, canvas);
        }
        
        public void UpdateElementSize()
        {
            ElementSize = Element.Measure(Size.Max);
        }
    }
}