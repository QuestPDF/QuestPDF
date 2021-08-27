using QuestPDF.Elements.Text.Calculation;

namespace QuestPDF.Elements.Text.Items
{
    internal class TextLineElement
    {
        public ITextBlockItem Item { get; set; }
        public TextMeasurementResult Measurement { get; set; }
    }
}