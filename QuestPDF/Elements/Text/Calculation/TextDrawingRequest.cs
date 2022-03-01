using QuestPDF.Infrastructure;

namespace QuestPDF.Elements.Text.Calculation
{
    internal struct TextDrawingRequest
    {
        public float TotalAscent { get; set; }
        public Size TextSize { get; set; }
    }
}