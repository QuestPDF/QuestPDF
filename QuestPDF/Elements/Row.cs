using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal abstract class RowElement : ContainerElement
    {
        public float Width { get; set; } = 1;
        
        internal override void Draw(ICanvas canvas, Size availableSpace)
        {
            Child?.Draw(canvas, availableSpace);
        }
    }
    
    internal class ConstantRowElement : RowElement
    {
        public ConstantRowElement(float width)
        {
            Width = width;
        }
    }
    
    internal class RelativeRowElement : RowElement
    {
        public RelativeRowElement(float width)
        {
            Width = width;
        }
    }
    
    internal class SimpleRow : Element
    {
        internal Element Left { get; set; }
        internal Element Right { get; set; }

        internal override ISpacePlan Measure(Size availableSpace)
        {
            var leftMeasurement = Left.Measure(new Size(availableSpace.Width, availableSpace.Height)) as Size;
            
            if (leftMeasurement == null)
                return new Wrap();
            
            var rightMeasurement = Right.Measure(new Size(availableSpace.Width - leftMeasurement.Width, availableSpace.Height)) as Size;

            if (rightMeasurement == null)
                return new Wrap();
            
            var totalWidth = leftMeasurement.Width + rightMeasurement.Width;
            var totalHeight = Math.Max(leftMeasurement.Height, rightMeasurement.Height);

            var targetSize = new Size(totalWidth, totalHeight);

            if (leftMeasurement is PartialRender || rightMeasurement is PartialRender)
                return new PartialRender(targetSize);
            
            return new FullRender(targetSize);
        }

        internal override void Draw(ICanvas canvas, Size availableSpace)
        {
            var leftMeasurement = Left.Measure(new Size(availableSpace.Width, availableSpace.Height));
            var leftWidth = (leftMeasurement as Size)?.Width ?? 0;
            
            Left.Draw(canvas, new Size(leftWidth, availableSpace.Height));
            
            canvas.Translate(new Position(leftWidth, 0));
            Right.Draw(canvas, new Size(availableSpace.Width - leftWidth, availableSpace.Height));
            canvas.Translate(new Position(-leftWidth, 0));
        }
    }
    
    internal class Row : Element
    {
        public ICollection<RowElement> Children { get; internal set; } = new List<RowElement>();
        public float Spacing { get; set; } = 0;
        
        public Element Compose(float availableWidth)
        {
            var elements = ReduceRows(AddSpacing(Children));
            return BuildTree(elements.ToArray());

            ICollection<Element> ReduceRows(ICollection<RowElement> elements)
            {
                var constantWidth = elements
                    .Where(x => x is ConstantRowElement)
                    .Cast<ConstantRowElement>()
                    .Sum(x => x.Width);
            
                var relativeWidth = elements
                    .Where(x => x is RelativeRowElement)
                    .Cast<RelativeRowElement>()
                    .Sum(x => x.Width);

                var widthPerRelativeUnit = (availableWidth - constantWidth) / relativeWidth;
                
                return elements
                    .Select(x =>
                    {
                        if (x is RelativeRowElement r)
                            return new ConstantRowElement(r.Width * widthPerRelativeUnit)
                            {
                                Child = x.Child
                            };
                        
                        return x;
                    })
                    .Select(x => new Constrained
                    {
                        MinWidth = x.Width,
                        MaxWidth = x.Width,
                        Child = x.Child
                    })
                    .Cast<Element>()
                    .ToList();
            }
            
            ICollection<RowElement> AddSpacing(ICollection<RowElement> elements)
            {
                if (Spacing < Size.Epsilon)
                    return elements;
                
                return elements
                    .SelectMany(x => new[] { new ConstantRowElement(Spacing), x })
                    .Skip(1)
                    .ToList();
            }

            Element BuildTree(Span<Element> elements)
            {
                if (elements.IsEmpty)
                    return Empty.Instance;

                if (elements.Length == 1)
                    return elements[0];

                var half = elements.Length / 2;
                
                return new SimpleRow
                {
                    Left = BuildTree(elements.Slice(0, half)),
                    Right = BuildTree(elements.Slice(half))
                };
            }
        }

        internal override ISpacePlan Measure(Size availableSpace)
        {
            return Compose(availableSpace.Width).Measure(availableSpace);
        }

        internal override void Draw(ICanvas canvas, Size availableSpace)
        {
            Compose(availableSpace.Width).Draw(canvas, availableSpace);
        }
    }
}