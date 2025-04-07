using System;
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

    public static SkCanvasMatrix Identity => new()
    {
        ScaleX = 1,
        SkewX = 0,
        TranslateX = 0,
        
        SkewY = 0,
        ScaleY = 1,
        TranslateY = 0,
        
        Perspective1 = 0,
        Perspective2 = 0,
        Perspective3 = 1
    };
    
    public SkCanvasMatrix Translate(float x, float y)
    {
        return this with { TranslateX = TranslateX + x, TranslateY = TranslateY + y };
    }  
    
    public SkCanvasMatrix Scale(float x, float y)
    {
        return this with { ScaleX = ScaleX * x, ScaleY = ScaleY * y };
    }
    
    public SkCanvasMatrix Rotate(float angle)
    {
        var radians = Math.PI * angle / 180;
        var cos = (float)Math.Cos(radians);
        var sin = (float)Math.Sin(radians);
        
        return this with
        {
            ScaleX = ScaleX * cos - SkewY * sin,
            SkewX = ScaleX * sin + SkewY * cos,
            ScaleY = ScaleY * cos - SkewX * sin,
            SkewY = ScaleY * sin + SkewX * cos
        };
    }
}