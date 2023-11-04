using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.LayoutTests.TestEngine;

internal class MockCanvas : ICanvas
{
    private SKPictureRecorder PictureRecorder { get; }
    private SKCanvas Canvas { get; }
    
    public MockCanvas()
    {
        
    }
    
    public void Translate(Position vector)
    {
        
    }

    public void DrawRectangle(Position vector, Size size, string color)
    {
        
    }

    public void DrawText(SKTextBlob skTextBlob, Position position, TextStyle style)
    {
        
    }

    public void DrawImage(SKImage image, Position position, Size size)
    {
        
    }

    public void DrawHyperlink(string url, Size size)
    {
        
    }

    public void DrawSectionLink(string sectionName, Size size)
    {
        
    }

    public void DrawSection(string sectionName)
    {
        
    }

    public void Rotate(float angle)
    {
        
    }

    public void Scale(float scaleX, float scaleY)
    {
        
    }
}