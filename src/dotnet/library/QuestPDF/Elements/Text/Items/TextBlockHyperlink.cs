namespace QuestPDF.Elements.Text.Items
{
    internal sealed class TextBlockHyperlink : TextBlockSpan
    {
        public string Url { get; set; }
        public int ParagraphBeginIndex { get; set; }
    }
}