using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements.Table
{
    /// <summary>
    /// This dictionary allows to access key that does not exist.
    /// Instead of throwing an exception, it returns a default value.
    /// </summary>
    internal class DynamicDictionary<TKey, TValue>
    {
        private TValue Default { get; }
        private IDictionary<TKey, TValue> Dictionary { get; } = new Dictionary<TKey, TValue>();

        public DynamicDictionary()
        {
            
        }
        
        public DynamicDictionary(TValue defaultValue)
        {
            Default = defaultValue;
        }
        
        public TValue this[TKey key]
        {
            get => Dictionary.TryGetValue(key, out var value) ? value : Default;
            set => Dictionary[key] = value;
        }

        public List<KeyValuePair<TKey, TValue>> Items => Dictionary.ToList();
    }
    
    internal class Table : Element, IStateResettable
    {
        public List<TableColumnDefinition> Columns { get; } = new List<TableColumnDefinition>();
        public List<TableCell> Children { get; } = new List<TableCell>();
        public float Spacing { get; set; }
        
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
            
            var renderingCommands = PlanLayout(availableSpace).ToList();

            if (!renderingCommands.Any())
                return SpacePlan.Wrap();
            
            var lastFullyRenderedRow = renderingCommands
                .GroupBy(x => x.Cell.Row)
                .Where(x => x.All(y => y.Measurement.Type == SpacePlanType.FullRender))
                .Select(x => x.Key)
                .DefaultIfEmpty(0)
                .Max();

            var width = Columns.Sum(x => x.Width);
            var height = renderingCommands.Max(x => x.Offset.Y + x.Size.Height);

            if (width > availableSpace.Width + Size.Epsilon)
                return SpacePlan.Wrap();
            
            var tableSize = new Size(width, height);

            var result = lastFullyRenderedRow == RowsCount 
                ? SpacePlan.FullRender(tableSize) 
                : SpacePlan.PartialRender(tableSize);
            
            var x = renderingCommands.Select(x => $"{x.Cell.Row} {x.Cell.Column} {x.Offset} {x.Size} {x.Measurement}");
            var res = string.Join("\n", x);
            
            Console.WriteLine($"Measure: {availableSpace} / {result}");
            return result;
        }

        internal override void Draw(Size availableSpace)
        {
            UpdateColumnsWidth(availableSpace.Width);
            var renderingCommands = PlanLayout(new Size(availableSpace.Width, availableSpace.Height + 1)).ToList();

            foreach (var command in renderingCommands)
            {
                if (command.Measurement.Type == SpacePlanType.FullRender)
                    command.Cell.IsRendered = true;
                
                Canvas.Translate(command.Offset);
                command.Cell.Draw(command.Size);
                Canvas.Translate(command.Offset.Reverse());
            }

            CurrentRow = renderingCommands
                .GroupBy(x => x.Cell.Row)
                .Where(x => x.All(y => y.Measurement.Type == SpacePlanType.FullRender))
                .Select(x => x.Key)
                .DefaultIfEmpty(0)
                .Max() + 1;

            var x = renderingCommands.Select(x => $"{x.Cell.Row} {x.Cell.Column} {x.Offset} {x.Size} {x.Measurement}");
            var res = string.Join("\n", x);
            
            var height = renderingCommands.Max(x => x.Offset.Y + x.Size.Height);
            Console.WriteLine($"Draw: {availableSpace} / height {height}");
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
            var cellOffsets = new float[Columns.Count + 1];
            cellOffsets[0] = 0;
            
            Enumerable
                .Range(1, cellOffsets.Length - 1)
                .ToList()
                .ForEach(x => cellOffsets[x] = Columns[x - 1].Width + cellOffsets[x - 1]);
            
            // update row heights
            var rowBottomOffsets = new DynamicDictionary<int, float>();
            var childrenToTry = Enumerable.Range(CurrentRow, RowsCount - CurrentRow + 1).SelectMany(x => OrderedChildren[x]);
            
            var maxRenderingRow = RowsCount;
            var currentRow = CurrentRow;

            var commands = new List<TableCellRenderingCommand>();
            
            foreach (var cell in childrenToTry)
            {
                if (cell.Row > currentRow)
                {
                    rowBottomOffsets[currentRow] = Math.Max(rowBottomOffsets[currentRow], rowBottomOffsets[currentRow - 1]);
                        
                    if (rowBottomOffsets[currentRow - 1] > availableSpace.Height + Size.Epsilon)
                        break;
                    
                    currentRow = cell.Row;
                }

                if (cell.Row > maxRenderingRow)
                    break;
                
                if (cell.IsRendered)
                    continue;
                
                var topOffset = rowBottomOffsets[cell.Row - 1];
                var availableHeight = availableSpace.Height - topOffset + Size.Epsilon;

                var cellSize = GetCellSize(cell, availableHeight);

                if (cellSize.Type == SpacePlanType.PartialRender)
                {
                    maxRenderingRow = Math.Min(maxRenderingRow, cell.Row + cell.RowSpan - 1);
                }
                
                if (cellSize.Type == SpacePlanType.Wrap)
                {
                    maxRenderingRow = Math.Min(maxRenderingRow, cell.Row - 1);
                    continue;
                }
                
                var cellBottomOffset = cellSize.Height + topOffset;
                
                var targetRowId = cell.Row + cell.RowSpan - 1; // -1 because rowSpan starts at 1
                rowBottomOffsets[targetRowId] = Math.Max(rowBottomOffsets[targetRowId], cellBottomOffset);

                var width = GetCellWidth(cell);
                
                var command = new TableCellRenderingCommand()
                {
                    Cell = cell,
                    Measurement = cellSize,
                    Size = new Size(width, cellSize.Height),
                    Offset = new Position(cellOffsets[cell.Column - 1], topOffset)
                };
                
                commands.Add(command);
            }

            var tableHeight = commands.Max(cell => cell.Offset.Y + cell.Size.Height);

            commands = commands.Where(x => x.Cell.Row <= maxRenderingRow).ToList();
            
            foreach (var column in Enumerable.Range(1, Columns.Count))
            {
                var lastCellInColumn = commands
                    .Where(x => x.Cell.Column <= column && column < x.Cell.Column + x.Cell.ColumnSpan)
                    .OrderByDescending(x => x.Cell.Row + x.Cell.RowSpan)
                    .FirstOrDefault();
                
                if (lastCellInColumn == null)
                    continue;
                
                lastCellInColumn.Size = new Size(lastCellInColumn.Size.Width, tableHeight - lastCellInColumn.Offset.Y);
            }

            foreach (var command in commands)
            {
                var height = tableHeight - command.Offset.Y;
                command.Size = new Size(command.Size.Width, height);
            }

            return commands;
            
            float GetCellWidth(TableCell cell)
            {
                return cellOffsets[cell.Column + cell.ColumnSpan - 1] - cellOffsets[cell.Column - 1];
            }
            
            SpacePlan GetCellSize(TableCell cell, float availableHeight)
            {
                var width = GetCellWidth(cell);
                var cellSize = new Size(width, availableHeight);

                return cell.Measure(cellSize);
            }
        }
    }
}