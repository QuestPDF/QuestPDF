using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements.Table
{
    internal class TableCellRenderingCommand
    {
        public TableCell Cell { get; set; }
        public SpacePlan Measurement { get; set; }
        public Size Size { get; set; }
        public Position Offset { get; set; }
    }
}