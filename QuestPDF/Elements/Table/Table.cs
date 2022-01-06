using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements.Table
{
    internal class Table : Element, IStateResettable
    {
        public List<TableColumnDefinition> Columns { get; set; } = new List<TableColumnDefinition>();
        public List<TableCell> Cells { get; set; } = new List<TableCell>();
        public bool ExtendLastCellsToTableBottom { get; set; }
        
        // cache for efficient cell finding
        // index of first array - number of row
        // nested array - collection of all cells starting at given row
        private TableCell[][] OrderedCells { get; set; }
        
        private int StartingRowsCount { get; set; }
        private int RowsCount { get; set; }
        private int CurrentRow { get; set; }
        
        internal override IEnumerable<Element?> GetChildren()
        {
            return Cells;
        }

        public void ResetState()
        {
            if (StartingRowsCount == default)
                StartingRowsCount = Cells.Select(x => x.Row).DefaultIfEmpty(0).Max();
            
            if (RowsCount == default)
                RowsCount = Cells.Select(x => x.Row + x.RowSpan - 1).DefaultIfEmpty(0).Max();

            if (OrderedCells == default)
            {
                var groups = Cells
                    .GroupBy(x => x.Row)
                    .ToDictionary(x => x.Key, x => x.OrderBy(y => y.Column).ToArray());
            
                OrderedCells = Enumerable
                    .Range(0, RowsCount + 1)
                    .Select(x => groups.TryGetValue(x, out var output) ? output : Array.Empty<TableCell>())
                    .ToArray();   
            }

            Cells.ForEach(x => x.IsRendered = false);
            CurrentRow = 1;
        }
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            if (!Cells.Any())
                return SpacePlan.FullRender(Size.Zero);
            
            UpdateColumnsWidth(availableSpace.Width);
            var renderingCommands = PlanLayout(availableSpace);

            if (!renderingCommands.Any())
                return SpacePlan.Wrap();
            
            var width = Columns.Sum(x => x.Width);
            var height = renderingCommands.Max(x => x.Offset.Y + x.Size.Height);
            var tableSize = new Size(width, height);

            if (tableSize.Width > availableSpace.Width + Size.Epsilon)
                return SpacePlan.Wrap();

            return FindLastRenderedRow(renderingCommands) == StartingRowsCount 
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
            
            var isFullRender = FindLastRenderedRow(renderingCommands) == StartingRowsCount;
            
            if (isFullRender)
                ResetState();
        }

        private int FindLastRenderedRow(ICollection<TableCellRenderingCommand> commands)
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

            if (!commands.Any())
                return commands;
            
            var tableHeight = commands.Max(cell => cell.Offset.Y + cell.Size.Height);
            
            if (ExtendLastCellsToTableBottom)
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
                
                var cellsToTry = Enumerable
                    .Range(CurrentRow, RowsCount - CurrentRow + 1)
                    .SelectMany(x => OrderedCells[x]);
            
                var currentRow = CurrentRow;
                var maxRenderingRow = RowsCount;
                
                var commands = new List<TableCellRenderingCommand>();
                
                foreach (var cell in cellsToTry)
                {
                    // update position of previous row
                    if (cell.Row > currentRow)
                    {
                        rowBottomOffsets[currentRow] = Math.Max(rowBottomOffsets[currentRow], rowBottomOffsets[currentRow - 1]);
                            
                        if (rowBottomOffsets[currentRow - 1] > availableSpace.Height + Size.Epsilon)
                            break;

                        foreach (var row in Enumerable.Range(cell.Row, cell.Row - currentRow))
                            rowBottomOffsets[row] = Math.Max(rowBottomOffsets[row - 1], rowBottomOffsets[row]);
                        
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
                    var availableHeight = availableSpace.Height - topOffset;
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

                if (!commands.Any())
                    return commands;

                var maxRow = commands.Select(x => x.Cell).Max(x => x.Row + x.RowSpan);

                foreach (var row in Enumerable.Range(CurrentRow, maxRow - CurrentRow))
                    rowBottomOffsets[row] = Math.Max(rowBottomOffsets[row - 1], rowBottomOffsets[row]);   

                AdjustCellSizes(commands, rowBottomOffsets);
                
                // corner case: reject cell if other cells within the same row are rejected
                return commands.Where(x => x.Cell.Row <= maxRenderingRow).ToList();
            }
            
            // corner sase: if two cells end up on the same row (a.Row + a.RowSpan = b.Row + b.RowSpan),
            // bottom edges of their bounding boxes should be at the same level
            static void AdjustCellSizes(ICollection<TableCellRenderingCommand> commands, DynamicDictionary<int, float> rowBottomOffsets)
            {
                foreach (var command in commands)
                {
                    var lastRow = command.Cell.Row + command.Cell.RowSpan - 1;
                    var height = rowBottomOffsets[lastRow] - command.Offset.Y;
                    
                    command.Size = new Size(command.Size.Width, height);
                    command.Offset = new Position(command.Offset.X, rowBottomOffsets[command.Cell.Row - 1]);
                }
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
