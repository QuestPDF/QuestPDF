namespace QuestPDF.Infrastructure
{
    internal readonly struct Position
    {
        public readonly float X;
        public readonly float Y;

        public static Position Zero => new Position(0, 0);
        
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