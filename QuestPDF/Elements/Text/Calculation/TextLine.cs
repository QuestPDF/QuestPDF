using System.Collections.Generic;
using System.Linq;
using QuestPDF.Elements.Text.Items;

namespace QuestPDF.Elements.Text.Calculation
{
    internal class TextLine
    {
        public ICollection<TextLineElement> Elements { get; set; }

        public float TextHeight => Elements.Max(x => x.Measurement.Height);
        public float LineHeight => Elements.Max(x => x.Measurement.LineHeight * x.Measurement.Height);
        
        public float Ascent => Elements.Min(x => x.Measurement.Ascent) - (LineHeight - TextHeight) / 2;
        public float Descent => Elements.Max(x => x.Measurement.Descent) + (LineHeight - TextHeight) / 2;

        public float Width => Elements.Sum(x => x.Measurement.Width);
    }
}