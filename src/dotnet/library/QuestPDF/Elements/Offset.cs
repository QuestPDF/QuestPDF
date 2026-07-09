using System.Collections.Generic;
using System.Linq;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class Offset : ContainerElement
    {
        public float OffsetX { get; set; } = 0;
        public float OffsetY { get; set; } = 0;

        internal override void Draw(Size availableSpace)
        {
            var offset = new Position(OffsetX, OffsetY);

            Canvas.Translate(offset);
            base.Draw(availableSpace);
            Canvas.Translate(offset.Reverse());
        }

        internal override string? GetCompanionHint()
        {
            return string.Join("   ", GetOptions().Where(x => x.value != 0).Select(x => $"{x.Label}={x.value.FormatAsCompanionNumber()}"));

            IEnumerable<(string Label, float value)> GetOptions()
            {
                yield return ("X", OffsetX);
                yield return ("Y", OffsetY);
            }
        }
    }
}
