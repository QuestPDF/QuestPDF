using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class TreeRow : Element
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