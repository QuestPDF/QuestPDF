using QuestPDF.Drawing;
using QuestPDF.Elements.Text.Calculation;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements.Text.Items
{
    internal class TextBlockElement : ITextBlockItem
    {
        public ICanvas Canvas { get; set; }
        public IPageContext PageContext { get; set; }
        
        public Element Element { get; set; } = Empty.Instance;
        
        public TextBlockSize? Measure()
        {
            Element.VisitChildren(x => (x as IStateResettable)?.ResetState());
            Element.VisitChildren(x => x.Initialize(PageContext, Canvas));

            var measurement = Element.Measure(Size.Max);

            if (measurement.Type != SpacePlanType.FullRender)
                return null;
            
            return new TextBlockSize
            {
                Width = measurement.Width,
                
                Ascent = -measurement.Height,
                Descent = 0,
                
                LineHeight = 1
            };
        }

        public void Draw(TextDrawingRequest request)
        {
            Element.VisitChildren(x => (x as IStateResettable)?.ResetState());
            Element.VisitChildren(x => x.Initialize(PageContext, Canvas));
            
            Canvas.Translate(new Position(0, request.TotalAscent));
            Element.Draw(new Size(request.TextSize.Width, -request.TotalAscent));
            Canvas.Translate(new Position(0, -request.TotalAscent));
        }
    }
}