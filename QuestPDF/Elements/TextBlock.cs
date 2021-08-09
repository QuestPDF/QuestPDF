using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class TextBlock : Element, IStateResettable
    {
        public List<Element> Children { get; set; } = new List<Element>();
        public Queue<Element> ChildrenQueue { get; set; } = new Queue<Element>();
        
        public void ResetState()
        {
            ChildrenQueue = new Queue<Element>(Children);
        }
        
        internal override void HandleVisitor(Action<Element?> visit)
        {
            Children.ForEach(x => x?.HandleVisitor(visit));
            base.HandleVisitor(visit);
        }
        
        internal override ISpacePlan Measure(Size availableSpace)
        {
            return new FullRender(availableSpace);
            
            if (!ChildrenQueue.Any())
                return new FullRender(Size.Zero);

            if (Children.Count < 50)
                return new FullRender(Size.Zero);
            
            var items = SelectItemsForCurrentLine(availableSpace);

            if (items == null)
                return new Wrap();

            var totalWidth = items.Sum(x => x.Measurement.Width);
            var totalHeight = items.Max(x => x.Measurement.Height);
            
            return new PartialRender(totalWidth, totalHeight);
            
            return new FullRender(Size.Zero);
            return CreateParent(availableSpace).Measure(availableSpace);
        }

        internal override void Draw(Size availableSpace)
        {
            while (true)
            {
                if (!ChildrenQueue.Any())
                    return;
                
                var items = SelectItemsForCurrentLine(availableSpace);
            
                if (items == null)
                    return;

                var totalWidth = items.Sum(x => x.Measurement.Width);
                var totalHeight = items.Max(x => x.Measurement.Height);
                
                var spaceBetween = (availableSpace.Width - totalWidth) / (items.Count - 1);

                var offset = items
                    .Select(x => x.Measurement)
                    .Cast<TextRender>()
                    .Where(x => x != null)
                    .Select(x => x.Ascent)
                    .Select(Math.Abs)
                    .Max();
                
                Canvas.Translate(new Position(0, offset));
                
                foreach (var item in items)
                {
                    item.Element.Draw(availableSpace);
                    Canvas.Translate(new Position(item.Measurement.Width + spaceBetween, 0));
                }
            
                Canvas.Translate(new Position(-availableSpace.Width - spaceBetween, totalHeight - offset));
            
                items.ForEach(x => ChildrenQueue.Dequeue());
            }
        }

        Container CreateParent(Size availableSpace)
        {
            var children = Children
                .Select(x => new
                {
                    Element = x,
                    Measurement = x.Measure(Size.Max) as Size
                })
                .Select(x => new GridElement()
                {
                    Child = x.Element,
                    Columns = (int)x.Measurement.Width + 1
                })
                .ToList();
            
            var grid = new Grid()
            {
                Alignment = HorizontalAlignment.Left,
                ColumnsCount = (int)availableSpace.Width,
                Children = children
            };

            var container = new Container();
            grid.Compose(container);
            container.HandleVisitor(x => x.Initialize(PageContext, Canvas));

            return container;
        }

        private List<MeasuredElement>? SelectItemsForCurrentLine(Size availableSpace)
        {
            var totalWidth = 0f;

            var items = ChildrenQueue
                .Select(x => new MeasuredElement
                {
                    Element = x,
                    Measurement = x.Measure(Size.Max) as FullRender
                })
                .TakeWhile(x =>
                {
                    if (x.Measurement == null)
                        return false;
                    
                    if (totalWidth + x.Measurement.Width > availableSpace.Width)
                        return false;

                    totalWidth += x.Measurement.Width;
                    return true;
                })
                .ToList();

            if (items.Any(x => x.Measurement == null))
                return null;
                
            if (items.Max(x => x.Measurement.Height) > availableSpace.Height)
                return null;

            return items;
        }

        private class MeasuredElement
        {
            public Element Element { get; set; }
            public FullRender? Measurement { get; set; }
        }
    }
}