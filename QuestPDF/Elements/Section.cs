using System;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Section : Element
    {
        public Element? Header { get; set; }
        public Element? Content { get; set; }

        internal override ISpacePlan Measure(Size availableSpace)
        {
            var headerMeasure = Header?.Measure(availableSpace);
            
            if (headerMeasure is Wrap || headerMeasure is PartialRender)
                return new Wrap();

            var headerSize = headerMeasure as Size ?? Size.Zero;
            var contentMeasure = Content?.Measure(new Size(availableSpace.Width, availableSpace.Height - headerSize.Height)) ?? new FullRender(Size.Zero);
            
            if (contentMeasure is Wrap)
                return new Wrap();

            var contentSize = contentMeasure as Size ?? Size.Zero;
            
            var newSize = new Size(
                availableSpace.Width,
                headerSize.Height + contentSize.Height);
            
            if (contentSize is PartialRender)
                return new PartialRender(newSize);
            
            if (contentSize is FullRender)
                return new FullRender(newSize);
            
            throw new NotSupportedException();
        }

        internal override void Draw(ICanvas canvas, Size availableSpace)
        {
            var measurement = Measure(availableSpace);

            if (measurement is Wrap)
                return;
            
            var headerSize = Header?.Measure(availableSpace) as Size ?? Size.Zero;

            var contentAvailableSize = new Size(availableSpace.Width, availableSpace.Height - headerSize.Height);
            var contentSize = Content?.Measure(contentAvailableSize) as Size;

            Header?.Draw(canvas, new Size(availableSpace.Width, headerSize.Height));
            
            canvas.Translate(new Position(0, headerSize.Height));
            
            if (contentSize != null)
                Content?.Draw(canvas, new Size(availableSpace.Width, contentSize.Height));
            
            canvas.Translate(new Position(0, -headerSize.Height));
        }
    }
}