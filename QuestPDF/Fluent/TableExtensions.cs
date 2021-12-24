using System;
using System.Diagnostics;
using QuestPDF.Elements;
using QuestPDF.Elements.Table;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public class TableDefinitionDescriptor
    {
        private Table Table { get; }

        internal TableDefinitionDescriptor(Table table)
        {
            Table = table;
        }
        
        public void ConstantColumn(float width)
        {
            ComplexColumn(constantWidth: width);
        }
        
        public void RelativeColumn(float width = 1)
        {
            ComplexColumn(relativeWidth: width);
        }
        
        public void ComplexColumn(float constantWidth = 0, float relativeWidth = 0)
        {
            var columnDefinition = new TableColumnDefinition(constantWidth, relativeWidth);
            Table.Columns.Add(columnDefinition);
        }
    }

    public class TableDescriptor
    {
        private Table Table { get; }

        internal TableDescriptor(Table table)
        {
            Table = table;
        }
        
        public void ColumnsDefinition(Action<TableDefinitionDescriptor> handler)
        {
            var descriptor = new TableDefinitionDescriptor(Table);
            handler(descriptor);
        }
        
        public void Spacing(float value)
        {
            Table.Spacing = value;
        }

        public ITableCellContainer Cell(int row = 1, int column = 1, int rowSpan = 1, int columnsSpan = 1)
        {
            var cell = new TableCell();
            Table.Children.Add(cell);
            return cell;
        }
    }
    
    public static class TableExtensions
    {
        public static void Table(this IContainer element, Action<TableDescriptor> handler)
        {
            var table = new Table();
            var descriptor = new TableDescriptor(table);
        
            handler(descriptor);
            table.PlanCellPositions();
            
            element.Element(table);
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