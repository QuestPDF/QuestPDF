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
        public Size Size { get; set; }
        public Position Offset { get; set; }
    }
    
    internal class Decoration : Element, ICacheable
    {
        internal Element Header { get; set; } = new Empty();
        internal Element Content { get; set; } = new Empty();
        internal Element Footer { get; set; } = new Empty();

        internal override IEnumerable<Element?> GetChildren()
        {
            yield return Header;
            yield return Content;
            yield return Footer;
        }
        
        internal override void CreateProxy(Func<Element?, Element?> create)
        {
            Header = create(Header);
            Content = create(Content);
            Footer = create(Footer);
        }

        internal override SpacePlan Measure(Size availableSpace)
        {
            var renderingCommands = PlanLayout(availableSpace);

            if (renderingCommands.Any(x => x.Measurement.Type == SpacePlanType.Wrap))
                return SpacePlan.Wrap();

            var width = renderingCommands.Max(x => x.Size.Width);
            var height = renderingCommands.Sum(x => x.Size.Height);
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
            foreach (var command in PlanLayout(availableSpace))
            {
                Canvas.Translate(command.Offset);
                command.Element.Draw(command.Size);
                Canvas.Translate(command.Offset.Reverse());
            }
        }

        private IEnumerable<DecorationItemRenderingCommand> PlanLayout(Size availableSpace)
        {
            var headerMeasurement = Header.Measure(availableSpace);
            var footerMeasurement = Footer.Measure(availableSpace);
            
            var contentSpace = new Size(availableSpace.Width, availableSpace.Height - headerMeasurement.Height - footerMeasurement.Height);
            var contentMeasurement = Content.Measure(contentSpace);

            yield return new DecorationItemRenderingCommand
            {
                Element = Header,
                Measurement = headerMeasurement,
                Size = new Size(availableSpace.Width, headerMeasurement.Height),
                Offset = Position.Zero
            };
            
            yield return new DecorationItemRenderingCommand
            {
                Element = Content,
                Measurement = contentMeasurement,
                Size = new Size(availableSpace.Width, contentMeasurement.Height),
                Offset = new Position(0, headerMeasurement.Height)
            };

            yield return new DecorationItemRenderingCommand
            {
                Element = Footer,
                Measurement = footerMeasurement,
                Size = new Size(availableSpace.Width, footerMeasurement.Height),
                Offset = new Position(0, headerMeasurement.Height + contentMeasurement.Height)
            };
        }
    }
}