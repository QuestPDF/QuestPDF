using QuestPDF.Infrastructure;

namespace QuestPDF.Elements.Table
{
    internal class TableCell : ContainerElement, ITableCellContainer
    {
        public int Row { get; set; } = 0;
        public int RowSpan { get; set; } = 1;

        public int Column { get; set; } = 0;
        public int ColumnSpan { get; set; } = 1;
        
        public bool IsRendered { get; set; }
    }
}