using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class TableColumnDefinition
    {
        public float ConstantSize { get;  }
        public float RelativeSize { get; }

        internal float Width { get; set; }

        public TableColumnDefinition(float constantSize, float relativeSize)
        {
            ConstantSize = constantSize;
            RelativeSize = relativeSize;
        }
    }
    
    public interface ITableCellContainer : IContainer
    {
            
    }
    
    internal class TableCell : Container, ITableCellContainer
    {
        public int Row { get; set; } = 1;
        public int RowSpan { get; set; } = 1;

        public int Column { get; set; } = 1;
        public int ColumnSpan { get; set; } = 1;
    }

    internal class TableRenderingPlan
    {
        public Size Size { get; set; }
        public List<TableCellRenderingCommand> CellRenderingCommands { get; set; }
        public int MaxRowRendered { get; set; }
    }
    
    internal class TableCellRenderingCommand
    {
        public TableCell Cell { get; set; }
        public Size Size { get; set; }
        public Position Offset { get; set; }
    }
    
    internal class Table : Element, IStateResettable
    {
        public ICollection<TableColumnDefinition> Columns { get; } = new List<TableColumnDefinition>();
        public ICollection<TableCell> Children { get; } = new List<TableCell>();
        public float Spacing { get; set; }
        
        public int CurrentRow { get; set; }
        
        internal override void HandleVisitor(Action<Element?> visit)
        {
            Children.ToList().ForEach(x => x.HandleVisitor(visit));
            base.HandleVisitor(visit);
        }
        
        public void ResetState()
        {
            CurrentRow = 1;
        }
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            UpdateColumnsWidth(availableSpace.Width);
            
            var layout = PlanLayout(availableSpace);

            return layout.MaxRowRendered < GetRowsCount() 
                ? SpacePlan.PartialRender(layout.Size) 
                : SpacePlan.FullRender(layout.Size);
        }

        internal override void Draw(Size availableSpace)
        {
            UpdateColumnsWidth(availableSpace.Width);
            var layout = PlanLayout(availableSpace);
            CurrentRow = layout.MaxRowRendered;
            
            foreach (var command in layout.CellRenderingCommands)
            {
                Canvas.Translate(command.Offset);
                command.Cell.Draw(command.Size);
                Canvas.Translate(command.Offset.Reverse());
            }
        }
        
        private void UpdateColumnsWidth(float availableWidth)
        {
            var constantWidth = Columns.Sum(x => x.ConstantSize);
            var relativeWidth = Columns.Sum(x => x.RelativeSize);

            var widthPerRelativeUnit = (relativeWidth > 0) ? (availableWidth - constantWidth) / relativeWidth : 0;
            
            foreach (var column in Columns)
            {
                column.Width = column.ConstantSize + column.RelativeSize * widthPerRelativeUnit;
            }
        }

        private TableRenderingPlan PlanLayout(Size availableSpace)
        {
            var cellRenderingCommands = new List<TableCellRenderingCommand>();
            
            // update row heights
            var rowsCount = GetRowsCount();
            var rowBottomOffsets = new float[rowsCount];
            var childrenToTry = Children.Where(x => x.Row >= CurrentRow).OrderBy(x => x.Row);
            
            foreach (var child in childrenToTry)
            {
                var rowIndex = child.Row - 1;
                
                var topOffset = 0f;

                if (rowIndex > 0)
                    topOffset = rowBottomOffsets[rowIndex - 1]; // look at previous row
                
                var height = GetCellSize(child).Height;
                var cellBottomOffset = height + topOffset;
                
                var targetRowId = child.Row + child.RowSpan - 2; // -1 because indexing starts at 0, -1 because rowSpan starts at 1
                rowBottomOffsets[targetRowId] = Math.Max(rowBottomOffsets[targetRowId], cellBottomOffset);
            }
            
            Enumerable
                .Range(1, rowsCount - 1)
                .ToList()
                .ForEach(x => rowBottomOffsets[x] = Math.Max(rowBottomOffsets[x], rowBottomOffsets[x-1]));
            
            var rowHeights = new float[rowsCount];
            rowHeights[0] = rowBottomOffsets[0];
            
            Enumerable
                .Range(1, rowsCount - 1)
                .ToList()
                .ForEach(x => rowHeights[x] = rowBottomOffsets[x] - rowBottomOffsets[x-1]);
            
            // find rows count to render in this pass
            var rowsToDisplay = rowHeights.Scan((x, y) => x + y).Count(x => x <= availableSpace.Height + Size.Epsilon);
            rowHeights = rowHeights.Take(rowsToDisplay).ToArray();
            
            var totalHeight = rowHeights.Sum();
            var totalWidth = Columns.Sum(x => x.Width);

            foreach (var cell in Children)
            {
                if (!IsCellVisible(cell, rowsToDisplay))
                    continue;

                var leftOffset = GetCellWidthOffset(cell);
                var topOffset = rowHeights.Take(cell.Row - 1).Sum();

                var width = GetCellWidth(cell);
                var height = rowHeights.Skip(cell.Row - 1).Take(cell.RowSpan).Sum();

                cellRenderingCommands.Add(new TableCellRenderingCommand()
                {
                    Cell = cell,
                    Size = new Size(width, height),
                    Offset = new Position(leftOffset, topOffset)
                });
            }

            return new TableRenderingPlan
            {
                Size = new Size(totalWidth, totalHeight),
                CellRenderingCommands = cellRenderingCommands,
                MaxRowRendered = rowsToDisplay
            };
            
            float GetCellWidthOffset(TableCell cell)
            {
                return Columns.Take(cell.Column - 1).Select(x => x.Width).DefaultIfEmpty(0).Sum();
            }

            float GetCellWidth(TableCell cell)
            {
                return Columns.Skip(cell.Column - 1).Take(cell.ColumnSpan).Sum(x => x.Width);   
            }
            
            Size GetCellSize(TableCell cell)
            {
                var width = GetCellWidth(cell);
                var cellSize = new Size(width, Size.Max.Height);

                var measurement = cell.Measure(cellSize);

                if (measurement.Type == SpacePlanType.Wrap)
                    return new Size(width, Size.Infinity);

                return measurement;
            }

            bool IsCellVisible(TableCell cell, int maxRow)
            {
                return cell.Row <= maxRow;
            }
        }
        
        int GetRowsCount()
        {
            return Children.Max(x => x.Row + x.RowSpan);
        }
    }
    
    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> Scan<T>(this IEnumerable<T> input, Func<T, T, T> accumulate)
        {
            using var enumerator = input.GetEnumerator();
            
            if (!enumerator.MoveNext())
                yield break;
            
            var state = enumerator.Current;
            yield return state;
            
            while (enumerator.MoveNext())
            {
                state = accumulate(state, enumerator.Current);
                yield return state;
            }
        }
    }
}