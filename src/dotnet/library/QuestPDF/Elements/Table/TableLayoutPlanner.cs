using System;
using System.Collections.Generic;

namespace QuestPDF.Elements.Table
{
    static class TableLayoutPlanner
    {
        public static void PlanCellPositions(this Table table)
        {
            PlanCellPositions(table.Columns.Count, table.Cells);
        }

        // Cells without an explicit position flow in reading order (left to right, top to bottom):
        // each takes the first unoccupied position, searching from the most recently placed cell.
        // An explicitly positioned cell moves that starting point.
        // A cell with only one coordinate specified keeps it, while the other coordinate is determined automatically.
        private static void PlanCellPositions(int columnsCount, ICollection<TableCell> cells)
        {
            // for every column, the topmost row that automatic placement considers unoccupied;
            // index 0 is unused so that columns can be addressed with their natural 1-based numbers
            var firstFreeRow = new int[columnsCount + 1];

            for (var i = 0; i < firstFreeRow.Length; i++)
                firstFreeRow[i] = 1;

            var cursorColumn = 1;
            var cursorRow = 1;
            var zIndex = 0;

            foreach (var cell in cells)
            {
                cell.ZIndex = zIndex;
                zIndex++;

                if (cell.Column == 0 && cell.Row == 0)
                    PlaceAutomatically(cell);

                else if (cell.Row == 0)
                    FindRowForColumn(cell);

                else if (cell.Column == 0)
                    FindColumnForRow(cell);

                MarkOccupiedPosition(cell);

                cursorColumn = cell.Column;
                cursorRow = cell.Row;
            }

            return;

            // a cell with an explicit column but no row flows to the first row,
            // at or below the cursor, where all of its columns are unoccupied
            void FindRowForColumn(TableCell cell)
            {
                var row = Math.Max(cursorRow, 1);

                // out-of-range fragments are ignored; the validator reports such cells afterwards
                var firstColumn = Math.Max(cell.Column, 1);
                var lastColumn = Math.Min(cell.Column + cell.ColumnSpan - 1, columnsCount);

                for (var i = firstColumn; i <= lastColumn; i++)
                    row = Math.Max(row, firstFreeRow[i]);

                cell.Row = row;
            }

            // a cell with an explicit row but no column takes the first unoccupied column of that row;
            // when the row has no space left, the cell is anchored at the row beginning
            void FindColumnForRow(TableCell cell)
            {
                var lastFittingColumn = columnsCount - cell.ColumnSpan + 1;

                for (var column = 1; column <= lastFittingColumn; column++)
                {
                    if (IsUnoccupied(column, cell.Row, cell.ColumnSpan))
                    {
                        cell.Column = column;
                        return;
                    }
                }

                cell.Column = 1;
            }

            void PlaceAutomatically(TableCell cell)
            {
                // a cell spanning more columns than the table has can never fit;
                // leave it at the table beginning so that the validator reports a descriptive error
                if (cell.ColumnSpan > columnsCount)
                {
                    cell.Column = 1;
                    cell.Row = Math.Max(cursorRow, 1);
                    return;
                }

                var column = Math.Max(cursorColumn, 1);
                var row = Math.Max(cursorRow, 1);
                var lastFittingColumn = columnsCount - cell.ColumnSpan + 1;

                while (true)
                {
                    if (column > lastFittingColumn)
                    {
                        column = 1;
                        row++;
                        continue;
                    }

                    if (IsUnoccupied(column, row, cell.ColumnSpan))
                    {
                        cell.Column = column;
                        cell.Row = row;
                        return;
                    }

                    column++;
                }
            }

            bool IsUnoccupied(int column, int row, int columnSpan)
            {
                for (var i = column; i < column + columnSpan; i++)
                {
                    if (firstFreeRow[i] > row)
                        return false;
                }

                return true;
            }

            void MarkOccupiedPosition(TableCell cell)
            {
                // out-of-range fragments are ignored; the validator reports such cells afterwards
                var firstColumn = Math.Max(cell.Column, 1);
                var lastColumn = Math.Min(cell.Column + cell.ColumnSpan - 1, columnsCount);

                for (var i = firstColumn; i <= lastColumn; i++)
                    firstFreeRow[i] = Math.Max(firstFreeRow[i], cell.Row + cell.RowSpan);
            }
        }
    }
}
