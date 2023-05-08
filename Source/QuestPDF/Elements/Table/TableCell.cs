namespace QuestPDF.Elements.Table
{
    internal class TableCell : Container, ITableCellContainer
    {
        public int Row { get; set; } = 0;
        public int RowSpan { get; set; } = 1;

        public int Column { get; set; } = 0;
        public int ColumnSpan { get; set; } = 1;
        
        public int ZIndex { get; set; }
        
        public bool IsRendered { get; set; }
    }
}