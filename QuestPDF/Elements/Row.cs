using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class RowElement : Constrained
    {
        public float ConstantSize { get; }
        public float RelativeSize { get; }

        public RowElement(float constantSize, float relativeSize)
        {
            ConstantSize = constantSize;
            RelativeSize = relativeSize;
        }
        
        public void SetWidth(float width)
        {
            MinWidth = width;
            MaxWidth = width;
        }
    }
    
    internal class BinaryRow : Element, ICacheable, IStateResettable
    {
        internal Element Left { get; set; }
        internal Element Right { get; set; }

        private bool IsLeftRendered { get; set; } 
        private bool IsRightRendered { get; set; } 
        
        public void ResetState()
        {
            IsLeftRendered = false;
            IsRightRendered = false;
        }
        
        internal override void HandleVisitor(Action<Element?> visit)
        {
            Left.HandleVisitor(visit);
            Right.HandleVisitor(visit);
            
            base.HandleVisitor(visit);
        }

        internal override void CreateProxy(Func<Element?, Element?> create)
        {
            Left = create(Left);
            Right = create(Right);
        }

        internal override SpacePlan Measure(Size availableSpace)
        {
            var leftMeasurement = Left.Measure(new Size(availableSpace.Width, availableSpace.Height));
            
            if (leftMeasurement.Type == SpacePlanType.Wrap)
                return SpacePlan.Wrap();

            if (leftMeasurement.Type == SpacePlanType.FullRender)
                IsLeftRendered = true;
            
            var rightMeasurement = Right.Measure(new Size(availableSpace.Width - leftMeasurement.Width, availableSpace.Height));

            if (rightMeasurement.Type == SpacePlanType.Wrap)
                return SpacePlan.Wrap();
            
            if (leftMeasurement.Type == SpacePlanType.FullRender)
                IsRightRendered = true;
            
            var totalWidth = leftMeasurement.Width + rightMeasurement.Width;
            var totalHeight = Math.Max(leftMeasurement.Height, rightMeasurement.Height);

            var targetSize = new Size(totalWidth, totalHeight);

            if ((!IsLeftRendered && leftMeasurement.Type == SpacePlanType.PartialRender) || 
                (!IsRightRendered && rightMeasurement.Type == SpacePlanType.PartialRender))
                return SpacePlan.PartialRender(targetSize);

            return SpacePlan.FullRender(targetSize);
        }

        internal override void Draw(Size availableSpace)
        {
            var leftMeasurement = Left.Measure(new Size(availableSpace.Width, availableSpace.Height));
            var leftWidth = leftMeasurement.Width;
            
            Left.Draw(new Size(leftWidth, availableSpace.Height));
            
            Canvas.Translate(new Position(leftWidth, 0));
            Right.Draw(new Size(availableSpace.Width - leftWidth, availableSpace.Height));
            Canvas.Translate(new Position(-leftWidth, 0));
        }
    }
    
    internal class Row : Element
    {
        public float Spacing { get; set; } = 0;
        
        public ICollection<RowElement> Children { get; internal set; } = new List<RowElement>();
        private Element? RootElement { get; set; }
        
        internal override void HandleVisitor(Action<Element?> visit)
        {
            if (RootElement == null)
                ComposeTree();
            
            RootElement.HandleVisitor(visit);
            base.HandleVisitor(visit);
        }

        internal override SpacePlan Measure(Size availableSpace)
        {
            UpdateElementsWidth(availableSpace.Width);
            return RootElement.Measure(availableSpace);
        }

        internal override void Draw(Size availableSpace)
        {
            UpdateElementsWidth(availableSpace.Width);
            RootElement.Draw(availableSpace);
        }
        
        #region structure
        
        private void ComposeTree()
        {
            Children = AddSpacing(Children, Spacing);
            
            var elements = Children.Cast<Element>().ToArray();
            RootElement = BuildTree(elements);
        }

        private void UpdateElementsWidth(float availableWidth)
        {
            var constantWidth = Children.Sum(x => x.ConstantSize);
            var relativeWidth = Children.Sum(x => x.RelativeSize);

            var widthPerRelativeUnit = (relativeWidth > 0) ? (availableWidth - constantWidth) / relativeWidth : 0;
            
            foreach (var row in Children)
            {
                row.SetWidth(row.ConstantSize + row.RelativeSize * widthPerRelativeUnit);
            }
        }
        
        private static ICollection<RowElement> AddSpacing(ICollection<RowElement> elements, float spacing)
        {
            if (spacing < Size.Epsilon)
                return elements;
            
            return elements
                .SelectMany(x => new[] { new RowElement(spacing, 0), x })
                .Skip(1)
                .ToList();
        }

        private static Element BuildTree(Span<Element> elements)
        {
            if (elements.IsEmpty)
                return Empty.Instance;

            if (elements.Length == 1)
                return elements[0];

            var half = elements.Length / 2;
            
            return new BinaryRow
            {
                Left = BuildTree(elements.Slice(0, half)),
                Right = BuildTree(elements.Slice(half))
            };
        }
        
        #endregion
    }
}