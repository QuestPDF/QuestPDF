using System;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class Rotate : ContainerElement
    {
        public float Angle { get; set; } = 0;

        internal override void Draw(Size availableSpace)
        {
            Canvas.Rotate(Angle);
            Child?.Draw(availableSpace);
            Canvas.Rotate(-Angle);
        }

        internal override string? GetCompanionHint()
        {
            if (Angle == 0)
                return "No rotation";

            var degrees = Math.Abs(Angle);
            
            // Stryker disable once equality: TurnCount = 0 is handled above
            var direction = Angle > 0 ? "clockwise" : "counter-clockwise"; 
            
            return $"{degrees.FormatAsCompanionNumber()} deg {direction}";
        }
    }
}