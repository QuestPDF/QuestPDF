using QuestPDF.Elements.Text.Calculation;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements.Text.Items
{
    internal class TextBlockPageNumber : TextBlockSpan
    {
        public string SlotName { get; set; }
        
        public override TextMeasurementResult? Measure(TextMeasurementRequest request)
        {
            SetPageNumber(request.PageContext);
            return MeasureWithoutCache(request);
        }

        public override void Draw(TextDrawingRequest request)
        {
            SetPageNumber(request.PageContext);
            base.Draw(request);
        }

        private void SetPageNumber(IPageContext context)
        {
            var pageNumberPlaceholder = 123;
            
            var pageNumber = context.GetRegisteredLocations().Contains(SlotName)
                ? context.GetLocationPage(SlotName)
                : pageNumberPlaceholder;

            Text = pageNumber.ToString();
        }
    }
}