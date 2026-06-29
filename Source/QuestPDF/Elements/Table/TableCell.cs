using System.Collections.Generic;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements.Table
{
    internal sealed class TableCell : ContainerElement, ITableCellContainer
    {
        public int Row { get; set; } = 0;
        public int RowSpan { get; set; } = 1;

        public int Column { get; set; } = 0;
        public int ColumnSpan { get; set; } = 1;
        
        public int ZIndex { get; set; }
        
        public bool IsSemanticHorizontalHeader { get; set; }
        public int SemanticNodeId { get; set; }
        
        public bool IsRendered { get; set; }

        internal override string? GetCompanionHint()
        {
            var rowSpan = RowSpan > 1 ? $" (span {RowSpan})" : string.Empty;
            var columnSpan = ColumnSpan > 1 ? $" (span {ColumnSpan})" : string.Empty;
            
            return $"R={Row}{rowSpan}   C={Column}{columnSpan}";
        }
    }
}