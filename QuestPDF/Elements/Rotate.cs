using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Rotate : ContainerElement
    {
        public float Angle { get; set; } = 0;

        internal override void Draw(Size availableSpace)
        {
            Canvas.Rotate(Angle);
            Child?.Draw(availableSpace);
            Canvas.Rotate(-Angle);
        }
    }
}