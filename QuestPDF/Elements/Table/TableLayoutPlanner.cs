using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Fluent;

namespace QuestPDF.Elements.Table
{
    static class TableLayoutValidator
    {
        public static void PlanCellPositions(this Table table)
        {
            PlanCellPositions(table.Columns.Count, table.Cells);
        }
        
        private static void PlanCellPositions(int columnsCount, ICollection<TableCell> cells)
        {
            var cellsWindow = new List<TableCell>();
            (int x, int y) currentLocation = (1, 1);
            
            foreach (var cell in cells)
            {
                if (cellsWindow.Count > Math.Max(columnsCount, 16))
                {
                    cellsWindow = cellsWindow
                        .Where(x => x.Row + x.RowSpan > currentLocation.y)
                        .ToList();
                }
                
                if (cell.HasLocation())
                {
                    cellsWindow.Add(cell);
                    currentLocation = (cell.Column, cell.Row);
                    continue;
                }

                foreach (var location in GenerateCoordinates(columnsCount, currentLocation))
                {
                    if (location.x + cell.ColumnSpan - 1 > columnsCount)
                        continue;
                    
                    cell.Column = location.x;
                    cell.Row = location.y;
                    
                    if (cell.CollidesWithAnyOf(cellsWindow))
                        continue;

                    cellsWindow.Add(cell);
                    currentLocation = (cell.Column, cell.Row);
                    break;
                }
            }
        }
        
        private static IEnumerable<(int x, int y)> GenerateCoordinates(int columnsCount, (int x, int y) startPosition)
        {
            if (startPosition.x > columnsCount)
                throw new ArgumentException();
            
            foreach (var x in Enumerable.Range(startPosition.x, columnsCount - startPosition.x + 1))
                yield return (x, startPosition.y);

            foreach (var y in Enumerable.Range(startPosition.y + 1, 1_000_000))
            foreach (var x in Enumerable.Range(1, columnsCount))
                yield return (x, y);
        }

        private static bool CollidesWith(this TableCell cell, TableCell neighbour)
        {
            return cell.Column < neighbour.Column + neighbour.ColumnSpan &&
                   cell.Column + cell.ColumnSpan > neighbour.Column &&
                   cell.Row < neighbour.Row + neighbour.RowSpan &&
                   cell.RowSpan + cell.Row > neighbour.Row;
        }
        
        private static bool CollidesWithAnyOf(this TableCell cell, ICollection<TableCell> neighbours)
        {
            return neighbours.Any(cell.CollidesWith);
        }

        private static bool HasLocation(this TableCell cell)
        {
            return cell.Row != 1 || cell.Column != 1;
        }
    }
}