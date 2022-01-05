using System;
using System.Diagnostics;
using QuestPDF.Elements;
using QuestPDF.Elements.Table;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public class TableColumnsDefinitionDescriptor
    {
        private Table Table { get; }

        internal TableColumnsDefinitionDescriptor(Table table)
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
        private Func<IContainer, IContainer> DefaultCellStyleFunc { get; set; } = x => x;

        internal TableDescriptor(Table table)
        {
            Table = table;
        }
        
        public void ColumnsDefinition(Action<TableColumnsDefinitionDescriptor> handler)
        {
            var descriptor = new TableColumnsDefinitionDescriptor(Table);
            handler(descriptor);
        }
        
        public void ExtendLastCellsToTableBottom()
        {
            Table.ExtendLastCellsToTableBottom = true;
        }

        public void DefaultCellStyle(Func<IContainer, IContainer> mapper)
        {
            DefaultCellStyleFunc = mapper;
        }

        public ITableCellContainer Cell()
        {
            var cell = new TableCell();
            Table.Cells.Add(cell);
            return cell;
        }

        internal void ApplyDefaultCellStyle()
        {
            foreach (var cell in Table.Cells)
            {
                var container = new Container();
                DefaultCellStyleFunc(container).Element(cell.Child);
                
                cell.Child = container;
            }
        }
    }
    
    public static class TableExtensions
    {
        public static void Table(this IContainer element, Action<TableDescriptor> handler)
        {
            var table = new Table();
            
            var descriptor = new TableDescriptor(table);
            handler(descriptor);
            descriptor.ApplyDefaultCellStyle();

            table.PlanCellPositions();
            table.ValidateCellPositions();

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