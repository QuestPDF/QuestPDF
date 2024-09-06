using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class Rotate : ContainerElement
    {
        public float Angle { get; set; } = 0;

        internal override void Draw(Size availableSpace)
        {
            Canvas.Rotate(Angle);
            Child?.Draw(availableSpace);
            Canvas.Rotate(-Angle);
        }
        
        internal override string? GetCompanionHint() => $"{Angle} deg clockwise";
    }
}