using System;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Fluent;
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
        public Element DecorationElement { get; set; } = Empty.Instance;
        public Element ContentElement { get; set; } = Empty.Instance;
        public DecorationType Type { get; set; }

        internal override void HandleVisitor(Action<Element?> visit)
        {
            DecorationElement.HandleVisitor(visit);
            ContentElement.HandleVisitor(visit);
            
            base.HandleVisitor(visit);
        }

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

        internal override void Draw(Size availableSpace)
        {
            var decorationSize = DecorationElement?.Measure(availableSpace) as Size ?? Size.Zero;
            var contentSize = new Size(availableSpace.Width, availableSpace.Height - decorationSize.Height);

            var translateHeight = Type == DecorationType.Prepend ? decorationSize.Height : contentSize.Height;
            Action drawDecoration = () => DecorationElement?.Draw(new Size(availableSpace.Width, decorationSize.Height));
            Action drawContent = () => ContentElement?.Draw(new Size (availableSpace.Width, contentSize.Height));

            var first = Type == DecorationType.Prepend ? drawDecoration : drawContent;
            var second = Type == DecorationType.Prepend ? drawContent : drawDecoration;

            first();
            Canvas.Translate(new Position(0, translateHeight));
            second();
            Canvas.Translate(new Position(0, -translateHeight));
        }
    }
    
    internal class Decoration : IComponent
    {
        public Element Header { get; set; } = Empty.Instance;
        public Element Content { get; set; } = Empty.Instance;
        public Element Footer { get; set; } = Empty.Instance;

        public void Compose(IContainer container)
        {
            container.Element(new SimpleDecoration
            {
                Type = DecorationType.Prepend,
                DecorationElement = Header,
                ContentElement = new SimpleDecoration
                {
                    Type = DecorationType.Append,
                    ContentElement = Content,
                    DecorationElement = Footer
                }
            });
        }
    }
}