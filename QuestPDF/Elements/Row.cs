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
        
        internal override IEnumerable<Element?> GetChildren()
        {
            yield return Left;
            yield return Right;
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

            var rightMeasurement = Right.Measure(new Size(availableSpace.Width - leftMeasurement.Width, availableSpace.Height));

            if (rightMeasurement.Type == SpacePlanType.Wrap)
                return SpacePlan.Wrap();

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
            var leftSpace = new Size(availableSpace.Width, availableSpace.Height);
            var leftMeasurement = Left.Measure(leftSpace);

            if (leftMeasurement.Type == SpacePlanType.FullRender)
                IsLeftRendered = true;
            
            Left.Draw(new Size(leftMeasurement.Width, availableSpace.Height));

            var rightSpace = new Size(availableSpace.Width - leftMeasurement.Width, availableSpace.Height);
            var rightMeasurement = Right.Measure(rightSpace);
            
            if (rightMeasurement.Type == SpacePlanType.FullRender)
                IsRightRendered = true;
            
            Canvas.Translate(new Position(leftMeasurement.Width, 0));
            Right.Draw(rightSpace);
            Canvas.Translate(new Position(-leftMeasurement.Width, 0));
        }
    }
    
    internal class Row : Element
    {
        public float Spacing { get; set; } = 0;
        
        public ICollection<RowElement> Items { get; internal set; } = new List<RowElement>();
        private Element? RootElement { get; set; }

        internal override IEnumerable<Element?> GetChildren()
        {
            if (RootElement == null)
                ComposeTree();

            yield return RootElement;
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
            Items = AddSpacing(Items, Spacing);
            
            var elements = Items.Cast<Element>().ToArray();
            RootElement = BuildTree(elements);
        }

        private void UpdateElementsWidth(float availableWidth)
        {
            var constantWidth = Items.Sum(x => x.ConstantSize);
            var relativeWidth = Items.Sum(x => x.RelativeSize);

            var widthPerRelativeUnit = (relativeWidth > 0) ? (availableWidth - constantWidth) / relativeWidth : 0;
            
            foreach (var row in Items)
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