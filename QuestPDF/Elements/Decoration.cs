using System;
using System.Collections.Generic;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal enum DecorationType
    {
        Prepend,
        Append
    }
    
    internal class BinaryDecoration : Element, ICacheable
    {
        public Element DecorationElement { get; set; } = Empty.Instance;
        public Element ContentElement { get; set; } = Empty.Instance;
        public DecorationType Type { get; set; }

        internal override IEnumerable<Element?> GetChildren()
        {
            yield return DecorationElement;
            yield return ContentElement;
        }

        internal override void CreateProxy(Func<Element, Element> create)
        {
            DecorationElement = create(DecorationElement);
            ContentElement = create(ContentElement);
        }

        internal override SpacePlan Measure(Size availableSpace)
        {
            var decorationMeasure = DecorationElement.Measure(availableSpace);
            
            if (decorationMeasure.Type == SpacePlanType.Wrap || decorationMeasure.Type == SpacePlanType.PartialRender)
                return SpacePlan.Wrap();

            var decorationSize = decorationMeasure;
            var contentMeasure = ContentElement.Measure(new Size(availableSpace.Width, availableSpace.Height - decorationSize.Height));
            
            if (contentMeasure.Type == SpacePlanType.Wrap)
                return SpacePlan.Wrap();

            var contentSize = contentMeasure;
            var resultSize = new Size(availableSpace.Width, decorationSize.Height + contentSize.Height);
            
            if (contentSize.Type == SpacePlanType.PartialRender)
                return SpacePlan.PartialRender(resultSize);
            
            if (contentSize.Type == SpacePlanType.FullRender)
                return SpacePlan.FullRender(resultSize);
            
            throw new NotSupportedException();
        }

        internal override void Draw(Size availableSpace)
        {
            var decorationSize = DecorationElement.Measure(availableSpace);
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
            container.Element(new BinaryDecoration
            {
                Type = DecorationType.Prepend,
                DecorationElement = Header,
                ContentElement = new BinaryDecoration
                {
                    Type = DecorationType.Append,
                    ContentElement = Content,
                    DecorationElement = Footer
                }
            });
        }
    }
}