namespace QuestPDF.Infrastructure
{
    public class Size
    {
        public const float Epsilon = 0.001f;
        
        public float Width { get; }
        public float Height { get; }
        
        public static Size Zero => new Size(0, 0);
        public static Size Max => new Size(14_400, 14_400);

        public Size(float width, float height)
        {
            Width = width;
            Height = height;
        }
        
        public override string ToString() => $"(W: {Width}, H: {Height})";
        
        protected bool Equals(Size other)
        {
            return Width.Equals(other.Width) && Height.Equals(other.Height);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Size) obj);
        }
    }
}