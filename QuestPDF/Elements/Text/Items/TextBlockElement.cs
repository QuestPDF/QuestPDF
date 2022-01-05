using QuestPDF.Drawing;
using QuestPDF.Elements.Text.Calculation;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements.Text.Items
{
    internal class TextBlockElement : ITextBlockItem
    {
        public Element Element { get; set; } = Empty.Instance;
        
        public TextMeasurementResult? Measure(TextMeasurementRequest request)
        {
            Element.VisitChildren(x => (x as IStateResettable)?.ResetState());
            Element.VisitChildren(x => x.Initialize(request.PageContext, request.Canvas));

            var measurement = Element.Measure(new Size(request.AvailableWidth, Size.Max.Height));

            if (measurement.Type != SpacePlanType.FullRender)
                return null;
            
            return new TextMeasurementResult
            {
                Width = measurement.Width,
                
                Ascent = -measurement.Height,
                Descent = 0,
                
                LineHeight = 1,
                
                StartIndex = 0,
                EndIndex = 0,
                TotalIndex = 0
            };
        }

        public void Draw(TextDrawingRequest request)
        {
            Element.VisitChildren(x => (x as IStateResettable)?.ResetState());
            Element.VisitChildren(x => x.Initialize(request.PageContext, request.Canvas));
            
            request.Canvas.Translate(new Position(0, request.TotalAscent));
            Element.Draw(new Size(request.TextSize.Width, -request.TotalAscent));
            request.Canvas.Translate(new Position(0, -request.TotalAscent));
        }
    }
}