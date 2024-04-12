using System.Runtime.InteropServices;

namespace QuestPDF.Skia;

[StructLayout(LayoutKind.Sequential)]
internal struct SkSize
{
    public float Width;
    public float Height;
    
    public SkSize(float width, float height)
    {
        Width = width;
        Height = height;
    }
}