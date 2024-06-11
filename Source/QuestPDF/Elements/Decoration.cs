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

    internal sealed class Decoration : Element, IContentDirectionAware
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
            var renderingCommands = PlanLayout(availableSpace);

            if (renderingCommands.Single(x => x.Element == Content).Measurement.Type == SpacePlanType.Empty)
                return SpacePlan.Empty();
            
            if (renderingCommands.Any(x => x.Measurement.Type == SpacePlanType.Wrap))
                return SpacePlan.Wrap();

            var width = renderingCommands.Max(x => x.Measurement.Width);
            var height = renderingCommands.Sum(x => x.Measurement.Height);
            var size = new Size(width, height);
            
            if (width > availableSpace.Width + Size.Epsilon || height > availableSpace.Height + Size.Epsilon)
                return SpacePlan.Wrap();
            
            var willBeFullyRendered = renderingCommands.All(x => x.Measurement.Type is SpacePlanType.Empty or SpacePlanType.FullRender);

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

        private DecorationItemRenderingCommand[] PlanLayout(Size availableSpace)
        {
            SpacePlan GetDecorationMeasurement(Element element)
            {
                var measurement = element.Measure(availableSpace);

                if (measurement.Type is SpacePlanType.PartialRender or SpacePlanType.Wrap)
                    return SpacePlan.Wrap();

                return measurement;
            }
            
            var beforeMeasurement = GetDecorationMeasurement(Before);
            var afterMeasurement = GetDecorationMeasurement(After);
            
            var contentSpace = new Size(availableSpace.Width, availableSpace.Height - beforeMeasurement.Height - afterMeasurement.Height);
            var contentMeasurement = Content.Measure(contentSpace);

            return new[]
            { 
                new DecorationItemRenderingCommand
                {
                    Element = Before,
                    Measurement = beforeMeasurement,
                    Offset = Position.Zero
                },
                new DecorationItemRenderingCommand
                {
                    Element = Content,
                    Measurement = contentMeasurement,
                    Offset = new Position(0, beforeMeasurement.Height)
                },
                new DecorationItemRenderingCommand
                {
                    Element = After,
                    Measurement = afterMeasurement,
                    Offset = new Position(0, beforeMeasurement.Height + contentMeasurement.Height)
                }
            };
        }
    }
}