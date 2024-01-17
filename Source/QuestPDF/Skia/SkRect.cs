using System.Runtime.InteropServices;

namespace NativeSkia;

[StructLayout(LayoutKind.Sequential)]
internal struct SkRect
{
    public float Left;
    public float Top;
    public float Right;
    public float Bottom;
    
    public SkRect(float left, float top, float right, float bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }
}