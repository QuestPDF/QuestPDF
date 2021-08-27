using QuestPDF.Elements.Text.Calculation;

namespace QuestPDF.Elements.Text.Items
{
    internal class TextLineElement
    {
        public ITextBlockElement Element { get; set; }
        public TextMeasurementResult Measurement { get; set; }
    }
}