using QuestPDF.Elements.Text.Calculation;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements.Text.Items
{
    internal sealed class TextBlockSectionLink : TextBlockSpan
    {
        public IPageContext PageContext { get; set; }
        public string SectionName { get; set; }
        
        public override TextMeasurementResult? Measure(TextMeasurementRequest request)
        {
            return MeasureWithoutCache(request);
        }

        public override void Draw(TextDrawingRequest request)
        {
            var targetName = PageContext.GetDocumentLocationName(SectionName);
            
            request.Canvas.Translate(new Position(0, request.TotalAscent));
            request.Canvas.DrawSectionLink(targetName, new Size(request.TextSize.Width, request.TextSize.Height));
            request.Canvas.Translate(new Position(0, -request.TotalAscent));
            
            base.Draw(request);
        }
    }
}