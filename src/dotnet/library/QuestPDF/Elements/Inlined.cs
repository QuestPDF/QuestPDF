using System;
using System.Collections.Generic;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal enum InlinedAlignment
    {
        Left,
        Center,
        Right,
        Justify,
        SpaceAround
    }

    internal readonly struct InlinedMeasurement
    {
        public Element Element { get; init; }
        public Size Size { get; init; }
        public Position Position { get; init; }
    }

    internal sealed class Inlined : Element, IContentDirectionAware, IStateful
    {
        public List<Element> Elements { get; internal set; } = new();

        public ContentDirection ContentDirection { get; set; }
        
        internal float VerticalSpacing { get; set; }
        internal float HorizontalSpacing { get; set; }
        
        internal InlinedAlignment? ElementsAlignment { get; set; }
        internal VerticalAlignment BaselineAlignment { get; set; }
        
        internal override IReadOnlyList<Element?> GetChildren()
        {
            return Elements;
        }
        
        internal override SpacePlan Measure(LayoutSpace availableSpace)
        {
            SetDefaultAlignment();
            
            if (CurrentRenderingIndex == Elements.Count)
                return SpacePlan.Empty();
            
            using var commands = Compose(availableSpace, out var contentIntrinsicSize);

            if (commands.Count == 0)
                return SpacePlan.Wrap("The available space is not sufficient to fully render even a single item.");

            var totalRenderedItems = CurrentRenderingIndex + commands.Count;
            var willBeFullyRendered = totalRenderedItems == Elements.Count;

            return willBeFullyRendered
                ? SpacePlan.FullRender(contentIntrinsicSize)
                : SpacePlan.PartialRender(contentIntrinsicSize);
        }

        internal override void Draw(LayoutSpace availableSpace)
        {
            // TODO: empty elements should not introduce spacing?
            
            SetDefaultAlignment();
            
            using var commands = Compose(availableSpace, out _);

            foreach (var command in commands)
            {
                Canvas.Translate(command.Position);
                command.Element.Draw(GetItemSpace(availableSpace, command.Size));
                Canvas.Translate(command.Position.Reverse());
            }

            CurrentRenderingIndex += commands.Count;
        }

        private void SetDefaultAlignment()
        {
            if (ElementsAlignment.HasValue)
                return;

            ElementsAlignment = ContentDirection == ContentDirection.LeftToRight
                ? InlinedAlignment.Left
                : InlinedAlignment.Right;
        }
        
        /// <summary>
        /// Every item is offered the entire line width, which never grows, so the horizontal constraint is inherited.
        /// The height is unlimited while measuring: the item is asked how tall it is, not told how tall it may be.
        /// </summary>
        private static LayoutSpace GetItemSpace(LayoutSpace availableSpace, Size itemSize)
        {
            return availableSpace.With(itemSize).WithHeightMode(LayoutAxisMode.Query);
        }
        
        private ReusableList<InlinedMeasurement> Compose(LayoutSpace availableSize, out Size contentIntrinsicSize)
        {
            var commands = ReusableList<InlinedMeasurement>.Get();

            var localRenderingIndex = CurrentRenderingIndex;
            var topOffset = 0f;

            // the content size is derived from the line metrics, never from the item positions,
            // as the latter also include the alignment offsets
            var maxLineIntrinsicWidth = 0f;
            var totalLineIntrinsicHeight = 0d; // sums accumulate in double to preserve float precision
            var lineCount = 0;

            while (true)
            {
                var lineStartIndex = commands.Count;
                var (lineWidth, lineHeight) = ComposeLine();
                var lineItemCount = commands.Count - lineStartIndex;

                if (lineItemCount == 0)
                    break;

                if (topOffset + lineHeight > availableSize.Height + Size.Epsilon)
                {
                    // the line does not fit vertically: discard its measurements
                    commands.RemoveRange(lineStartIndex, lineItemCount);
                    break;
                }

                ApplyLinePositions(lineStartIndex, lineItemCount, lineWidth, lineHeight);

                maxLineIntrinsicWidth = Math.Max(maxLineIntrinsicWidth, lineWidth + (lineItemCount - 1) * HorizontalSpacing);
                totalLineIntrinsicHeight += lineHeight;
                lineCount++;

                topOffset += lineHeight + VerticalSpacing;
            }

            contentIntrinsicSize = lineCount == 0
                ? Size.Zero
                : new Size(maxLineIntrinsicWidth, (float) totalLineIntrinsicHeight + (lineCount - 1) * VerticalSpacing);

            return commands;

            (float Width, float Height) ComposeLine()
            {
                var lineIntrinsicWidth = 0d; // sums accumulate in double to preserve float precision
                var lineIntrinsicHeight = 0f;
                var leftOffset = GetInitialAlignmentOffset();

                while (localRenderingIndex < Elements.Count)
                {
                    var element = Elements[localRenderingIndex];
                    var size = element.Measure(GetItemSpace(availableSize, new Size(availableSize.Width, Size.Max.Height)));

                    if (size.Type is SpacePlanType.PartialRender or SpacePlanType.Wrap)
                        break;

                    if (leftOffset + size.Width > availableSize.Width + Size.Epsilon)
                        break;

                    localRenderingIndex++;
                    leftOffset += size.Width + HorizontalSpacing;

                    lineIntrinsicWidth += size.Width;
                    lineIntrinsicHeight = Math.Max(lineIntrinsicHeight, size.Height);

                    commands.Add(new InlinedMeasurement
                    {
                        Element = element,
                        Size = size,
                        // the position is assigned once the entire line is measured
                    });
                }

                return ((float)lineIntrinsicWidth, lineIntrinsicHeight);
            }

            void ApplyLinePositions(int lineStartIndex, int lineItemCount, float lineWidth, float lineHeight)
            {
                var elementOffset = ElementOffset();
                var leftOffset = AlignOffset();
                
                for (var i = lineStartIndex; i < lineStartIndex + lineItemCount; i++)
                {
                    var command = commands[i];

                    var size = command.Size;
                    var baselineOffset = BaselineOffset(size.Height);

                    if (size.Height == 0)
                        size = new Size(size.Width, lineHeight);

                    var position = ContentDirection == ContentDirection.LeftToRight
                        ? new Position(leftOffset, topOffset + baselineOffset)
                        : new Position(availableSize.Width - size.Width - leftOffset, topOffset + baselineOffset);

                    commands[i] = new InlinedMeasurement
                    {
                        Element = command.Element,
                        Size = size,
                        Position = position
                    };

                    leftOffset += size.Width + elementOffset;
                }

                float ElementOffset()
                {
                    var difference = availableSize.Width - lineWidth;

                    if (lineItemCount == 1)
                        return 0;

                    return ElementsAlignment switch
                    {
                        InlinedAlignment.Justify => difference / (lineItemCount - 1),
                        InlinedAlignment.SpaceAround => difference / (lineItemCount + 1),
                        _ => HorizontalSpacing
                    };
                }

                float AlignOffset()
                {
                    var emptySpace = availableSize.Width - lineWidth - (lineItemCount - 1) * HorizontalSpacing;

                    return ElementsAlignment switch
                    {
                        InlinedAlignment.Left => ContentDirection == ContentDirection.LeftToRight ? 0 : emptySpace,
                        InlinedAlignment.Justify => 0,
                        InlinedAlignment.SpaceAround => elementOffset,
                        InlinedAlignment.Center => emptySpace / 2,
                        InlinedAlignment.Right => ContentDirection == ContentDirection.LeftToRight ? emptySpace : 0,
                        _ => 0
                    };
                }
                
                float BaselineOffset(float elementHeight)
                {
                    var difference = lineHeight - elementHeight;

                    return BaselineAlignment switch
                    {
                        VerticalAlignment.Top => 0,
                        VerticalAlignment.Middle => difference / 2,
                        _ => difference
                    };
                }
            }

            float GetInitialAlignmentOffset()
            {
                // this method makes sure that the spacing between elements is no lesser than configured

                return ElementsAlignment switch
                {
                    InlinedAlignment.SpaceAround => HorizontalSpacing * 2,
                    _ => 0
                };
            }
        }
        
        #region IStateful
        
        private int CurrentRenderingIndex { get; set; }
    
        public void ResetState(bool hardReset = false) => CurrentRenderingIndex = 0;
        public object GetState() => CurrentRenderingIndex;
        public void SetState(object state) => CurrentRenderingIndex = (int) state;
        
        #endregion
    }
}