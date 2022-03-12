using QuestPDF.Elements.Text.Calculation;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements.Text.Items
{
    internal class TextBlockSectionlLink : TextBlockSpan
    {
        public string SectionName { get; set; }
        
        public override TextMeasurementResult? Measure(TextMeasurementRequest request)
        {
            return MeasureWithoutCache(request);
        }

        public override void Draw(TextDrawingRequest request)
        {
            request.Canvas.Translate(new Position(0, request.TotalAscent));
            request.Canvas.DrawSectionLink(SectionName, new Size(request.TextSize.Width, request.TextSize.Height));
            request.Canvas.Translate(new Position(0, -request.TotalAscent));
            
            base.Draw(request);
        }
    }
}