using System;
using System.Collections.Generic;
using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal enum RowItemType
    {
        Auto,
        Constant,
        Relative
    }

    internal sealed class RowItem : ContainerElement
    {
        public bool IsRendered { get; set; }
        public float Width { get; set; }
        
        public RowItemType Type { get; set; }
        public float Size { get; set; }

        internal override string? GetCompanionHint()
        {
            if (Type == RowItemType.Auto)
                return "Auto";
            
            return $"{Type} {Size.FormatAsCompanionNumber()}";
        }
    }

    internal struct RowItemRenderingCommand
    {
        public RowItem RowItem { get; set; }
        public SpacePlan Measurement { get; set; }
        public Size Size { get; set; }
        public Position Offset { get; set; }
    }

    internal sealed class Row : Element, IStateful, IContentDirectionAware
    {
        public ContentDirection ContentDirection { get; set; }
        
        internal List<RowItem> Items { get; } = new();
        internal float Spacing { get; set; }
        
        internal override IReadOnlyList<Element?> GetChildren()
        {
            return Items;
        }

        internal override SpacePlan Measure(Size availableSpace)
        {
            if (Items.Count == 0)
                return SpacePlan.Empty();

            if (AreAllItemsRendered())
                return SpacePlan.Empty();
            
            UpdateItemsWidth(availableSpace.Width);
            
            if (AnyItemHasNegativeWidth())
                return SpacePlan.Wrap("One of the items has a negative size, indicating insufficient horizontal space. Usually, constant items require more space than is available, potentially causing other content to overflow.");
            
            using var renderingCommands = PlanLayout(availableSpace);

            var (width, height, hasWrappedItem, hasPartiallyRenderedItem) = SummarizeLayout(renderingCommands);

            if (hasWrappedItem)
                return SpacePlan.Wrap("One of the items does not fit (even partially) in the available space.");

            var size = new Size(width, height);

            if (width.IsGreaterThan(availableSpace.Width))
                return SpacePlan.Wrap("The content requires more horizontal space than available.");
            
            if (height.IsGreaterThan(availableSpace.Height))
                return SpacePlan.Wrap("The content requires more vertical space than available.");
            
            if (hasPartiallyRenderedItem)
                return SpacePlan.PartialRender(size);

            return SpacePlan.FullRender(size);
        }

        internal override void Draw(Size availableSpace)
        {
            if (Items.Count == 0)
                return;

            if (AreAllItemsRendered())
                return;

            UpdateItemsWidth(availableSpace.Width);
            using var renderingCommands = PlanLayout(availableSpace);

            foreach (var command in renderingCommands)
            {
                if (command.Measurement.Type is SpacePlanType.Empty or SpacePlanType.FullRender)
                    command.RowItem.IsRendered = true;
                
                // TODO: investigate, as the final targetSize is still changed to use available vertical space
                if (command.Measurement.Type is SpacePlanType.Wrap)
                    continue;

                var offset = ContentDirection == ContentDirection.LeftToRight
                    ? command.Offset
                    : new Position(availableSpace.Width - command.Offset.X - command.Size.Width, 0);

                var targetSize = new Size(command.Size.Width, availableSpace.Height);
                    
                if (targetSize.Width.IsLessThan(0))
                    continue;
                
                Canvas.Translate(offset);
                command.RowItem.Draw(targetSize);
                Canvas.Translate(offset.Reverse());
            }
        }

        private bool AreAllItemsRendered()
        {
            foreach (var item in Items)
            {
                if (!item.IsRendered)
                    return false;
            }

            return true;
        }

        private bool AnyItemHasNegativeWidth()
        {
            foreach (var item in Items)
            {
                if (item.Width.IsLessThan(0))
                    return true;
            }

            return false;
        }

        // the List parameter type keeps the struct enumerator
        private static (float Width, float Height, bool HasWrappedItem, bool HasPartiallyRenderedItem) SummarizeLayout(List<RowItemRenderingCommand> renderingCommands)
        {
            var height = 0f;
            var hasWrappedItem = false;
            var hasPartiallyRenderedItem = false;

            foreach (var command in renderingCommands)
            {
                height = Math.Max(height, command.Size.Height);

                if (command.RowItem.IsRendered)
                    continue;

                if (command.Measurement.Type == SpacePlanType.Wrap)
                    hasWrappedItem = true;

                else if (command.Measurement.Type == SpacePlanType.PartialRender)
                    hasPartiallyRenderedItem = true;
            }
            
            var lastCommand = renderingCommands[renderingCommands.Count - 1];
            var width = lastCommand.Offset.X + lastCommand.Size.Width;

            return (width, height, hasWrappedItem, hasPartiallyRenderedItem);
        }

        private void UpdateItemsWidth(float availableWidth)
        {
            var widthPerRelativeUnit = GetWidthPerRelativeUnit();

            foreach (var item in Items)
            {
                if (item.Type == RowItemType.Relative)
                    item.Width = item.Size * widthPerRelativeUnit;
                
                else
                    item.Width = item.Size;
            }

            float GetWidthPerRelativeUnit()
            {
                var constantWidth = 0f;
                var relativeWidth = 0f;

                foreach (var item in Items)
                {
                    if (item.Type == RowItemType.Auto && item.Size == 0)
                        item.Size = item.Measure(Size.Max).Width;

                    if (item.Type == RowItemType.Relative)
                        relativeWidth += item.Size;
                    
                    else
                        constantWidth += item.Size;
                }

                if (relativeWidth <= 0)
                    return 0;

                var spacingWidth = (Items.Count - 1) * Spacing;
                return (availableWidth - constantWidth - spacingWidth) / relativeWidth;
            }
        }
        
        private ReusableList<RowItemRenderingCommand> PlanLayout(Size availableSpace)
        {
            var renderingCommands = ReusableList<RowItemRenderingCommand>.Get();
            
            // measure all items and their positions
            var leftOffset = 0f;
            var hasWrappedItem = false;

            foreach (var item in Items)
            {
                var itemSpace = new Size(item.Width, availableSpace.Height);
                
                var command = new RowItemRenderingCommand
                {
                    RowItem = item,
                    Size = itemSpace,
                    Measurement = item.Measure(itemSpace),
                    Offset = new Position(leftOffset, 0)
                };
                
                renderingCommands.Add(command);
                leftOffset += item.Width + Spacing;

                if (command.Measurement.Type == SpacePlanType.Wrap)
                    hasWrappedItem = true;
            }

            if (hasWrappedItem)
                return renderingCommands;

            // adjust all items height to the tallest item
            var rowHeight = GetTallestItemHeight();

            // RowItemRenderingCommand is a struct: modified copies must be written back to the list
            for (var i = 0; i < renderingCommands.Count; i++)
            {
                var command = renderingCommands[i];

                command.Size = new Size(command.Size.Width, rowHeight);
                command.Measurement = command.RowItem.Measure(command.Size);

                renderingCommands[i] = command;
            }
            
            return renderingCommands;

            float GetTallestItemHeight()
            {
                var result = 0f;

                foreach (var command in renderingCommands)
                {
                    if (!command.RowItem.IsRendered)
                        result = Math.Max(result, command.Measurement.Height);
                }

                return result;
            }
        }
        
        #region IStateful
        
        // State is stored in the RowItem instances
    
        public void ResetState(bool hardReset = false)
        {
            foreach (var rowItem in Items)
            {
                rowItem.IsRendered = false;
                
                // required when the row contains items with text representing page numbers
                if (rowItem.Type == RowItemType.Auto)
                    rowItem.Size = 0;
            }
        }

        public object GetState()
        {
            var result = new bool[Items.Count];
            
            for (var i = 0; i < Items.Count; i++)
                result[i] = Items[i].IsRendered;
            
            return result;
        }

        public void SetState(object state)
        {
            var states = (bool[]) state;
            
            for (var i = 0; i < Items.Count; i++)
                Items[i].IsRendered = states[i];
        }
    
        #endregion
    }
}