using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class DecorationItemRenderingCommand
    {
        public Element Element { get; set; }
        public SpacePlan Measurement { get; set; }
        public Position Offset { get; set; }
    }

    internal sealed class Decoration : Element, ICacheable, IContentDirectionAware
    {
        public ContentDirection ContentDirection { get; set; }
        
        internal Element Before { get; set; } = new Empty();
        internal Element Content { get; set; } = new Empty();
        internal Element After { get; set; } = new Empty();

        internal override IEnumerable<Element?> GetChildren()
        {
            yield return Before;
            yield return Content;
            yield return After;
        }
        
        internal override void CreateProxy(Func<Element?, Element?> create)
        {
            Before = create(Before);
            Content = create(Content);
            After = create(After);
        }

        internal override SpacePlan Measure(Size availableSpace)
        {
            var renderingCommands = PlanLayout(availableSpace).ToList();

            if (renderingCommands.Any(x => x.Measurement.Type == SpacePlanType.Wrap))
                return SpacePlan.Wrap();

            var width = renderingCommands.Max(x => x.Measurement.Width);
            var height = renderingCommands.Sum(x => x.Measurement.Height);
            var size = new Size(width, height);
            
            if (width > availableSpace.Width + Size.Epsilon || height > availableSpace.Height + Size.Epsilon)
                return SpacePlan.Wrap();

            if (renderingCommands.All(x => x.Measurement.Type == SpacePlanType.NoContent))
                return SpacePlan.None();
            
            var willBeFullyRendered = renderingCommands.All(x => x.Measurement.Type is SpacePlanType.NoContent or SpacePlanType.FullRender);

            return willBeFullyRendered
                ? SpacePlan.FullRender(size)
                : SpacePlan.PartialRender(size);
        }

        internal override void Draw(Size availableSpace)
        {
            var renderingCommands = PlanLayout(availableSpace).ToList();
            var width = renderingCommands.Max(x => x.Measurement.Width);
            
            foreach (var command in renderingCommands)
            {
                var elementSize = new Size(width, command.Measurement.Height);
                
                var offset = ContentDirection == ContentDirection.LeftToRight
                    ? command.Offset
                    : new Position(availableSpace.Width - width, command.Offset.Y);
                
                Canvas.Translate(offset);
                command.Element.Draw(elementSize);
                Canvas.Translate(offset.Reverse());
            }
        }

        private IEnumerable<DecorationItemRenderingCommand> PlanLayout(Size availableSpace)
        {
            SpacePlan GetDecorationMeasurement(Element element)
            {
                var measurement = element.Measure(availableSpace);
                
                return measurement.Type == SpacePlanType.Wrap 
                    ? SpacePlan.Wrap() 
                    : measurement;
            }
            
            var beforeMeasurement = GetDecorationMeasurement(Before);
            var afterMeasurement = GetDecorationMeasurement(After);
            
            var contentSpace = new Size(availableSpace.Width, availableSpace.Height - beforeMeasurement.Height - afterMeasurement.Height);
            var contentMeasurement = Content.Measure(contentSpace);

            yield return new DecorationItemRenderingCommand
            {
                Element = Before,
                Measurement = beforeMeasurement,
                Offset = Position.Zero
            };
            
            yield return new DecorationItemRenderingCommand
            {
                Element = Content,
                Measurement = contentMeasurement,
                Offset = new Position(0, beforeMeasurement.Height)
            };

            yield return new DecorationItemRenderingCommand
            {
                Element = After,
                Measurement = afterMeasurement,
                Offset = new Position(0, beforeMeasurement.Height + contentMeasurement.Height)
            };
        }
    }
}