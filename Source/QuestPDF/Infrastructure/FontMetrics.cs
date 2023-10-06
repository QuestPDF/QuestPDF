namespace QuestPDF.Infrastructure
{
    internal struct FontMetrics
    {
        public float Ascent { get; set; }
        public float Descent { get; set; }
    
        public float UnderlinePosition { get; set; }
        public float UnderlineThickness { get; set; }
    
        public float StrikeoutPosition { get; set; }
        public float StrikeoutThickness { get; set; }
    }
}