using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements.Table
{
    internal class Table : Element, IStateResettable
    {
        public List<TableColumnDefinition> Columns { get; } = new List<TableColumnDefinition>();
        public List<TableCell> Children { get; } = new List<TableCell>();
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
            
            var cellOffsets = new float[Columns.Count + 1];
            cellOffsets[0] = 0;
            
            Enumerable
                .Range(1, cellOffsets.Length - 1)
                .ToList()
                .ForEach(x => cellOffsets[x] = Columns[x - 1].Width + cellOffsets[x - 1]);
            
            // update row heights
            var rowsCount = GetRowsCount();
            var rowBottomOffsets = new float[rowsCount];
            var childrenToTry = Children.Where(x => x.Row >= CurrentRow).OrderBy(x => x.Row);

            var currentRow = CurrentRow;
            
            foreach (var child in childrenToTry)
            {
                if (currentRow < child.Row)
                {
                    rowBottomOffsets[currentRow] = Math.Max(rowBottomOffsets[currentRow], rowBottomOffsets[currentRow-1]);
                    currentRow = child.Row;
                }
                
                var rowIndex = child.Row - 1;
                
                var topOffset = 0f;

                if (rowIndex > 0)
                    topOffset = rowBottomOffsets[rowIndex - 1]; // look at previous row
                
                var height = GetCellSize(child).Height;
                var cellBottomOffset = height + topOffset;
                
                var targetRowId = child.Row + child.RowSpan - 2; // -1 because indexing starts at 0, -1 because rowSpan starts at 1
                rowBottomOffsets[targetRowId] = Math.Max(rowBottomOffsets[targetRowId], cellBottomOffset);
                
                //if (targetRowId > 1 && rowBottomOffsets[targetRowId - 1] > availableSpace.Height)
                //    break;
            }
            
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
                if (cell.Row >= CurrentRow && cell.Row > rowsToDisplay)
                    continue;

                var leftOffset = cellOffsets[cell.Column - 1];
                var topOffset = cell.Row == 1 ? 0 : rowBottomOffsets[cell.Row - 2];

                var width = GetCellWidth(cell);
                var height = Enumerable.Range(cell.Row - 1, cell.RowSpan).TakeWhile(x => x < rowHeights.Length).Select(x => rowHeights[x]).Sum();

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
            
            float GetCellWidth(TableCell cell)
            {
                return cellOffsets[cell.Column + cell.ColumnSpan - 1] - cellOffsets[cell.Column - 1];
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
        }
        
        int GetRowsCount()
        {
            return Children.Max(x => x.Row + x.RowSpan);
        }
    }
}