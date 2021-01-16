namespace QuestPDF.Infrastructure
{
    internal class Position
    {
        public float X { get; set; }
        public float Y { get; set; }

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