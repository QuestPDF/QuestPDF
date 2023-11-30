using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class ColumnItemRenderingCommand
    {
        public Element ColumnItem { get; set; }
        public SpacePlan Measurement { get; set; }
        public Size Size { get; set; }
        public Position Offset { get; set; }
    }

    internal struct ColumnState
    {
        public int ChildIndex = 0;
        public bool IsRendered = false;
        
        public ColumnState()
        {
            
        }
    }
    
    internal sealed class Column : Element, ICacheable, IStateResettable, IStateful<int>
    {
        public int State { get; set; } = 0; // index of item to be rendered next
        
        internal List<Element> Items { get; } = new();
        internal float Spacing { get; set; }

        public void ResetState()
        {
            State = 0;
        }
        
        internal override IEnumerable<Element?> GetChildren()
        {
            return Items;
        }
        
        internal override void CreateProxy(Func<Element?, Element?> create)
        {
            Items.ForEach(x => x = create(x));
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
            
            var totalRenderedItems = State + renderingCommands.Count(x => x.Measurement.Type == SpacePlanType.FullRender);
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
                    State++;

                var targetSize = new Size(availableSpace.Width, command.Size.Height);

                Canvas.Translate(command.Offset);
                command.ColumnItem.Draw(targetSize);
                Canvas.Translate(command.Offset.Reverse());
            }
            
            if (State == Items.Count)
                ResetState();
        }

        private ICollection<ColumnItemRenderingCommand> PlanLayout(Size availableSpace)
        {
            var topOffset = 0f;
            var targetWidth = 0f;
            var commands = new List<ColumnItemRenderingCommand>();

            for (var i = State; i < Items.Count; i++)
            {
                var item = Items[i];

                var availableHeight = availableSpace.Height - topOffset;
                
                if (availableHeight < -Size.Epsilon)
                    break;

                var itemSpace = new Size(availableSpace.Width, availableHeight);
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
                
                if (measurement.Width > targetWidth)
                    targetWidth = measurement.Width;
                
                if (measurement.Type == SpacePlanType.PartialRender)
                    break;
                
                topOffset += measurement.Height + Spacing;
            }

            foreach (var command in commands)
                command.Size = new Size(targetWidth, command.Size.Height);

            return commands;
        }
    }
}