using QuestPDF.Elements.Text.Calculation;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements.Text.Items
{
    internal class TextBlockPageNumber : ITextBlockElement
    {
        public TextStyle Style { get; set; } = new TextStyle();
        public string SlotName { get; set; }
        
        public TextMeasurementResult? Measure(TextMeasurementRequest request)
        {
            return GetItem(request.PageContext).MeasureWithoutCache(request);
        }

        public void Draw(TextDrawingRequest request)
        {
            GetItem(request.PageContext).Draw(request);
        }

        private TextItem GetItem(IPageContext context)
        {
            var pageNumberPlaceholder = 123;
            
            var pageNumber = context.GetRegisteredLocations().Contains(SlotName)
                ? context.GetLocationPage(SlotName)
                : pageNumberPlaceholder;
            
            return new TextItem
            {
                Style = Style,
                Text = pageNumber.ToString()
            };
        }
    }
}