using System;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Page : Element
    {
        public Element? Header { get; set; }
        public Element? Content { get; set; }
        public Element? Footer { get; set; }
        
        internal override ISpacePlan Measure(Size availableSpace)
        {
            var headerSize = Header?.Measure(availableSpace) as Size;
            var footerSize = Footer?.Measure(availableSpace) as Size;

            var contentHeight = availableSpace.Height - (headerSize?.Height ?? 0) - (footerSize?.Height ?? 0);

            var required = Content.Measure(new Size(availableSpace.Width, contentHeight));

            if (required is FullRender)
                return new FullRender(availableSpace);
            
            if (required is PartialRender)
                return new PartialRender(availableSpace);
            
            if (required is Wrap)
                return new Wrap();
            
            throw new NotSupportedException();
        }

        internal override void Draw(ICanvas canvas, Size availableSpace)
        {
            var headerHeight = (Header?.Measure(availableSpace) as Size)?.Height ?? 0;
            var footerHeight = (Footer?.Measure(availableSpace) as Size)?.Height ?? 0;

            var contentHeight = availableSpace.Height - headerHeight - footerHeight;
            var contentSize = new Size(availableSpace.Width, contentHeight);

            if (headerHeight > 0)
            {
                Header?.Draw(canvas, new Size(availableSpace.Width, headerHeight));
                canvas.Translate(new Position(0, headerHeight));
            }

            Content.Draw(canvas, new Size(availableSpace.Width, contentSize.Height));
            canvas.Translate(new Position(0, contentSize.Height));
            
            if (footerHeight > 0)
            {
                Footer?.Draw(canvas, new Size(availableSpace.Width, footerHeight));
            }
        }
    }
}