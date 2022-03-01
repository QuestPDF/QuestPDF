using System;

namespace QuestPDF.Elements.Text.Calculation
{
    internal class TextBlockSize
    {
        public float Width { get; set; }
        public float Height => Math.Abs(Descent) + Math.Abs(Ascent);

        public float Ascent { get; set; }
        public float Descent { get; set; }

        public float LineHeight { get; set; }
    }
}