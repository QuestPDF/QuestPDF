using System.Collections.Generic;
using System.Linq;
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

        internal override string? GetCompanionHint()
        {
            return string.Join("   ", GetOptions().Where(x => x.value != 0).Select(x => $"{x.Label}={x.value:F1}"));
            
            IEnumerable<(string Label, float value)> GetOptions()
            {
                yield return ("X", TranslateX);
                yield return ("Y", TranslateY);
            }
        }
    }
}