namespace QuestPDF.Infrastructure
{
    internal class Position
    {
        public float X { get; }
        public float Y { get; }

        public static Position Zero { get; } = new Position(0, 0);
        
        public Position(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Position Reverse()
        {
            return new Position(-X, -Y);
        }
    }
}