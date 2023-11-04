using QuestPDF.Infrastructure;

namespace QuestPDF.LayoutTests.TestEngine;

internal class BoundingBox
{
    public double MinX { get; init; }
    public double MinY { get; init; }
    public double MaxX { get; init; }
    public double MaxY { get; init; }
    
    public double Width => MaxX - MinX;
    public double Height => MaxY - MinY;

    public static BoundingBox From(Position position, Size size)
    {
        return new BoundingBox
        {
            MinX = position.X,
            MinY = position.Y,
            MaxX = position.X + size.Width,
            MaxY = position.Y + size.Height
        };
    }
}

internal static class BoundingBoxExtensions
{
    public static BoundingBox? Intersection(BoundingBox first, BoundingBox second)
    {
        var common = new BoundingBox
        {
            MinX = Math.Max(first.MinX, second.MinX),
            MinY = Math.Max(first.MinY, second.MinY),
            MaxX = Math.Min(first.MaxX, second.MaxX),
            MaxY = Math.Min(first.MaxY, second.MaxY),
        };

        if (common.Width < 0 || common.Height < 0)
            return null;

        return common;
    }
}