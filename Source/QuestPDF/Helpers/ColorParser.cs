using System.Collections.Concurrent;
using QuestPDF.Skia;

namespace QuestPDF.Helpers;

internal static class ColorParser
{
    private static readonly ConcurrentDictionary<string, uint> ColorCodes = new();
    
    internal static uint ColorToCode(this string color)
    {
        return ColorCodes.GetOrAdd(color, SkColor.Parse);
    }
}