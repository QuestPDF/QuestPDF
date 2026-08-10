using System;
using System.Numerics;
using System.Runtime.InteropServices;
using QuestPDF.Infrastructure;

namespace QuestPDF.Skia;

[StructLayout(LayoutKind.Sequential)]
internal struct SkCanvasMatrix
{
    public float ScaleX;
    public float SkewX;
    public float TranslateX;
    
    public float SkewY;
    public float ScaleY;
    public float TranslateY;

    public float Perspective1;
    public float Perspective2;
    public float Perspective3;

    public static readonly SkCanvasMatrix Identity = FromMatrix4x4(Matrix4x4.Identity);

    public Matrix4x4 ToMatrix4x4()
    {
        return new Matrix4x4(
            ScaleX, SkewY, 0, 0,
            SkewX, ScaleY, 0, 0,
            0, 0, Perspective3, 0,
            TranslateX, TranslateY, 0, 1);
    }
    
    /// <summary>
    /// Maps a rectangle of the given size, anchored at the local origin, to the page coordinate space.
    /// The result is the axis-aligned rectangle enclosing the mapped corners,
    /// as the content may be translated, scaled, skewed or rotated by its parent elements.
    /// </summary>
    public SkRect GetTransformedBoundingBox(Size size)
    {
        var matrix = ToMatrix4x4();

        var topLeft = Vector2.Transform(new Vector2(0, 0), matrix);
        var topRight = Vector2.Transform(new Vector2(size.Width, 0), matrix);
        var bottomRight = Vector2.Transform(new Vector2(size.Width, size.Height), matrix);
        var bottomLeft = Vector2.Transform(new Vector2(0, size.Height), matrix);

        return new SkRect(
            left: Min4(topLeft.X, topRight.X, bottomRight.X, bottomLeft.X),
            top: Min4(topLeft.Y, topRight.Y, bottomRight.Y, bottomLeft.Y),
            right: Max4(topLeft.X, topRight.X, bottomRight.X, bottomLeft.X),
            bottom: Max4(topLeft.Y, topRight.Y, bottomRight.Y, bottomLeft.Y));

        static float Min4(float a, float b, float c, float d) => Math.Min(Math.Min(a, b), Math.Min(c, d));
        static float Max4(float a, float b, float c, float d) => Math.Max(Math.Max(a, b), Math.Max(c, d));
    }

    public static SkCanvasMatrix FromMatrix4x4(Matrix4x4 matrix)
    {
        return new SkCanvasMatrix
        {
            ScaleX = matrix.M11,
            SkewX = matrix.M21,
            TranslateX = matrix.M41,
            
            SkewY = matrix.M12,
            ScaleY = matrix.M22,
            TranslateY = matrix.M42,
            
            Perspective1 = 0,
            Perspective2 = 0,
            Perspective3 = 1
        };
    }
}