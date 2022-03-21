using System;

namespace QuestPDF.Elements.Text.Calculation
{
    internal class TextMeasurementResult
    {
        public float Width { get; set; }
        public float Height => Math.Abs(Descent) + Math.Abs(Ascent);

        public float Ascent { get; set; }
        public float Descent { get; set; }

        public float LineHeight { get; set; }
        
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public int NextIndex { get; set; }
        public int TotalIndex { get; set; }
        public bool ForceWrap { get; set; }
        public bool IsLast => ForceWrap
            ?  NextIndex == TotalIndex
            : NextIndex == EndIndex;
    }
}