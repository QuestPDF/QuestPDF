using System;

namespace QuestPDF.Infrastructure
{
    public enum HorizontalAlignment
    {
        Left,
        Center,
        Right
    }

    internal static class HorizontalAlignmentExtensions
    {
        public static TextAlignment ToTextAlignment(this HorizontalAlignment alignment)
        {
            return alignment switch
            {
                HorizontalAlignment.Left => TextAlignment.Left,
                HorizontalAlignment.Center => TextAlignment.Center,
                HorizontalAlignment.Right => TextAlignment.Right,
                _ => throw new ArgumentOutOfRangeException(nameof(alignment)),
            };
        }
    }
}