using System.Runtime.InteropServices;

namespace QuestPDF.Skia;

[StructLayout(LayoutKind.Sequential)]
internal class SkSize
{
    public float Width;
    public float Height;
    
    public SkSize()
    {
        
    }
    
    public SkSize(float width, float height)
    {
        Width = width;
        Height = height;
    }
}