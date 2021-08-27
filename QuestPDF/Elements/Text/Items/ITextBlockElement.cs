using QuestPDF.Elements.Text.Calculation;

namespace QuestPDF.Elements.Text.Items
{
    internal interface ITextBlockElement
    {
        TextMeasurementResult? Measure(TextMeasurementRequest request);
        void Draw(TextDrawingRequest request);
    }
}