using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using IComponent = QuestPDF.Infrastructure.IComponent;
using IContainer = QuestPDF.Infrastructure.IContainer;

namespace QuestPDF.Elements
{
    internal class BinaryStack : Element, IStateResettable
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
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            var firstElement = IsFirstRendered ? Empty.Instance : First;
            var firstSize = firstElement.Measure(availableSpace);

            if (firstSize.Type == SpacePlanType.Wrap)
                return SpacePlan.Wrap();
            
            if (firstSize.Type == SpacePlanType.PartialRender)
                return firstSize;
                
            var spaceForSecond = new Size(availableSpace.Width, availableSpace.Height - firstSize.Height);
            var secondSize = Second.Measure(spaceForSecond);

            if (secondSize.Type == SpacePlanType.Wrap)
                return SpacePlan.PartialRender(firstSize);

            var totalWidth = Math.Max(firstSize.Width, secondSize.Width);
            var totalHeight = firstSize.Height + secondSize.Height;
            var targetSize = new Size(totalWidth, totalHeight);

            if (secondSize.Type == SpacePlanType.PartialRender)
                return SpacePlan.PartialRender(targetSize);
                
            return SpacePlan.FullRender(targetSize);
        }

        internal override void Draw(Size availableSpace)
        {
            var firstElement = IsFirstRendered ? Empty.Instance : First;

            var firstMeasurement = firstElement.Measure(availableSpace);

            if (firstMeasurement.Type == SpacePlanType.FullRender)
                IsFirstRendered = true;

            var firstSize = firstMeasurement;

            if (firstSize.Type != SpacePlanType.Wrap)
                firstElement.Draw(new Size(availableSpace.Width, firstSize.Height));

            if (firstMeasurement.Type == SpacePlanType.Wrap || firstMeasurement.Type == SpacePlanType.PartialRender)
                return;

            var firstHeight = firstSize.Height;
            var spaceForSecond = new Size(availableSpace.Width, availableSpace.Height - firstHeight);
            var secondMeasurement = Second.Measure(spaceForSecond);

            if (secondMeasurement.Type == SpacePlanType.Wrap)
                return;

            Canvas.Translate(new Position(0, firstHeight));
            Second.Draw(new Size(availableSpace.Width, secondMeasurement.Height));
            Canvas.Translate(new Position(0, -firstHeight));
            
            if (secondMeasurement.Type == SpacePlanType.FullRender)
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
                
            return new BinaryStack
            {
                First = BuildTree(elements.Slice(0, half)),
                Second = BuildTree(elements.Slice(half))
            };
        }
    }
}