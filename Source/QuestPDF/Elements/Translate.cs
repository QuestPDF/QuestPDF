using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class Translate : ContainerElement
    {
        public float TranslateX { get; set; } = 0;
        public float TranslateY { get; set; } = 0;

        internal override void Draw(Size availableSpace)
        {
            var translate = new Position(TranslateX, TranslateY);
            
            Canvas.Translate(translate);
            base.Draw(availableSpace);
            Canvas.Translate(translate.Reverse());
        }
        
        internal override string? ToCompanionHint() => $"X: {TranslateX:F1}, Y: {TranslateY:F1}";
    }
}