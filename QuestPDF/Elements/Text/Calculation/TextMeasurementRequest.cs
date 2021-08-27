using QuestPDF.Infrastructure;

namespace QuestPDF.Elements.Text.Calculation
{
    internal class TextMeasurementRequest
    {
        public ICanvas Canvas { get; set; }
        public IPageContext PageContext { get; set; }
        
        public int StartIndex { get; set; }
        public float AvailableWidth { get; set; }
    }
}