using System;
using System.Collections.Generic;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Elements;

internal class LayoutOverflowPageMarker : ContainerElement
{
    public HashSet<int> PageNumbersWithLayoutIssues { get; set; } = new();
    
    private const string LineColor = Colors.Red.Medium;
    private const byte LineOpacity = 64;
    private const float BorderThickness = 24f;

    internal override void Draw(Size availableSpace)
    {
        Child?.Draw(availableSpace);
        
        if (!PageNumbersWithLayoutIssues.Contains(PageContext.CurrentPage))
            return;

        DrawPageIndication(availableSpace);
    }

    private void DrawPageIndication(Size availableSpace)
    {
        if (Canvas is not SkiaCanvasBase canvasBase)
            return;

        using var indicatorPaint = new SKPaint
        {
            StrokeWidth = BorderThickness * 2, // half of the width will be outside of the page area
            Color = SKColor.Parse(LineColor).WithAlpha(LineOpacity),
            IsStroke = true
        };
        
        var skiaCanvas = canvasBase.Canvas;
        skiaCanvas.DrawRect(0, 0, availableSpace.Width, availableSpace.Height, indicatorPaint);
    }
}