using System;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Elements;

namespace QuestPDF.Infrastructure
{
    internal abstract class ContainerElement : Element, IContainer
    {
        internal Element? Child { get; set; } = Empty.Instance;

        IElement? IContainer.Child
        {
            get => Child;
            set => Child = value as Element;
        }

        internal override void HandleVisitor(Action<Element?> visit)
        {
            Child?.HandleVisitor(visit);
            base.HandleVisitor(visit);
        }

        internal override void CreateProxy(Func<Element?, Element?> create)
        {
            Child = create(Child);
        }

        internal override ISpacePlan Measure(Size availableSpace)
        {
            return Child?.Measure(availableSpace) ?? new FullRender(Size.Zero);
        }
        
        internal override void Draw(Size availableSpace)
        {
            Child?.Draw(availableSpace);
        }
    }
}