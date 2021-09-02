using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using IComponent = QuestPDF.Infrastructure.IComponent;
using IContainer = QuestPDF.Infrastructure.IContainer;

namespace QuestPDF.Elements
{
    internal class SimpleStack : Element, IStateResettable
    {
        internal Element First { get; set; } = Empty.Instance;
        internal Element Second { get; set; } = Empty.Instance;

        internal bool IsFirstRendered { get; set; } = false;

        internal override void HandleVisitor(Action<Element?> visit)
        {
            First?.HandleVisitor(visit);
            Second?.HandleVisitor(visit);
            
            base.HandleVisitor(visit);
        }
        
        public void ResetState()
        {
            IsFirstRendered = false;
        }

        internal override void CreateProxy(Func<Element?, Element?> create)
        {
            First = create(First);
            Second = create(Second);
        }
        
        internal override ISpacePlan Measure(Size availableSpace)
        {
            var firstElement = IsFirstRendered ? Empty.Instance : First;
            var firstSize = firstElement.Measure(availableSpace) as Size;

            if (firstSize == null)
                return new Wrap();
            
            if (firstSize is PartialRender partialRender)
                return partialRender;
                
            var spaceForSecond = new Size(availableSpace.Width, availableSpace.Height - firstSize.Height);
            var secondSize = Second.Measure(spaceForSecond) as Size;

            if (secondSize == null)
                return new PartialRender(firstSize);

            var totalWidth = Math.Max(firstSize.Width, secondSize.Width);
            var totalHeight = firstSize.Height + secondSize.Height;
            var targetSize = new Size(totalWidth, totalHeight);

            if (secondSize is PartialRender)
                return new PartialRender(targetSize);
                
            return new FullRender(targetSize);
        }

        internal override void Draw(Size availableSpace)
        {
            var firstElement = IsFirstRendered ? Empty.Instance : First;

            var firstMeasurement = firstElement.Measure(availableSpace);

            if (firstMeasurement is FullRender)
                IsFirstRendered = true;

            var firstSize = firstMeasurement as Size;

            if (firstSize != null)
                firstElement.Draw(new Size(availableSpace.Width, firstSize.Height));

            if (firstMeasurement is Wrap || firstMeasurement is PartialRender)
                return;

            var firstHeight = firstSize?.Height ?? 0;
            var spaceForSecond = new Size(availableSpace.Width, availableSpace.Height - firstHeight);
            var secondMeasurement = Second?.Measure(spaceForSecond) as Size;

            if (secondMeasurement == null)
                return;

            Canvas.Translate(new Position(0, firstHeight));
            Second.Draw(new Size(availableSpace.Width, secondMeasurement.Height));
            Canvas.Translate(new Position(0, -firstHeight));
            
            if (secondMeasurement is FullRender)
                IsFirstRendered = false;
        }
    }
    
    internal class Stack : IComponent
    {
        public ICollection<Element> Children { get; internal set; } = new List<Element>();
        public float Spacing { get; set; } = 0;
        
        public void Compose(IContainer container)
        {
            var elements = AddSpacing(Spacing, Children);

            container
                .PaddingBottom(-Spacing)    
                .Element(BuildTree(elements.ToArray()));
        }
        
        static ICollection<Element> AddSpacing(float spacing, ICollection<Element> elements)
        {
            if (spacing < Size.Epsilon)
                return elements;
                
            return elements
                .Where(x => !(x is Empty))
                .Select(x => new Padding
                {
                    Bottom = spacing,
                    Child = x
                })
                .Cast<Element>()
                .ToList();
        }

        static Element BuildTree(Span<Element> elements)
        {
            if (elements.IsEmpty)
                return Empty.Instance;

            if (elements.Length == 1)
                return elements[0];

            var half = elements.Length / 2;
                
            return new SimpleStack
            {
                First = BuildTree(elements.Slice(0, half)),
                Second = BuildTree(elements.Slice(half))
            };
        }
    }
}