using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Elements;
using QuestPDF.Elements.Table;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public class TableColumnsDefinitionDescriptor
    {
        internal List<TableColumnDefinition> Columns { get; } = new();
        
        public void ConstantColumn(float width, Unit unit = Unit.Point)
        {
            ComplexColumn(constantWidth: width.ToPoints(unit));
        }
        
        public void RelativeColumn(float width = 1)
        {
            ComplexColumn(relativeWidth: width);
        }
        
        private void ComplexColumn(float constantWidth = 0, float relativeWidth = 0)
        {
            var columnDefinition = new TableColumnDefinition(constantWidth, relativeWidth);
            Columns.Add(columnDefinition);
        }
    }

    public class TableCellDescriptor
    {
        private ICollection<TableCell> Cells { get; }

        internal TableCellDescriptor(ICollection<TableCell> cells)
        {
            Cells = cells;
        }
        
        public ITableCellContainer Cell()
        {
            var cell = new TableCell();
            Cells.Add(cell);
            return cell;
        }
    }
    
    public class TableDescriptor
    {
        internal List<TableColumnDefinition> Columns { get; private set; }

        private Table HeaderTable { get; } = new();
        private Table ContentTable { get; } = new();
        private Table FooterTable { get; } = new();

        public void ColumnsDefinition(Action<TableColumnsDefinitionDescriptor> handler)
        {
            var descriptor = new TableColumnsDefinitionDescriptor();
            handler(descriptor);

            HeaderTable.Columns = descriptor.Columns;
            ContentTable.Columns = descriptor.Columns;
            FooterTable.Columns = descriptor.Columns;
        }
        
        public void ExtendLastCellsToTableBottom()
        {
            ContentTable.ExtendLastCellsToTableBottom = true;
        }
        
        public void Header(Action<TableCellDescriptor> handler)
        {
            var descriptor = new TableCellDescriptor(HeaderTable.Cells);
            handler(descriptor);
        }
        
        public void Footer(Action<TableCellDescriptor> handler)
        {
            var descriptor = new TableCellDescriptor(FooterTable.Cells);
            handler(descriptor);
        }
        
        public ITableCellContainer Cell()
        {
            var cell = new TableCell();
            ContentTable.Cells.Add(cell);
            return cell;
        }

        internal IElement CreateElement()
        {
            var container = new Container();

            ConfigureTable(HeaderTable);
            ConfigureTable(ContentTable);
            ConfigureTable(FooterTable);
            
            container
                .Decoration(decoration =>
                {
                    decoration.Before().Element(HeaderTable);
                    decoration.Content().Element(ContentTable);
                    decoration.After().Element(FooterTable);
                });

            return container;
        }

        private static void ConfigureTable(Table table)
        {
            if (table.Columns.Count == 0)
                throw new DocumentComposeException($"Table should have at least one column. Please call the '{nameof(ColumnsDefinition)}' method to define columns.");
            
            table.PlanCellPositions();
            table.ValidateCellPositions();
        }
    }
    
    public static class TableExtensions
    {
        public static void Table(this IContainer element, Action<TableDescriptor> handler)
        {
            var descriptor = new TableDescriptor();
            handler(descriptor);
            element.Element(descriptor.CreateElement());
        }
    }

    public static class TableCellExtensions
    {
        private static ITableCellContainer TableCell(this ITableCellContainer element, Action<TableCell> handler)
        {
            if (element is TableCell tableCell)
                handler(tableCell);
            
            return element;
        }
        
        public static ITableCellContainer Column(this ITableCellContainer tableCellContainer, uint value)
        {
            return tableCellContainer.TableCell(x => x.Column = (int)value);
        }
        
        public static ITableCellContainer ColumnSpan(this ITableCellContainer tableCellContainer, uint value)
        {
            return tableCellContainer.TableCell(x => x.ColumnSpan = (int)value);
        }
        
        public static ITableCellContainer Row(this ITableCellContainer tableCellContainer, uint value)
        {
            return tableCellContainer.TableCell(x => x.Row = (int)value);
        }
        
        public static ITableCellContainer RowSpan(this ITableCellContainer tableCellContainer, uint value)
        {
            return tableCellContainer.TableCell(x => x.RowSpan = (int)value);
        }
    }
}