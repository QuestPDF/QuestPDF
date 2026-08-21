using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Drawing.DrawingCanvases;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements.Table
{
    internal sealed class Table : Element, IStateful, IContentDirectionAware, ISemanticAware
    {
        // configuration
        public List<TableColumnDefinition> Columns { get; set; } = new();
        public List<TableCell> Cells { get; set; } = new();
        public bool ExtendLastCellsToTableBottom { get; set; }
        
        public ContentDirection ContentDirection { get; set; }
        
        // cache
        private bool CacheInitialized { get; set; }
        private bool HasRelativeColumns { get; set; }
        private int LastRowIndex { get; set; }
        private int MaxRow { get; set; }
        private int MaxRowSpan { get; set; }
        
        // cache that stores all cells
        // first index: row number
        // inner table: list of all cells that ends at the corresponding row
        private TableCell[][] CellsCache { get; set; }
        
        private bool IsRendered => CurrentRow > LastRowIndex;
        
        internal override IReadOnlyList<Element?> GetChildren()
        {
            return Cells;
        }
        
        private void Initialize()
        {
            if (CacheInitialized)
                return;

            HasRelativeColumns = AnyColumnHasRelativeSize(Columns);
            LastRowIndex = CalculateLastRowIndex(Cells);
            Cells.Sort(OrderCellsByRowThenColumn);
            BuildCache();

            CacheInitialized = true;

            static bool AnyColumnHasRelativeSize(List<TableColumnDefinition> columns)
            {
                foreach (var column in columns)
                {
                    if (column.RelativeSize > 0)
                        return true;
                }

                return false;
            }

            static int CalculateLastRowIndex(List<TableCell> cells)
            {
                var lastRowIndex = 0;

                foreach (var cell in cells)
                    lastRowIndex = Math.Max(lastRowIndex, GetCellLastOccupiedRow(cell));

                return lastRowIndex;
            }
        }

        private void BuildCache()
        {
            if (CellsCache != null)
                return;

            if (Cells.Count == 0)
            {
                MaxRow = 0;
                MaxRowSpan = 1;
                CellsCache = Array.Empty<TableCell[]>();
                
                return;
            }

            UpdateMaxRowAndMaxRowSpan();
            CellsCache = GroupCellsByLastOccupiedRow();

            void UpdateMaxRowAndMaxRowSpan()
            {
                MaxRow = 0;
                MaxRowSpan = 1;

                foreach (var cell in Cells)
                {
                    MaxRow = Math.Max(MaxRow, GetCellLastOccupiedRow(cell));
                    MaxRowSpan = Math.Max(MaxRowSpan, cell.RowSpan);
                }
            }
            
            // Builds an array where the N-th element contains all cells whose last occupied row is N, ordered by column.
            TableCell[][] GroupCellsByLastOccupiedRow()
            {
                var arrayPool = ArrayPool<int>.Shared;

                // first pass: determine the size of each bucket
                var rowCellCounts = arrayPool.Rent(MaxRow + 1);
                Array.Clear(rowCellCounts, 0, MaxRow + 1);

                foreach (var cell in Cells)
                    rowCellCounts[GetCellLastOccupiedRow(cell)]++;

                var buckets = new TableCell[MaxRow + 1][];

                for (var row = 0; row <= MaxRow; row++)
                    buckets[row] = rowCellCounts[row] > 0 ? new TableCell[rowCellCounts[row]] : Array.Empty<TableCell>();

                arrayPool.Return(rowCellCounts);

                // second pass: fill the buckets
                var rowInsertionIndexes = arrayPool.Rent(MaxRow + 1);
                Array.Clear(rowInsertionIndexes, 0, MaxRow + 1);

                foreach (var cell in Cells)
                {
                    var row = GetCellLastOccupiedRow(cell);
                    buckets[row][rowInsertionIndexes[row]] = cell;
                    rowInsertionIndexes[row]++;
                }

                // final pass: order cells within each bucket by column
                foreach (var rowCells in buckets)
                {
                    if (rowCells.Length > 1)
                        Array.Sort(rowCells, OrderCellsByColumnThenRow);
                }
                
                arrayPool.Return(rowInsertionIndexes);

                return buckets;
            }
        }
        
        private static int GetCellLastOccupiedRow(TableCell cell)
        {
            return cell.Row + cell.RowSpan - 1;
        }
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            Initialize();
            
            if (Cells.Count == 0)
                return SpacePlan.Empty();
            
            if (IsRendered)
                return SpacePlan.Empty();
            
            if (HasRelativeColumns && availableSpace.Width < Size.Epsilon)
                return SpacePlan.Wrap("Insufficient space to render columns of relative size.");
            
            UpdateColumnsWidth(availableSpace.Width);
            
            using var renderingCommands = PlanLayout(availableSpace);

            if (renderingCommands.Count == 0)
                return SpacePlan.Wrap("Insufficient space to render (even partially) a single row.");
            
            var width = Columns.Sum(x => x.Width);
            var height = GetTableHeight(renderingCommands);
            var tableSize = new Size(width, height);

            if (tableSize.Width > availableSpace.Width + Size.Epsilon)
                return SpacePlan.Wrap("The content requires more horizontal space than available.");

            return CalculateCurrentRow(renderingCommands) > LastRowIndex 
                ? SpacePlan.FullRender(tableSize) 
                : SpacePlan.PartialRender(tableSize);

            static float GetTableHeight(ReusableList<TableCellRenderingCommand> commands)
            {
                var result = 0f;
                
                foreach (var command in commands)
                    result = Math.Max(result, command.Offset.Y + command.Size.Height);
                
                return result;
            }
        }

        internal override void Draw(Size availableSpace)
        {
            Initialize();
            RegisterSemanticTree();
            
            if (IsRendered)
                return;
            
            UpdateColumnsWidth(availableSpace.Width);
            
            using var renderingCommands = PlanLayout(availableSpace);
            renderingCommands.Sort(OrderCommandsByCellZIndex);

            foreach (var command in renderingCommands)
            {
                if (command.Measurement.Type is SpacePlanType.Empty or SpacePlanType.FullRender)
                    command.Cell.IsRendered = true;

                if (command.Measurement.Type == SpacePlanType.Wrap)
                    continue;
                
                var offset = ContentDirection == ContentDirection.LeftToRight
                    ? command.Offset
                    : new Position(availableSpace.Width - command.Offset.X - command.Size.Width, command.Offset.Y);
                
                Canvas.Translate(offset);
                command.Cell.Draw(command.Size);
                Canvas.Translate(offset.Reverse());
            }

            CurrentRow = CalculateCurrentRow(renderingCommands);
        }

        private int CalculateCurrentRow(List<TableCellRenderingCommand> commands)
        {
            if (commands.Count == 0)
                return CurrentRow;

            // Advance past every row whose cells all finished on this page (FullRender or Empty).
            // Stop at the first row that still has a cell needing more space — this also prevents
            // a spanning cell with FullRender measurement from hiding wrapped cells in its span.
            CalculateCurrentRow_DoneCells_Cache.Clear();

            foreach (var command in commands)
            {
                if (command.Measurement.Type is SpacePlanType.Empty or SpacePlanType.FullRender)
                    CalculateCurrentRow_DoneCells_Cache.Add(command.Cell);
            }

            var nextRow = CurrentRow;
            
            while (nextRow <= MaxRow && IsRowFullyRendered(nextRow))
                nextRow++;

            return nextRow;

            bool IsRowFullyRendered(int row)
            {
                foreach (var cell in CellsCache[row])
                {
                    if (!cell.IsRendered && !CalculateCurrentRow_DoneCells_Cache.Contains(cell))
                        return false;
                }

                return true;
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
        
        private ReusableList<TableCellRenderingCommand> PlanLayout(Size availableSpace)
        {
            var commands = ReusableListPool<TableCellRenderingCommand>.Get();
            
            var columnOffsets = GetColumnLeftOffsets(Columns);
            var rowBottomOffsets = new DynamicDictionary<int, float>();
            
            var currentRow = CurrentRow;
            var maxRenderingRow = LastRowIndex;

            for (var row = CurrentRow; row <= MaxRow; row++)
            {
                var rowCells = CellsCache[row];
                
                if (rowCells.Length == 0)
                    continue;
                
                // update position of previous row
                if (row > currentRow)
                {
                    rowBottomOffsets[currentRow] = Math.Max(rowBottomOffsets[currentRow], rowBottomOffsets[currentRow - 1]);
                    
                    if (rowBottomOffsets[currentRow - 1] > availableSpace.Height + Size.Epsilon)
                        break;

                    for (var gapRow = currentRow + 1; gapRow < row; gapRow++)
                        rowBottomOffsets[gapRow] = Math.Max(rowBottomOffsets[gapRow - 1], rowBottomOffsets[gapRow]);
                    
                    currentRow = row;
                }
                
                // cell visibility optimizations
                if (row > maxRenderingRow + MaxRowSpan)
                    break;
                
                for (var i = 0; i < rowCells.Length; i++)
                {
                    var cell = rowCells[i];
                    
                    // calculate cell position / size
                    var topOffset = rowBottomOffsets[cell.Row - 1];
                    
                    var availableWidth = GetCellWidth(cell);
                    var availableHeight = availableSpace.Height - topOffset;
                    var availableCellSize = new Size(availableWidth, availableHeight);
                    
                    var cellSize = cell.Measure(availableCellSize);
                    
                    // corner case: if cell within the row is not fully rendered, do not attempt to render next row
                    if (cellSize.Type == SpacePlanType.PartialRender)
                        maxRenderingRow = Math.Min(maxRenderingRow, cell.Row + cell.RowSpan - 1);
                    
                    // corner case: if cell within the row want to wrap to the next page, do not attempt to render this row
                    if (cellSize.Type == SpacePlanType.Wrap)
                    {
                        maxRenderingRow = Math.Min(maxRenderingRow, cell.Row - 1);
                        continue;
                    }
                    
                    // update position of the last row that cell occupies
                    var bottomRow = cell.Row + cell.RowSpan - 1;
                    rowBottomOffsets[bottomRow] = Math.Max(rowBottomOffsets[bottomRow], topOffset + cellSize.Height);
                    
                    commands.Add(new TableCellRenderingCommand()
                    {
                        Cell = cell,
                        Measurement = cellSize,
                        Size = new Size(availableWidth, cellSize.Height),
                        Offset = new Position(columnOffsets[cell.Column - 1], topOffset)
                    });
                }
            }

            if (commands.Count == 0)
                return commands;

            var maxRow = 0;

            foreach (var command in commands)
                maxRow = Math.Max(maxRow, command.Cell.Row + command.Cell.RowSpan);

            for (var row = CurrentRow; row < maxRow; row++)
                rowBottomOffsets[row] = Math.Max(rowBottomOffsets[row - 1], rowBottomOffsets[row]);

            AdjustCellSizes(commands, rowBottomOffsets);
            
            // corner case: reject cell if other cells within the same row are rejected
            commands.RemoveAll(x => x.Cell.Row > maxRenderingRow);
            
            if (ExtendLastCellsToTableBottom)
                AdjustLastCellSizes(commands, Columns.Count);
            
            return commands;

            static float[] GetColumnLeftOffsets(IList<TableColumnDefinition> columns)
            {
                var cellOffsets = new float[columns.Count + 1];
                cellOffsets[0] = 0;

                foreach (var column in Enumerable.Range(1, cellOffsets.Length - 1))
                    cellOffsets[column] = columns[column - 1].Width + cellOffsets[column - 1];

                return cellOffsets;
            }
            
            // corner sase: if two cells end up on the same row (a.Row + a.RowSpan = b.Row + b.RowSpan),
            // bottom edges of their bounding boxes should be at the same level
            static void AdjustCellSizes(List<TableCellRenderingCommand> commands, DynamicDictionary<int, float> rowBottomOffsets)
            {
                for (var i = 0; i < commands.Count; i++)
                {
                    var command = commands[i];
                    
                    var lastRow = command.Cell.Row + command.Cell.RowSpan - 1;
                    var height = rowBottomOffsets[lastRow] - command.Offset.Y;
                    
                    command.Size = new Size(command.Size.Width, height);
                    command.Offset = new Position(command.Offset.X, rowBottomOffsets[command.Cell.Row - 1]);
                    
                    commands[i] = command;
                }
            }
            
            // corner sase: all cells, that are last ones in their respective columns, should take all remaining space
            static void AdjustLastCellSizes(List<TableCellRenderingCommand> commands, int columnsCount)
            {
                var tableHeight = 0f;
                var bottomRowPerColumn = new int[columnsCount];

                // first pass: find the bottom-most occupied row of every column
                foreach (var command in commands)
                {
                    tableHeight = Math.Max(tableHeight, command.Offset.Y + command.Size.Height);
                    
                    var cell = command.Cell;

                    for (var column = cell.Column; column < cell.Column + cell.ColumnSpan; column++)
                        bottomRowPerColumn[column - 1] = Math.Max(bottomRowPerColumn[column - 1], cell.Row + cell.RowSpan);
                }

                // second pass: stretch every cell that touches the bottom of any of its columns
                for (var i = 0; i < commands.Count; i++)
                {
                    var command = commands[i];
                    var cell = command.Cell;

                    for (var column = cell.Column; column < cell.Column + cell.ColumnSpan; column++)
                    {
                        if (bottomRowPerColumn[column - 1] != cell.Row + cell.RowSpan)
                            continue;

                        command.Size = new Size(command.Size.Width, tableHeight - command.Offset.Y);
                        commands[i] = command;
                        break;
                    }
                }
            }

            float GetCellWidth(TableCell cell)
            {
                return columnOffsets[cell.Column + cell.ColumnSpan - 1] - columnOffsets[cell.Column - 1];
            }
        }
        
        #region Helpers
        
        private HashSet<TableCell> CalculateCurrentRow_DoneCells_Cache { get; } = new();
        
        private static readonly Comparison<TableCell> OrderCellsByRowThenColumn = static (left, right) =>
        {
            if (left.Row != right.Row)
                return left.Row.CompareTo(right.Row);

            if (left.Column != right.Column)
                return left.Column.CompareTo(right.Column);

            return left.ZIndex.CompareTo(right.ZIndex);
        };

        private static readonly Comparison<TableCell> OrderCellsByColumnThenRow = static (left, right) =>
        {
            if (left.Column != right.Column)
                return left.Column.CompareTo(right.Column);

            if (left.Row != right.Row)
                return left.Row.CompareTo(right.Row);

            return left.ZIndex.CompareTo(right.ZIndex);
        };
        
        private static readonly Comparison<TableCellRenderingCommand> OrderCommandsByCellZIndex =
            static (left, right) => left.Cell.ZIndex.CompareTo(right.Cell.ZIndex);
        
        #endregion
        
        #region IStateful
        
        private int CurrentRow { get; set; }
        // state is also stored in TableCell instances
    
        public struct TableState
        {
            public bool[] CellsRenderingState;
            public int CurrentRow;
        }
        
        public void ResetState(bool hardReset = false)
        {
            foreach (var x in Cells)
                x.IsRendered = false;
            
            CurrentRow = 1;
        }

        public object GetState()
        {
            var cellsRenderingState = new bool[Cells.Count];
            
            for (var i = 0; i < Cells.Count; i++)
                cellsRenderingState[i] = Cells[i].IsRendered;
            
            return new TableState
            {
                CellsRenderingState = cellsRenderingState,
                CurrentRow = CurrentRow
            };
        }

        public void SetState(object state)
        {
            var tableState = (TableState) state;
            
            for (var i = 0; i < Cells.Count; i++)
                Cells[i].IsRendered = tableState.CellsRenderingState[i];
            
            CurrentRow = tableState.CurrentRow;
        }
    
        #endregion
        
        #region Semantic

        internal enum TablePartType
        {
            Header,
            Body,
            Footer
        }
        
        private bool IsSemanticTaggingApplied { get; set; }
        public SemanticTreeManager? SemanticTreeManager { get; set; } = new();

        internal bool TableRequiresAdvancedHeaderTagging { get; set; }
        internal TablePartType PartType { get; set; }
        public List<TableCell> HeaderCells { get; set; } = []; 

        private void RegisterSemanticTree()
        {
            if (IsSemanticTaggingApplied)
                return;
            
            if (Canvas.Is<DiscardDrawingCanvas>())
                return;

            if (SemanticTreeManager == null)
                return;
            
            if (SemanticTreeManager.IsCurrentContentArtifact())
                return;
            
            if (!Cells.Any())
                return;
            
            if (SemanticTreeManager.TryPeekStack()?.Type != "Table")
                return;

            IsSemanticTaggingApplied = true;

            var sectionSemanticTreeNode = new SemanticTreeNode
            {
                NodeId = SemanticTreeManager.GetNextNodeId(),
                Type = PartType switch
                {
                    TablePartType.Header => "THead",
                    TablePartType.Body => "TBody",
                    TablePartType.Footer => "TFoot",
                    _ => throw new ArgumentOutOfRangeException()
                }
            };

            SemanticTreeManager.AddNode(sectionSemanticTreeNode);
            SemanticTreeManager.PushOnStack(sectionSemanticTreeNode);

            foreach (var tableRow in Cells.GroupBy(x => x.Row))
            {
                var rowSemanticTreeNode = new SemanticTreeNode()
                {
                    NodeId = SemanticTreeManager.GetNextNodeId(), 
                    Type = "TR"
                };
                
                SemanticTreeManager.AddNode(rowSemanticTreeNode);
                SemanticTreeManager.PushOnStack(rowSemanticTreeNode);
                
                foreach (var tableCell in tableRow.OrderBy(x => x.Column))
                {
                    tableCell.CreateProxy(x => new SemanticTag
                    {
                        SemanticTreeManager = SemanticTreeManager,
                        Canvas = Canvas,
                        
                        TagType = "TD",
                        Child = x
                    });

                    if (tableCell.Child is not SemanticTag semanticTag)
                        continue;
                    
                    if (PartType is TablePartType.Header || tableCell.IsSemanticHorizontalHeader)
                        semanticTag.TagType = "TH";
                    
                    semanticTag.RegisterCurrentSemanticNode();
                    tableCell.SemanticNodeId = semanticTag.SemanticTreeNode!.NodeId;
                    
                    AssignCellAttributesForColumnAndRowSpans(tableCell, semanticTag);
                }
                
                SemanticTreeManager.PopStack();
            }

            SemanticTreeManager.PopStack();

            AssignCellAttributesForHeaderCellRoles();
            
            static void AssignCellAttributesForColumnAndRowSpans(TableCell tableCell, SemanticTag semanticTag)
            {
                if (tableCell.ColumnSpan > 1)
                {
                    semanticTag.SemanticTreeNode.Attributes.Add(new SemanticTreeNode.Attribute
                    {
                        Owner = "Table",
                        Name = "ColSpan",
                        Value = tableCell.ColumnSpan
                    });
                }

                if (tableCell.RowSpan > 1)
                {
                    semanticTag.SemanticTreeNode.Attributes.Add(new SemanticTreeNode.Attribute
                    {
                        Owner = "Table",
                        Name = "RowSpan",
                        Value = tableCell.RowSpan
                    });
                }
            }

            void AssignCellAttributesForHeaderCellRoles()
            {
                if (PartType is TablePartType.Footer)
                    return;

                if (TableRequiresAdvancedHeaderTagging)
                {
                    AssignCellAttributesForHeaderCellRolesOfComplexTables();
                }
                else
                {
                    AssignCellAttributesForHeaderCellRolesOfSimpleTables();
                }
            }
            
            void AssignCellAttributesForHeaderCellRolesOfSimpleTables()
            {
                foreach (var tableCell in Cells)
                {
                    if (tableCell.Child is not SemanticTag semanticTag)
                        continue;

                    if (semanticTag.TagType != "TH") 
                        continue;
                    
                    var scopeValue = (PartType is TablePartType.Header, tableCell.IsSemanticHorizontalHeader) switch
                    {
                        (true, true) => "Both",
                        (true, false) => "Column",
                        (false, true) => "Row",
                        (false, false) => null
                    };

                    if (scopeValue == null)
                        continue;
                    
                    semanticTag.SemanticTreeNode.Attributes.Add(new SemanticTreeNode.Attribute
                    {
                        Owner = "Table", 
                        Name = "Scope", 
                        Value = scopeValue
                    });
                }
            }
            
            void AssignCellAttributesForHeaderCellRolesOfComplexTables()
            {
                var semanticHorizontalHeaders = Cells
                    .Where(x => x.IsSemanticHorizontalHeader)
                    .ToList();
                
                foreach (var tableCell in Cells)
                {
                    if (tableCell.Child is not SemanticTag semanticTag)
                        continue;
                    
                    var relatedHeaders = GetRelatedHeadersFor(tableCell).ToArray();
                    
                    if (!relatedHeaders.Any())
                        continue;
                    
                    semanticTag.SemanticTreeNode!.Attributes.Add(new SemanticTreeNode.Attribute
                    {
                        Owner = "Table",
                        Name = "Headers",
                        Value = relatedHeaders
                    });
                }

                IEnumerable<int> GetRelatedHeadersFor(TableCell cell)
                {
                    var isHeader = PartType == TablePartType.Header;
                    
                    var headerCells = (isHeader ? Cells : HeaderCells).AsEnumerable();
                    
                    if (isHeader)
                        headerCells = headerCells.Where(x => x.Row < cell.Row);
                    
                    var relatedVerticalHeaders = headerCells
                        .Where(x => x.Column < cell.Column + cell.ColumnSpan && cell.Column < x.Column + x.ColumnSpan)
                        .Select(x => x.SemanticNodeId);
                    
                    if (isHeader)
                        return relatedVerticalHeaders; 
                    
                    var relatedHorizontalHeaders = semanticHorizontalHeaders
                        .Where(x => x.Column < cell.Column)
                        .Where(x => x.Row < cell.Row + cell.RowSpan && cell.Row < x.Row + x.RowSpan)
                        .Select(x => x.SemanticNodeId);
                        
                    return relatedVerticalHeaders.Concat(relatedHorizontalHeaders);
                }
            }
        }
        
        public static bool DoesTableBodyRequireExtendedHeaderTagging(ICollection<TableCell> headerCells, ICollection<TableCell> bodyCells)
        {
            return ContainsSpanningCells(headerCells) || ContainsSpanningCells(bodyCells);
                
            static bool ContainsSpanningCells(IEnumerable<TableCell> cells) =>
                cells.Any(x => x.RowSpan > 1 || x.ColumnSpan > 1);
        }
        
        #endregion
    }
}