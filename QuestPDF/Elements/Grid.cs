using System.Collections.Generic;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using static QuestPDF.Infrastructure.HorizontalAlignment;

namespace QuestPDF.Elements
{
    internal class Grid : IComponent
    {
        public const int DefaultColumnsCount = 12;
        
        public List<GridElement> Children { get; set; } = new List<GridElement>();
        public Queue<GridElement> ChildrenQueue { get; set; } = new Queue<GridElement>();
        public int ColumnsCount { get; set; } = DefaultColumnsCount;

        public HorizontalAlignment Alignment { get; set; } = Left;
        public float VerticalSpacing { get; set; } = 0;
        public float HorizontalSpacing { get; set; } = 0;
        
        public void Compose(IContainer container)
        {
            ChildrenQueue = new Queue<GridElement>(Children);
            
            container.Stack(stack =>
            {
                stack.Spacing(HorizontalSpacing);
                
                while (ChildrenQueue.Any())
                    stack.Element().Row(BuildRow);
            });
        }
        
        IEnumerable<GridElement> GetRowElements()
        {
            var rowLength = 0;
                
            while (ChildrenQueue.Any())
            {
                var element = ChildrenQueue.Peek();
                            
                if (rowLength + element.Columns > ColumnsCount)
                    break;

                rowLength += element.Columns;
                yield return ChildrenQueue.Dequeue();
            }
        }
            
        void BuildRow(RowDescriptor row)
        {
            row.Spacing(HorizontalSpacing);
                
            var elements = GetRowElements().ToList();
            var columnsWidth = elements.Sum(x => x.Columns);
            var emptySpace = ColumnsCount - columnsWidth;
            var hasEmptySpace = emptySpace >= Size.Epsilon;

            if (Alignment == Center)
                emptySpace /= 2;
            
            if (hasEmptySpace && Alignment != Left)
                row.RelativeColumn(emptySpace);
                
            elements.ForEach(x => row.RelativeColumn(x.Columns).Element(x.Child));

            if (hasEmptySpace && Alignment != Right)
                row.RelativeColumn(emptySpace);
        }
    }
}