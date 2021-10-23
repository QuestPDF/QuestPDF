namespace QuestPDF.Infrastructure
{
    public class Size
    {
        public const float Epsilon = 0.001f;
        
        public float Width { get; }
        public float Height { get; }
        
        public static Size Zero { get; } = new Size(0, 0);
        public static Size Max { get; } = new Size(14_400, 14_400);

        public Size(float width, float height)
        {
            Width = width;
            Height = height;
        }
        
        public override string ToString() => $"(W: {Width}, H: {Height})";
    }
}