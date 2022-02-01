using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class ColumnItem : Container
    {
        public bool IsRendered { get; set; }
    }
    
    internal class ColumnItemRenderingCommand
    {
        public ColumnItem ColumnItem { get; set; }
        public SpacePlan Measurement { get; set; }
        public Size Size { get; set; }
        public Position Offset { get; set; }
    }
    
    internal class Column : Element, ICacheable, IStateResettable
    {
        internal List<ColumnItem> Items { get; } = new();
        internal float Spacing { get; set; }

        public void ResetState()
        {
            Items.ForEach(x => x.IsRendered = false);
        }
        
        internal override IEnumerable<Element?> GetChildren()
        {
            return Items;
        }
        
        internal override void CreateProxy(Func<Element?, Element?> create)
        {
            Items.ForEach(x => x.Child = create(x.Child));
        }

        internal override SpacePlan Measure(Size availableSpace)
        {
            if (!Items.Any())
                return SpacePlan.FullRender(Size.Zero);
            
            var renderingCommands = PlanLayout(availableSpace);

            if (!renderingCommands.Any())
                return SpacePlan.Wrap();

            var width = renderingCommands.Max(x => x.Size.Width);
            var height = renderingCommands.Last().Offset.Y + renderingCommands.Last().Size.Height;
            var size = new Size(width, height);
            
            if (width > availableSpace.Width + Size.Epsilon || height > availableSpace.Height + Size.Epsilon)
                return SpacePlan.Wrap();
            
            var totalRenderedItems = Items.Count(x => x.IsRendered) + renderingCommands.Count(x => x.Measurement.Type == SpacePlanType.FullRender);
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
                if (command.Measurement.Type == SpacePlanType.FullRender)
                    command.ColumnItem.IsRendered = true;

                var targetSize = new Size(availableSpace.Width, command.Size.Height);
                
                Canvas.Translate(command.Offset);
                command.ColumnItem.Draw(targetSize);
                Canvas.Translate(command.Offset.Reverse());
            }
            
            if (Items.All(x => x.IsRendered))
                ResetState();
        }

        private ICollection<ColumnItemRenderingCommand> PlanLayout(Size availableSpace)
        {
            var topOffset = 0f;
            var commands = new List<ColumnItemRenderingCommand>();

            foreach (var item in Items)
            {
                if (item.IsRendered)
                    continue;

                var itemSpace = new Size(availableSpace.Width, availableSpace.Height - topOffset);
                var measurement = item.Measure(itemSpace);
                
                if (measurement.Type == SpacePlanType.Wrap)
                    break;

                commands.Add(new ColumnItemRenderingCommand
                {
                    ColumnItem = item,
                    Size = measurement,
                    Measurement = measurement,
                    Offset = new Position(0, topOffset)
                });
                
                if (measurement.Type == SpacePlanType.PartialRender)
                    break;
                
                topOffset += measurement.Height + Spacing;
            }

            var targetWidth = commands.Select(x => x.Size.Width).DefaultIfEmpty(0).Max();
            commands.ForEach(x => x.Size = new Size(targetWidth, x.Size.Height));
            
            return commands;
        }
    }
}