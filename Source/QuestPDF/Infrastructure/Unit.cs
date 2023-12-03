using System;
using static QuestPDF.Infrastructure.Unit;

namespace QuestPDF.Infrastructure
{
    public enum Unit
    {
        /// <summary>
        /// Point is a standard unit for all PDF documents.
        /// 72 points equal 1 inch
        /// </summary>
        Point,
            
        Meter,
            
        Centimetre,
        Millimetre,
        
        Feet,
        Inch,
        
        /// <summary>
        /// 1/1000th of inch
        /// </summary>
        Mil
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
                    Mil => InchToPoints / 1000f,
                    _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, null)
                };
            }
        }
    }
}