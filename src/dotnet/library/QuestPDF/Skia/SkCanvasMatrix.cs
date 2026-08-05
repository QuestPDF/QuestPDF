using System;
using System.Numerics;
using System.Runtime.InteropServices;

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
    
    /// <summary>
    /// Returns the axis-aligned bounding box of the (0, 0, width, height) rectangle after applying this matrix.
    /// </summary>
    public SkRect GetTransformedBoundingBox(float width, float height)
    {
        var matrix = ToMatrix4x4();

        var topLeft = Vector2.Transform(new Vector2(0, 0), matrix);
        var topRight = Vector2.Transform(new Vector2(width, 0), matrix);
        var bottomLeft = Vector2.Transform(new Vector2(0, height), matrix);
        var bottomRight = Vector2.Transform(new Vector2(width, height), matrix);

        var min = Vector2.Min(Vector2.Min(topLeft, topRight), Vector2.Min(bottomLeft, bottomRight));
        var max = Vector2.Max(Vector2.Max(topLeft, topRight), Vector2.Max(bottomLeft, bottomRight));

        return new SkRect(min.X, min.Y, max.X, max.Y);
    }
}