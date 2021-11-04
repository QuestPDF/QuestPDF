using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class InlinedElement : Container
    {
        public SpacePlan? MeasureCache { get; set; }

        internal override SpacePlan Measure(Size availableSpace)
        {
            // TODO: once element caching proxy is introduces, this can be removed
            
            MeasureCache ??= Child.Measure(Size.Max);
            return MeasureCache.Value;
        }
    }

    internal enum InlinedAlignment
    {
        Left,
        Center,
        Right,
        Justify,
        SpaceAround
    }
    
    internal class Inlined : Element, IStateResettable
    {
        public List<InlinedElement> Elements { get; internal set; } = new List<InlinedElement>();
        private Queue<InlinedElement> ChildrenQueue { get; set; }

        internal float VerticalSpacing { get; set; }
        internal float HorizontalSpacing { get; set; }
        
        internal InlinedAlignment ElementsAlignment { get; set; }
        internal VerticalAlignment BaselineAlignment { get; set; }
        
        public void ResetState()
        {
            ChildrenQueue = new Queue<InlinedElement>(Elements);
        }
        
        internal override void HandleVisitor(Action<Element?> visit)
        {
            Elements.ForEach(x => x.HandleVisitor(visit));
            base.HandleVisitor(visit);
        }

        internal override SpacePlan Measure(Size availableSpace)
        {
            if (!ChildrenQueue.Any())
                return SpacePlan.FullRender(Size.Zero);
            
            var lines = Compose(availableSpace);

            if (!lines.Any())
                return SpacePlan.Wrap();

            var lineSizes = lines
                .Select(line =>
                {
                    var size = GetLineSize(line);
                    var heightWithSpacing = size.Height + (line.Count - 1) * HorizontalSpacing;
                    return new Size(size.Width, heightWithSpacing);
                })
                .ToList();
            
            var width = lineSizes.Max(x => x.Width);
            var height = lineSizes.Sum(x => x.Height) + (lines.Count - 1) * VerticalSpacing;
            var targetSize = new Size(width, height);

            var isPartiallyRendered = lines.Sum(x => x.Count) != ChildrenQueue.Count;

            if (isPartiallyRendered)
                return SpacePlan.PartialRender(targetSize);
            
            return SpacePlan.FullRender(targetSize);
        }

        internal override void Draw(Size availableSpace)
        {
            var lines = Compose(availableSpace);
            var topOffset = 0f;
            
            foreach (var line in lines)
            {
                var height = line
                    .Select(x => x.Measure(Size.Max))
                    .Where(x => x.Type != SpacePlanType.Wrap)
                    .Max(x => x.Height);
                
                DrawLine(line);

                topOffset += height + VerticalSpacing;
                Canvas.Translate(new Position(0, height + VerticalSpacing));
            }
            
            Canvas.Translate(new Position(0, -topOffset));
            lines.SelectMany(x => x).ToList().ForEach(x => ChildrenQueue.Dequeue());

            void DrawLine(ICollection<InlinedElement> elements)
            {
                var lineSize = GetLineSize(elements);

                var elementOffset = ElementOffset();
                var leftOffset = AlignOffset();
                Canvas.Translate(new Position(leftOffset, 0));
                
                foreach (var element in elements)
                {
                    var size = element.Measure(Size.Max);
                    var baselineOffset = BaselineOffset(size, lineSize.Height);
                    
                    Canvas.Translate(new Position(0, baselineOffset));
                    element.Draw(size);
                    Canvas.Translate(new Position(0, -baselineOffset));

                    leftOffset += size.Width + elementOffset;
                    Canvas.Translate(new Position(size.Width + elementOffset, 0));
                }
                
                Canvas.Translate(new Position(-leftOffset, 0));

                float ElementOffset()
                {
                    var difference = availableSpace.Width - lineSize.Width;

                    if (elements.Count == 1)
                        return 0;

                    if (ElementsAlignment == InlinedAlignment.Justify)
                        return difference / (elements.Count - 1);
                    
                    if (ElementsAlignment == InlinedAlignment.SpaceAround)
                        return difference / (elements.Count + 1);
                    
                    return HorizontalSpacing;
                }

                float AlignOffset()
                {
                    if (ElementsAlignment == InlinedAlignment.Left)
                        return 0;
                    
                    if (ElementsAlignment == InlinedAlignment.Justify)
                        return 0;
                    
                    if (ElementsAlignment == InlinedAlignment.SpaceAround)
                        return elementOffset;

                    var difference = availableSpace.Width - lineSize.Width - (elements.Count - 1) * HorizontalSpacing;
                    
                    if (ElementsAlignment == InlinedAlignment.Center)
                        return difference / 2;

                    if (ElementsAlignment == InlinedAlignment.Right)
                        return difference;

                    return 0;
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
                .Select(x => x.Measure(Size.Max))
                .Where(x => x.Type != SpacePlanType.Wrap)
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
                    .Select(x => x.Measure(availableSize))
                    .Where(x => x.Type != SpacePlanType.Wrap)
                    .Max(x => x.Height);
                
                if (topOffset + height > availableSize.Height + Size.Epsilon)
                    break;

                topOffset += height + VerticalSpacing;
                result.Add(line);
            }

            return result;

            ICollection<InlinedElement> GetNextLine()
            {
                var result = new List<InlinedElement>();
                var leftOffset = GetInitialAlignmentOffset();
                
                while (true)
                {
                    if (!queue.Any())
                        break;
                    
                    var element = queue.Peek();
                    var size = element.Measure(Size.Max);
                    
                    if (size.Type == SpacePlanType.Wrap)
                        break;
                    
                    if (leftOffset + size.Width > availableSize.Width + Size.Epsilon)
                        break;

                    queue.Dequeue();
                    leftOffset += size.Width + HorizontalSpacing;
                    result.Add(element);    
                }

                return result;
            }

            float GetInitialAlignmentOffset()
            {
                // this method makes sure that the spacing between elements is no lesser than configured
                
                if (ElementsAlignment == InlinedAlignment.SpaceAround)
                    return HorizontalSpacing * 2;

                return 0;
            }
        }
    }
}