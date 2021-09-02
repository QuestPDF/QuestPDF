namespace QuestPDF.Infrastructure
{
    public readonly struct Size
    {
        public const float Epsilon = 0.001f;

        public readonly float Width;
        public readonly float Height;
        
        public static Size Zero => new Size(0, 0);
        public static Size Max => new Size(14_400, 14_400);

        public Size(float width, float height)
        {
            Width = width;
            Height = height;
        }
        
        public override string ToString() => $"(W: {Width}, H: {Height})";
    }
}