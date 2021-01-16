using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Elements;

namespace QuestPDF.Infrastructure
{
    internal abstract class ContainerElement : Element, IContainer
    {
        internal Element? Child { get; set; } = new Empty();

        IElement? IContainer.Child
        {
            get => Child;
            set => Child = value as Element;
        }

        internal override ISpacePlan Measure(Size availableSpace)
        {
            return Child?.Measure(availableSpace) ?? new FullRender(Size.Zero);
        }
        
        internal override void Draw(ICanvas canvas, Size availableSpace)
        {
            Child?.Draw(canvas, availableSpace);
        }
    }
}