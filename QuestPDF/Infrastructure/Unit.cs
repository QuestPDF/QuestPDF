using System;
using static QuestPDF.Infrastructure.Unit;

namespace QuestPDF.Infrastructure
{
    public enum Unit
    {
        Point,
        
        Meter,
        Centimetre,
        Millimetre,
        
        Feet,
        Inch,
        Mill
    }

    internal static class UnitExtensions
    {
        private const float InchToCentimetre = 2.54f;
        private const float InchToPoints = 72;
        
        public static float ToPoints(this float value, Unit unit)
        {
            return value * GetConversionFactor();
            
            float GetConversionFactor()
            {
                return unit switch
                {
                    Point => 1,
                    Meter => 100 / InchToCentimetre * InchToPoints,
                    Centimetre => 1 / InchToCentimetre * InchToPoints,
                    Millimetre => 0.1f / InchToCentimetre * InchToPoints,
                    Feet => 12 * InchToPoints,
                    Inch => InchToPoints,
                    Mill => InchToPoints / 1000f,
                    _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, null)
                };
            }
        }
    }
}