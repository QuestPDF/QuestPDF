using QuestPDF.Elements.Text.Calculation;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements.Text.Items
{
    internal class TextBlockExternalLink : ITextBlockItem
    {
        public TextStyle Style { get; set; } = new TextStyle();
        public string Text { get; set; }
        public string Url { get; set; }
        
        public TextMeasurementResult? Measure(TextMeasurementRequest request)
        {
            return GetItem().MeasureWithoutCache(request);
        }

        public void Draw(TextDrawingRequest request)
        {
            request.Canvas.Translate(new Position(0, request.TotalAscent));
            request.Canvas.DrawExternalLink(Url, new Size(request.TextSize.Width, request.TextSize.Height));
            request.Canvas.Translate(new Position(0, -request.TotalAscent));
            
            GetItem().Draw(request);
        }

        private TextBlockSpan GetItem()
        {
            return new TextBlockSpan
            {
                Style = Style,
                Text = Text
            };
        }
    }
}