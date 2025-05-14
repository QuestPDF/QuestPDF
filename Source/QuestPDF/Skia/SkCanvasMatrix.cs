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
}