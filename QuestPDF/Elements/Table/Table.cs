using System;
using System.Collections.Generic;
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

            CurrentRow = 1;
        }
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            UpdateColumnsWidth(availableSpace.Width);
            
            var layout = PlanLayout(availableSpace);

            return layout.MaxRowRendered < RowsCount
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
            var rowBottomOffsets = new DynamicDictionary<int, float>();
            var childrenToTry = Enumerable.Range(CurrentRow, RowsCount - CurrentRow + 1).SelectMany(x => OrderedChildren[x]);

            var currentRow = CurrentRow;
            
            foreach (var child in childrenToTry)
            {
                if (child.Row > currentRow)
                {
                    rowBottomOffsets[currentRow] = Math.Max(rowBottomOffsets[currentRow], rowBottomOffsets[currentRow - 1]);
                        
                    if (rowBottomOffsets[currentRow - 1] > availableSpace.Height + Single.Epsilon)
                        break;
                    
                    currentRow = child.Row;
                }

                var topOffset = rowBottomOffsets[child.Row - 1];
                
                var height = GetCellSize(child).Height;
                var cellBottomOffset = height + topOffset;
                
                var targetRowId = child.Row + child.RowSpan - 1; // -1 because rowSpan starts at 1
                rowBottomOffsets[targetRowId] = Math.Max(rowBottomOffsets[targetRowId], cellBottomOffset);
            }

            var rowHeights = new DynamicDictionary<int, float>();
            
            Enumerable
                .Range(CurrentRow, rowBottomOffsets.Items.Count)
                .ToList()
                .ForEach(x => rowHeights[x] = rowBottomOffsets[x] - rowBottomOffsets[x-1]);
            
            // find rows count to render in this pass
            var maxRowToDisplay = rowBottomOffsets.Items.Where(x => x.Value <= availableSpace.Height + Size.Epsilon).Max(x => x.Key);

            var totalHeight = rowHeights.Items.Where(x => x.Key <= maxRowToDisplay).Sum(x => x.Value);
            var totalWidth = Columns.Sum(x => x.Width);

            var childrenToDraw = Enumerable
                .Range(CurrentRow, maxRowToDisplay - CurrentRow + 1)
                .SelectMany(x => OrderedChildren[x]);
            
            foreach (var cell in childrenToDraw)
            {
                var leftOffset = cellOffsets[cell.Column - 1];
                var topOffset = cell.Row == 1 ? 0 : rowBottomOffsets[cell.Row - 1];

                var width = GetCellWidth(cell);
                var height = Enumerable.Range(cell.Row, cell.RowSpan).Select(x => rowHeights[x]).Sum();

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
                MaxRowRendered = maxRowToDisplay
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
    }
}