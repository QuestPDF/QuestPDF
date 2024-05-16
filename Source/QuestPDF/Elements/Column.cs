using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class ColumnItemRenderingCommand
    {
        public Element Element { get; set; }
        public SpacePlan Measurement { get; set; }
        public Position Offset { get; set; }
    }

    internal sealed class Column : Element, ICacheable, IStateResettable
    {
        internal List<Element> Items { get; } = new();
        internal float Spacing { get; set; }
        
        internal int CurrentRenderingIndex { get; set; }

        public void ResetState(bool hardReset)
        {
            CurrentRenderingIndex = 0;
        }
        
        internal override IEnumerable<Element?> GetChildren()
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
            if (!Items.Any())
                return SpacePlan.FullRender(Size.Zero);
            
            if (CurrentRenderingIndex == Items.Count)
                return SpacePlan.FullRender(Size.Zero);
            
            var renderingCommands = PlanLayout(availableSpace);

            if (!renderingCommands.Any())
                return SpacePlan.Wrap();

            var width = renderingCommands.Max(x => x.Measurement.Width);
            var height = renderingCommands.Last().Offset.Y + renderingCommands.Last().Measurement.Height;
            var size = new Size(width, height);
            
            if (width > availableSpace.Width + Size.Epsilon || height > availableSpace.Height + Size.Epsilon)
                return SpacePlan.Wrap();
            
            var totalRenderedItems = CurrentRenderingIndex + renderingCommands.Count(x => x.Measurement.Type is SpacePlanType.FullRender);
            var willBeFullyRendered = totalRenderedItems == Items.Count;

            return willBeFullyRendered
                ? SpacePlan.FullRender(size)
                : SpacePlan.PartialRender(size);
        }

        internal override void Draw(Size availableSpace)
        {
            var renderingCommands = PlanLayout(availableSpace);

            foreach (var command in renderingCommands)
            {
                var targetSize = new Size(availableSpace.Width, command.Measurement.Height);

                Canvas.Translate(command.Offset);
                command.Element.Draw(targetSize);
                Canvas.Translate(command.Offset.Reverse());
            }
            
            var fullyRenderedItems = renderingCommands.Count(x => x.Measurement.Type is SpacePlanType.FullRender);
            CurrentRenderingIndex += fullyRenderedItems;
        }

        private List<ColumnItemRenderingCommand> PlanLayout(Size availableSpace)
        {
            var topOffset = 0f;
            var targetWidth = 0f;
            var commands = new List<ColumnItemRenderingCommand>();

            foreach (var item in Items.Skip(CurrentRenderingIndex))
            {
                var availableHeight = availableSpace.Height - topOffset;

                var itemSpace = availableHeight > 0
                    ? new Size(availableSpace.Width, availableHeight)
                    : Size.Zero;
                
                var measurement = item.Measure(itemSpace);
                
                if (measurement.Type == SpacePlanType.Wrap)
                    break;
                
                if (Size.Equal(itemSpace, Size.Zero) && !Size.Equal(measurement, Size.Zero))
                    break;

                // when the item does not take any space, do not add spacing
                if (measurement.Width < Size.Epsilon && measurement.Height < Size.Epsilon)
                    topOffset -= Spacing;
                
                commands.Add(new ColumnItemRenderingCommand
                {
                    Element = item,
                    Measurement = measurement,
                    Offset = new Position(0, topOffset)
                });
                
                if (measurement.Width > targetWidth)
                    targetWidth = measurement.Width;
                
                if (measurement.Type == SpacePlanType.PartialRender)
                    break;
                
                topOffset += measurement.Height + Spacing;
            }

            return commands;
        }
    }
}