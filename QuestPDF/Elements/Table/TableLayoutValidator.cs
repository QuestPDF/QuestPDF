using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Fluent;

namespace QuestPDF.Elements.Table
{
    static class TableLayoutPlanner
    {
        public static void ValidateCellPositions(this Table table)
        {
            ValidateCellPositions(table.Columns.Count, table.Cells);
        }
        
        private static void ValidateCellPositions(int columnsCount, ICollection<TableCell> cells)
        {
            foreach (var cell in cells)
            {
                if (cell.Column > columnsCount)
                    throw new DocumentLayoutException($"Cell location is incorrect. Cell starts at column that does not exist. Cell details: {GetCellDetails(cell)}.");
                
                if (cell.Column + cell.ColumnSpan - 1 > columnsCount)
                    throw new DocumentLayoutException($"Cell location is incorrect. Cell spans over columns that do not exist. Cell details: {GetCellDetails(cell)}.");
            }

            string GetCellDetails(TableCell cell)
            {
                return $"Row {cell.Row}, Column {cell.Column}, RowSpan {cell.RowSpan}, ColumnSpan {cell.ColumnSpan}";
            }
        }
    }
}