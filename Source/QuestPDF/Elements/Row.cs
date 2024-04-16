using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal enum RowItemType
    {
        Auto,
        Constant,
        Relative
    }

    internal sealed class RowItem : Container
    {
        public bool IsRendered { get; set; }
        public float Width { get; set; }
        
        public RowItemType Type { get; set; }
        public float Size { get; set; }
    }

    internal sealed class RowItemRenderingCommand
    {
        public RowItem RowItem { get; set; }
        public SpacePlan Measurement { get; set; }
        public Size Size { get; set; }
        public Position Offset { get; set; }
    }

    internal sealed class Row : Element, IStateful, ICacheable, IContentDirectionAware
    {
        public ContentDirection ContentDirection { get; set; }
        
        internal List<RowItem> Items { get; } = new();
        internal float Spacing { get; set; }

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
                return SpacePlan.None();
            
            UpdateItemsWidth(availableSpace.Width);
            var renderingCommands = PlanLayout(availableSpace);

            if (renderingCommands.Any(x => !x.RowItem.IsRendered && x.Measurement.Type == SpacePlanType.Wrap))
                return SpacePlan.Wrap();

            var width = renderingCommands.Last().Offset.X + renderingCommands.Last().Size.Width;
            var height = renderingCommands.Max(x => x.Size.Height);
            var size = new Size(width, height);

            if (width > availableSpace.Width + Size.Epsilon || height > availableSpace.Height + Size.Epsilon)
                return SpacePlan.Wrap();
            
            if (renderingCommands.Any(x => !x.RowItem.IsRendered && x.Measurement.Type == SpacePlanType.PartialRender))
                return SpacePlan.PartialRender(size);

            return SpacePlan.FullRender(size);
        }

        internal override void Draw(Size availableSpace)
        {
            if (!Items.Any())
                return;

            UpdateItemsWidth(availableSpace.Width);
            var renderingCommands = PlanLayout(availableSpace);

            foreach (var command in renderingCommands)
            {
                if (command.Measurement.Type is SpacePlanType.NoContent or SpacePlanType.FullRender)
                    command.RowItem.IsRendered = true;
                
                if (command.Measurement.Type == SpacePlanType.Wrap)
                    continue;

                var offset = ContentDirection == ContentDirection.LeftToRight
                    ? command.Offset
                    : new Position(availableSpace.Width - command.Offset.X - command.Size.Width, 0);

                var targetSize = new Size(command.Size.Width, availableSpace.Height);
                    
                Canvas.Translate(offset);
                command.RowItem.Draw(targetSize);
                Canvas.Translate(offset.Reverse());
            }
        }

        private void UpdateItemsWidth(float availableWidth)
        {
            foreach (var rowItem in Items.Where(x => x.Type == RowItemType.Auto))
                rowItem.Size = rowItem.Measure(Size.Max).Width;
            
            var constantWidth = Items.Where(x => x.Type != RowItemType.Relative).Sum(x => x.Size);
            var relativeWidth = Items.Where(x => x.Type == RowItemType.Relative).Sum(x => x.Size);
            var spacingWidth = (Items.Count - 1) * Spacing;

            foreach (var item in Items.Where(x => x.Type != RowItemType.Relative))
                item.Width = item.Size;
            
            if (relativeWidth <= 0)
                return;

            var widthPerRelativeUnit = (availableWidth - constantWidth - spacingWidth) / relativeWidth;
            
            foreach (var item in Items.Where(x => x.Type == RowItemType.Relative))
                item.Width = item.Size * widthPerRelativeUnit;
        }
        
        private ICollection<RowItemRenderingCommand> PlanLayout(Size availableSpace)
        {
            var leftOffset = 0f;
            var renderingCommands = new List<RowItemRenderingCommand>();

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
            }

            if (renderingCommands.Any(x => x.Measurement.Type == SpacePlanType.Wrap))
                return renderingCommands;
            
            var rowHeight = renderingCommands
                .Where(x => !x.RowItem.IsRendered)
                .Select(x => x.Measurement.Height)
                .DefaultIfEmpty(0)
                .Max();
            
            foreach (var command in renderingCommands)
            {
                command.Size = new Size(command.Size.Width, rowHeight);
                command.Measurement = command.RowItem.Measure(command.Size);
            }
            
            return renderingCommands;
        }
        
        #region IStateful
        
        object IStateful.CloneState()
        {
            return Items.Select(x => x.IsRendered).ToArray();
        }

        void IStateful.SetState(object state)
        {
            var states = (bool[]) state;
            
            for (var i = 0; i < Items.Count; i++)
                Items[i].IsRendered = states[i];
        }

        void IStateful.ResetState(bool hardReset)
        {
            Items.ForEach(x => x.IsRendered = false);
        }
    
        #endregion
    }
}