using System.Runtime.InteropServices;

namespace QuestPDF.Skia;

[StructLayout(LayoutKind.Sequential)]
internal struct SkBoxShadow
{
    public float OffsetX;
    public float OffsetY;
    public float Blur;
    public uint Color;
}