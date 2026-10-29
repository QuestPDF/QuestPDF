using System;
using System.Collections.Generic;
using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal readonly struct ColumnItemRenderingCommand
    {
        public Element Element { get; init; }
        public SpacePlan Measurement { get; init; }
        public Position Offset { get; init; }
    }

    internal sealed class Column : Element, IStateful
    {
        internal List<Element> Items { get; } = new();
        internal float Spacing { get; set; }
        
        internal override IReadOnlyList<Element?> GetChildren()
        {
            return Items;
        }
        
        internal override void CreateProxy(Func<Element?, Element?> create)
        {
            for (var i = 0; i < Items.Count; i++)
                Items[i] = create(Items[i]);
        }

        internal override SpacePlan Measure(Size availableSpace)
        {
            if (Items.Count == 0)
                return SpacePlan.Empty();
            
            if (CurrentRenderingIndex == Items.Count)
                return SpacePlan.Empty();
            
            if (availableSpace.IsNegative())
                return SpacePlan.Wrap("The available space is negative.");
            
            using var renderingCommands = PlanLayout(availableSpace);

            if (renderingCommands.Count == 0)
                return SpacePlan.Wrap("The available space is not sufficient for even partially rendering a single item.");

            var (width, height) = SummarizeLayout(renderingCommands);
            var size = new Size(width, height);
            
            if (width > availableSpace.Width + Size.Epsilon)
                return SpacePlan.Wrap("The content requires more horizontal space than available.");
            
            if (height > availableSpace.Height + Size.Epsilon)
                return SpacePlan.Wrap("The content requires more vertical space than available.");
            
            var totalRenderedItems = CurrentRenderingIndex + GetFullyRenderedItemsCount(renderingCommands);
            var willBeFullyRendered = totalRenderedItems == Items.Count;

            return willBeFullyRendered
                ? SpacePlan.FullRender(size)
                : SpacePlan.PartialRender(size);
        }

        internal override void Draw(Size availableSpace)
        {
            using var renderingCommands = PlanLayout(availableSpace);

            foreach (var command in renderingCommands)
            {
                var targetSize = new Size(availableSpace.Width, command.Measurement.Height);

                Canvas.Translate(command.Offset);
                command.Element.Draw(targetSize);
                Canvas.Translate(command.Offset.Reverse());
            }
            
            CurrentRenderingIndex += GetFullyRenderedItemsCount(renderingCommands);
        }

        private ReusableList<ColumnItemRenderingCommand> PlanLayout(Size availableSpace)
        {
            var commands = ReusableList<ColumnItemRenderingCommand>.Get();

            // bottom edge of the last item that has height
            var topOffset = 0f;
            var anyItemHasHeight = false;

            for (var i = CurrentRenderingIndex; i < Items.Count; i++)
            {
                var item = Items[i];

                var spacing = anyItemHasHeight ? Spacing : 0;
                var availableHeight = Math.Max(0, availableSpace.Height - topOffset - spacing);

                // the full width is always offered, so an item without height (e.g. postponed content with horizontal padding) is accepted even when no height is left
                var measurement = item.Measure(new Size(availableSpace.Width, availableHeight));

                if (measurement.Type == SpacePlanType.Wrap)
                    break;

                // the spacing separates items that have height; an item without height sits at the bottom edge of the previous one, regardless of its width
                var itemHasHeight = measurement.Height > Size.Epsilon;
                var itemOffset = itemHasHeight ? topOffset + spacing : topOffset;

                commands.Add(new ColumnItemRenderingCommand
                {
                    Element = item,
                    Measurement = measurement,
                    Offset = new Position(0, itemOffset)
                });

                if (measurement.Type == SpacePlanType.PartialRender)
                    break;

                if (!itemHasHeight)
                    continue;

                topOffset = itemOffset + measurement.Height;
                anyItemHasHeight = true;
            }

            return commands;
        }

        // the List parameter type keeps the struct enumerator
        private static (float Width, float Height) SummarizeLayout(List<ColumnItemRenderingCommand> renderingCommands)
        {
            var width = 0f;

            foreach (var command in renderingCommands)
            {
                width = Math.Max(width, command.Measurement.Width);
            }

            var lastCommand = renderingCommands[renderingCommands.Count - 1];
            var height = lastCommand.Offset.Y + lastCommand.Measurement.Height;

            return (width, height);
        }
        
        // the List parameter type keeps the struct enumerator
        private static int GetFullyRenderedItemsCount(List<ColumnItemRenderingCommand> renderingCommands)
        {
            var result = 0;
            
            foreach (var command in renderingCommands)
            {
                if (command.Measurement.Type is SpacePlanType.Empty or SpacePlanType.FullRender)
                    result++;
            }

            return result;
        }
        
        #region IStateful
        
        internal int CurrentRenderingIndex { get; set; }
    
        public void ResetState(bool hardReset = false) => CurrentRenderingIndex = 0;
        public object GetState() => CurrentRenderingIndex;
        public void SetState(object state) => CurrentRenderingIndex = (int) state;
        
        #endregion
    }
}