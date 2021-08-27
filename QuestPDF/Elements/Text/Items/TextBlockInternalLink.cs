using QuestPDF.Elements.Text.Calculation;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements.Text.Items
{
    internal class TextBlockInternalLink : ITextBlockElement
    {
        public TextStyle Style { get; set; } = new TextStyle();
        public string Text { get; set; }
        public string LocationName { get; set; }
        
        public TextMeasurementResult? Measure(TextMeasurementRequest request)
        {
            return GetItem().MeasureWithoutCache(request);
        }

        public void Draw(TextDrawingRequest request)
        {
            request.Canvas.Translate(new Position(0, request.TotalAscent));
            request.Canvas.DrawLocationLink(LocationName, new Size(request.TextSize.Width, request.TextSize.Height));
            request.Canvas.Translate(new Position(0, -request.TotalAscent));
            
            GetItem().Draw(request);
        }

        private TextItem GetItem()
        {
            return new TextItem
            {
                Style = Style,
                Text = Text
            };
        }
    }
}