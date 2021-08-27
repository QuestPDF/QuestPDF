using QuestPDF.Elements.Text.Calculation;

namespace QuestPDF.Elements.Text.Items
{
    internal interface ITextBlockItem
    {
        TextMeasurementResult? Measure(TextMeasurementRequest request);
        void Draw(TextDrawingRequest request);
    }
}