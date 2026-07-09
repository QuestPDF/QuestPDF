using System.Runtime.InteropServices;

namespace QuestPDF.Skia;

[StructLayout(LayoutKind.Sequential)]
internal struct SkPoint(float x, float y)
{
    public float X = x;
    public float Y = y;
}