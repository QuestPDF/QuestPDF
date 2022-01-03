using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        
        // cache for efficient cell finding
        // index of first array - number of row
        // nested array - collection of all cells starting at given row
        private TableCell[][] OrderedChildren { get; set; }
        
        private int RowsCount { get; set; }
        private int CurrentRow { get; set; }
        
        internal override void HandleVisitor(Action<Element?> visit)
        {
            Children.ToList().ForEach(x => x.HandleVisitor(visit));
            base.HandleVisitor(visit);
        }
        
        public void ResetState()
        {
            if (RowsCount == default)
                RowsCount = Children.Max(x => x.Row + x.RowSpan - 1);

            if (OrderedChildren == default)
            {
                var groups = Children
                    .GroupBy(x => x.Row)
                    .ToDictionary(x => x.Key, x => x.OrderBy(y => y.Column).ToArray());
            
                OrderedChildren = Enumerable
                    .Range(0, RowsCount + 1)
                    .Select(x => groups.TryGetValue(x, out var output) ? output : Array.Empty<TableCell>())
                    .ToArray();   
            }

            Children.ForEach(x => x.IsRendered = false);
            CurrentRow = 1;
        }
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            UpdateColumnsWidth(availableSpace.Width);
            var renderingCommands = PlanLayout(availableSpace);

            if (!renderingCommands.Any())
                return SpacePlan.Wrap();
            
            var width = Columns.Sum(x => x.Width);
            var height = renderingCommands.Max(x => x.Offset.Y + x.Size.Height);
            var tableSize = new Size(width, height);

            if (tableSize.Width > availableSpace.Width + Size.Epsilon)
                return SpacePlan.Wrap();

            return FindLastRenderedRow(renderingCommands) == RowsCount 
                ? SpacePlan.FullRender(tableSize) 
                : SpacePlan.PartialRender(tableSize);
        }

        internal override void Draw(Size availableSpace)
        {
            UpdateColumnsWidth(availableSpace.Width);
            var renderingCommands = PlanLayout(availableSpace);

            foreach (var command in renderingCommands)
            {
                if (command.Measurement.Type == SpacePlanType.FullRender)
                    command.Cell.IsRendered = true;
                
                Canvas.Translate(command.Offset);
                command.Cell.Draw(command.Size);
                Canvas.Translate(command.Offset.Reverse());
            }

            CurrentRow = FindLastRenderedRow(renderingCommands) + 1;
        }

        private static int FindLastRenderedRow(ICollection<TableCellRenderingCommand> commands)
        {
            return commands
                .GroupBy(x => x.Cell.Row)
                .Where(x => x.All(y => y.Measurement.Type == SpacePlanType.FullRender))
                .Select(x => x.Key)
                .DefaultIfEmpty(0)
                .Max();
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
        
        private ICollection<TableCellRenderingCommand> PlanLayout(Size availableSpace)
        {
            var columnOffsets = GetColumnLeftOffsets(Columns);
            
            var commands = GetRenderingCommands();
            var tableHeight = commands.Max(cell => cell.Offset.Y + cell.Size.Height);
            
            AdjustCellSizes(commands);
            AdjustLastCellSizes(tableHeight, commands);

            return commands;

            static float[] GetColumnLeftOffsets(IList<TableColumnDefinition> columns)
            {
                var cellOffsets = new float[columns.Count + 1];
                cellOffsets[0] = 0;

                foreach (var column in Enumerable.Range(1, cellOffsets.Length - 1))
                    cellOffsets[column] = columns[column - 1].Width + cellOffsets[column - 1];

                return cellOffsets;
            }
            
            ICollection<TableCellRenderingCommand> GetRenderingCommands()
            {
                var rowBottomOffsets = new DynamicDictionary<int, float>();
                
                var childrenToTry = Enumerable
                    .Range(CurrentRow, RowsCount - CurrentRow + 1)
                    .SelectMany(x => OrderedChildren[x]);
            
                var currentRow = CurrentRow;
                var maxRenderingRow = RowsCount;
                
                var commands = new List<TableCellRenderingCommand>();
                
                foreach (var cell in childrenToTry)
                {
                    // update position of previous row
                    if (cell.Row > currentRow)
                    {
                        rowBottomOffsets[currentRow] = Math.Max(rowBottomOffsets[currentRow], rowBottomOffsets[currentRow - 1]);
                            
                        if (rowBottomOffsets[currentRow - 1] > availableSpace.Height + Size.Epsilon)
                            break;
                        
                        currentRow = cell.Row;
                    }

                    // cell visibility optimizations
                    if (cell.Row > maxRenderingRow)
                        break;
                    
                    if (cell.IsRendered)
                        continue;
                    
                    // calculate cell position / size
                    var topOffset = rowBottomOffsets[cell.Row - 1];
                    
                    var availableWidth = GetCellWidth(cell);
                    var availableHeight = availableSpace.Height - topOffset + Size.Epsilon;
                    var availableCellSize = new Size(availableWidth, availableHeight);

                    var cellSize = cell.Measure(availableCellSize);

                    // corner case: if cell within the row is not fully rendered, do not attempt to render next row
                    if (cellSize.Type == SpacePlanType.PartialRender)
                    {
                        maxRenderingRow = Math.Min(maxRenderingRow, cell.Row + cell.RowSpan - 1);
                    }
                    
                    // corner case: if cell within the row want to wrap to the next page, do not attempt to render this row
                    if (cellSize.Type == SpacePlanType.Wrap)
                    {
                        maxRenderingRow = Math.Min(maxRenderingRow, cell.Row - 1);
                        continue;
                    }
                    
                    // update position of the last row that cell occupies
                    var bottomRow = cell.Row + cell.RowSpan - 1;
                    rowBottomOffsets[bottomRow] = Math.Max(rowBottomOffsets[bottomRow], topOffset + cellSize.Height);

                    // accept cell to be rendered
                    commands.Add(new TableCellRenderingCommand()
                    {
                        Cell = cell,
                        Measurement = cellSize,
                        Size = new Size(availableWidth, cellSize.Height),
                        Offset = new Position(columnOffsets[cell.Column - 1], topOffset)
                    });
                }

                // corner case: reject cell if other cells within the same row are rejected
                return commands.Where(x => x.Cell.Row <= maxRenderingRow).ToList();
            }
            
            // corner sase: if two cells end up on the same row (a.Row + a.RowSpan = b.Row + b.RowSpan),
            // bottom edges of their bounding boxes should be at the same level
            static void AdjustCellSizes(ICollection<TableCellRenderingCommand> commands)
            {
                commands
                    .GroupBy(x => x.Cell.Row + x.Cell.RowSpan)
                    .ToList()
                    .ForEach(group =>
                    {
                        var groupCells = group.ToList();
                        var bottomBorderOffset = groupCells.Max(x => x.Offset.Y + x.Size.Height);
                        groupCells.ForEach(x => x.Size = new Size(x.Size.Width, bottomBorderOffset - x.Offset.Y));
                    });
            }
            
            // corner sase: all cells, that are last ones in their respective columns, should take all remaining space
            static void AdjustLastCellSizes(float tableHeight, ICollection<TableCellRenderingCommand> commands)
            {
                var columnsCount = commands.Select(x => x.Cell).Max(x => x.Column + x.ColumnSpan - 1);
                
                foreach (var column in Enumerable.Range(1, columnsCount))
                {
                    var lastCellInColumn = commands
                        .Where(x => x.Cell.Column <= column && column < x.Cell.Column + x.Cell.ColumnSpan)
                        .OrderByDescending(x => x.Cell.Row + x.Cell.RowSpan)
                        .FirstOrDefault();
                
                    if (lastCellInColumn == null)
                        continue;
                
                    lastCellInColumn.Size = new Size(lastCellInColumn.Size.Width, tableHeight - lastCellInColumn.Offset.Y);
                }
            }

            float GetCellWidth(TableCell cell)
            {
                return columnOffsets[cell.Column + cell.ColumnSpan - 1] - columnOffsets[cell.Column - 1];
            }
        }
    }
}
