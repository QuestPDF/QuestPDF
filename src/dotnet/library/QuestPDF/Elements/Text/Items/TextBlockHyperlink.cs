using QuestPDF.Drawing;

namespace QuestPDF.Elements.Text.Items
{
    internal sealed class TextBlockHyperlink : TextBlockSpan
    {
        public string Url { get; set; }
        public int ParagraphBeginIndex { get; set; }
        public int ParagraphEndIndex { get; set; }

        // the Link element in the semantic tree of a tagged document, created when the link is drawn for the first time
        public SemanticTreeNode? SemanticTreeNode { get; set; }
    }
}