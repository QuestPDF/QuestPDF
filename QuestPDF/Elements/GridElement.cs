using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class GridElement
    {
        public int Columns { get; set; } = 1;
        public Element? Child { get; set; }
    }
}