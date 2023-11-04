using System;

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
        
        public static bool Equal(Position first, Position second)
        {
            if (Math.Abs(first.X - second.X) > Size.Epsilon)
                return false;
            
            if (Math.Abs(first.Y - second.Y) > Size.Epsilon)
                return false;

            return true;
        }
        
        public override string ToString() => $"(Left: {X:N3}, Top: {Y:N3})";
    }
}