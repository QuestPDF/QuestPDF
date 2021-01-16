namespace QuestPDF.Infrastructure
{
    public class TextStyle
    {
        public string Color { get; set; } = "#000000";
        public string FontType { get; set; } = "Helvetica";
        public float Size { get; set; } = 12;
        public float LineHeight { get; set; } = 1.2f;
        public HorizontalAlignment Alignment { get; set; } = HorizontalAlignment.Left;
        
        public static TextStyle Default => new TextStyle();
        
        public override string ToString()
        {
            return $"{Color}|{FontType}|{Size}|{LineHeight}|{Alignment}";
        }
    }
}