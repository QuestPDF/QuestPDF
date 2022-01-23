using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class DecorationItemRenderingCommand
    {
        public Element Element { get; set; }
        public SpacePlan Measurement { get; set; }
        public Position Offset { get; set; }
    }
    
    internal class Decoration : Element, ICacheable
    {
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
            
            var willBeFullyRendered = renderingCommands.All(x => x.Measurement.Type == SpacePlanType.FullRender);

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
                
                Canvas.Translate(command.Offset);
                command.Element.Draw(elementSize);
                Canvas.Translate(command.Offset.Reverse());
            }
        }

        private IEnumerable<DecorationItemRenderingCommand> PlanLayout(Size availableSpace)
        {
            SpacePlan GetDecorationMeasurement(Element element)
            {
                var measurement = element.Measure(availableSpace);
                
                return measurement.Type == SpacePlanType.FullRender 
                    ? measurement 
                    : SpacePlan.Wrap();
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