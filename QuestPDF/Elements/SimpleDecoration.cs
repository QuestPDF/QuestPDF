using System;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal enum DecorationType
    {
        Prepend,
        Append
    }
    
    internal class SimpleDecoration : Element
    {
        public Element DecorationElement { get; set; } = new Empty();
        public Element ContentElement { get; set; } = new Empty();
        public DecorationType Type { get; set; } 

        internal override ISpacePlan Measure(Size availableSpace)
        {
            var decorationMeasure = DecorationElement?.Measure(availableSpace);
            
            if (decorationMeasure is Wrap || decorationMeasure is PartialRender)
                return new Wrap();

            var decorationSize = decorationMeasure as Size ?? Size.Zero;
            var contentMeasure = ContentElement?.Measure(new Size(availableSpace.Width, availableSpace.Height - decorationSize.Height)) ?? new FullRender(Size.Zero);
            
            if (contentMeasure is Wrap)
                return new Wrap();

            var contentSize = contentMeasure as Size ?? Size.Zero;
            var resultSize = new Size(availableSpace.Width, decorationSize.Height + contentSize.Height);
            
            if (contentSize is PartialRender)
                return new PartialRender(resultSize);
            
            if (contentSize is FullRender)
                return new FullRender(resultSize);
            
            throw new NotSupportedException();
        }

        internal override void Draw(ICanvas canvas, Size availableSpace)
        {
            var decorationSize = DecorationElement?.Measure(availableSpace) as Size ?? Size.Zero;
            var contentSize = new Size(availableSpace.Width, availableSpace.Height - decorationSize.Height);

            var translateHeight = Type == DecorationType.Prepend ? decorationSize.Height : contentSize.Height;
            Action drawDecoration = () => DecorationElement?.Draw(canvas, new Size(availableSpace.Width, decorationSize.Height));
            Action drawContent = () => ContentElement?.Draw(canvas, new Size (availableSpace.Width, contentSize.Height));

            var first = Type == DecorationType.Prepend ? drawDecoration : drawContent;
            var second = Type == DecorationType.Prepend ? drawContent : drawDecoration;

            first();
            canvas.Translate(new Position(0, translateHeight));
            second();
            canvas.Translate(new Position(0, -translateHeight));
        }
    }
}