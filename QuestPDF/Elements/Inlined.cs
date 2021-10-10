using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class InlinedElement : Container
    {
        public ISpacePlan? Size { get; set; }
    }
    
    internal class Inlined : Element, IStateResettable
    {
        public List<InlinedElement> Elements { get; internal set; } = new List<InlinedElement>();
        private Queue<InlinedElement> ChildrenQueue { get; set; }

        internal HorizontalAlignment HorizontalAlignment { get; set; }
        internal VerticalAlignment BaselineAlignment { get; set; }
        
        public void ResetState()
        {
            Elements.ForEach(x => x.Size ??= x.Measure(Size.Max));
            ChildrenQueue = new Queue<InlinedElement>(Elements);
        }
        
        internal override void HandleVisitor(Action<Element?> visit)
        {
            Elements.ForEach(x => x.HandleVisitor(visit));
            base.HandleVisitor(visit);
        }

        internal override ISpacePlan Measure(Size availableSpace)
        {
            if (!ChildrenQueue.Any())
                return new FullRender(Size.Zero);
            
            var lines = Compose(availableSpace);

            if (!lines.Any())
                return new Wrap();

            var lineSizes = lines.Select(GetLineSize).ToList();
            
            var width = lineSizes.Max(x => x.Width);
            var height = lineSizes.Sum(x => x.Height);
            var targetSize = new Size(width, height);

            var isPartiallyRendered = lines.Sum(x => x.Count) != ChildrenQueue.Count;

            if (isPartiallyRendered)
                return new PartialRender(targetSize);
            
            return new FullRender(targetSize);
        }

        internal override void Draw(Size availableSpace)
        {
            var lines = Compose(availableSpace);
            var topOffset = 0f;
            
            foreach (var line in lines)
            {
                var height = line.Select(x => x.Size as Size).Where(x => x != null).Max(x => x.Height);
                DrawLine(line);

                topOffset += height;
                Canvas.Translate(new Position(0, height));
            }
            
            Canvas.Translate(new Position(0, -topOffset));
            lines.SelectMany(x => x).ToList().ForEach(x => ChildrenQueue.Dequeue());

            void DrawLine(ICollection<InlinedElement> elements)
            {
                var lineSize = GetLineSize(elements);
                
                var leftOffset = AlignOffset();
                Canvas.Translate(new Position(leftOffset, 0));
                
                foreach (var element in elements)
                {
                    var size = element.Size as Size;
                    var baselineOffset = BaselineOffset(size, lineSize.Height);
                    
                    Canvas.Translate(new Position(0, baselineOffset));
                    element.Draw(size);
                    Canvas.Translate(new Position(0, -baselineOffset));

                    leftOffset += size.Width;
                    Canvas.Translate(new Position(size.Width, 0));
                }
                
                Canvas.Translate(new Position(-leftOffset, 0));

                float AlignOffset()
                {
                    if (HorizontalAlignment == HorizontalAlignment.Left)
                        return 0;

                    var difference = availableSpace.Width - lineSize.Width;
                    
                    if (HorizontalAlignment == HorizontalAlignment.Center)
                        return difference / 2;

                    return difference;
                }
                
                float BaselineOffset(Size elementSize, float lineHeight)
                {
                    if (BaselineAlignment == VerticalAlignment.Top)
                        return 0;

                    var difference = lineHeight - elementSize.Height;
                    
                    if (BaselineAlignment == VerticalAlignment.Middle)
                        return difference / 2;

                    return difference;
                }
            }
        }

        Size GetLineSize(ICollection<InlinedElement> elements)
        {
            var sizes = elements
                .Select(x => x.Size as Size)
                .Where(x => x != null)
                .ToList();
            
            var width = sizes.Sum(x => x.Width);
            var height = sizes.Max(x => x.Height);

            return new Size(width, height);
        }
        
        // list of lines, each line is a list of elements
        private ICollection<ICollection<InlinedElement>> Compose(Size availableSize)
        {
            var queue = new Queue<InlinedElement>(ChildrenQueue);
            var result = new List<ICollection<InlinedElement>>();

            var topOffset = 0f;
            
            while (true)
            {
                var line = GetNextLine();
                
                if (!line.Any())
                    break;

                var height = line
                    .Select(x => x.Size as Size)
                    .Where(x => x != null)
                    .Max(x => x.Height);
                
                if (topOffset + height > availableSize.Height)
                    break;

                topOffset += height;
                result.Add(line);
            }

            return result;

            ICollection<InlinedElement> GetNextLine()
            {
                var result = new List<InlinedElement>();
                var leftOffset = 0f;
                
                while (true)
                {
                    if (!queue.Any())
                        break;
                    
                    var element = queue.Peek();
                    var size = element.Size as Size;
                    
                    if (size == null)
                        break;
                    
                    if (leftOffset + size.Width > availableSize.Width)
                        break;

                    queue.Dequeue();
                    leftOffset += size.Width;
                    result.Add(element);    
                }

                return result;
            }
        }
    }
}