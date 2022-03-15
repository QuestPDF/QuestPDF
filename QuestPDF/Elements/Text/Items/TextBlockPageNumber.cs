using System;
using QuestPDF.Elements.Text.Calculation;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements.Text.Items
{
    internal class TextBlockPageNumber : TextBlockSpan
    {
        public const string PageNumberPlaceholder = "123";
        public Func<IPageContext, string> Source { get; set; } = _ => PageNumberPlaceholder;
        
        public override TextMeasurementResult? Measure(TextMeasurementRequest request)
        {
            UpdatePageNumberText(request.PageContext);
            return MeasureWithoutCache(request);
        }

        public override void Draw(TextDrawingRequest request)
        {
            UpdatePageNumberText(request.PageContext);
            base.Draw(request);
        }

        private void UpdatePageNumberText(IPageContext context)
        {
            Text = Source(context) ?? string.Empty;
        }
    }
}