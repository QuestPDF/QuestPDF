using System.Collections.Generic;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements.Table
{
    internal class TableRenderingPlan
    {
        public Size Size { get; set; }
        public List<TableCellRenderingCommand> CellRenderingCommands { get; set; }
        public int MaxRowRendered { get; set; }
    }
}