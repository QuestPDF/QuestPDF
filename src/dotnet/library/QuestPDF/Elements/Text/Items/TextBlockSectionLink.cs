using QuestPDF.Infrastructure;

namespace QuestPDF.Elements.Text.Items
{
    internal sealed class TextBlockSectionLink : TextBlockSpan
    {
        public string SectionName { get; set; }
        public int ParagraphBeginIndex { get; set; }
    }
}