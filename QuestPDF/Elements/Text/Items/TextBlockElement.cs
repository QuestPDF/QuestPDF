using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Elements.Text.Calculation;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements.Text.Items
{
    internal class TextBlockElement : ITextBlockItem
    {
        public Element Element { get; set; } = Empty.Instance;
        
        public TextMeasurementResult? Measure(TextMeasurementRequest request)
        {
            Element.HandleVisitor(x => (x as IStateResettable)?.ResetState());
            Element.HandleVisitor(x => x.Initialize(request.PageContext, request.Canvas));

            var measurement = Element.Measure(new Size(request.AvailableWidth, Size.Max.Height));

            if (measurement is Wrap || measurement is PartialRender)
                return null;

            var elementSize = measurement as Size;
            
            return new TextMeasurementResult
            {
                Width = elementSize.Width,
                
                Ascent = -elementSize.Height,
                Descent = 0,
                
                LineHeight = 1,
                
                StartIndex = 0,
                EndIndex = 0,
                TotalIndex = 0
            };
        }

        public void Draw(TextDrawingRequest request)
        {
            Element.HandleVisitor(x => (x as IStateResettable)?.ResetState());
            Element.HandleVisitor(x => x.Initialize(request.PageContext, request.Canvas));
            
            request.Canvas.Translate(new Position(0, request.TotalAscent));
            Element.Draw(new Size(request.TextSize.Width, -request.TotalAscent));
            request.Canvas.Translate(new Position(0, -request.TotalAscent));
        }
    }
}