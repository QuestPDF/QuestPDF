using System;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;
using QuestPDF.Skia.Text;

namespace QuestPDF.Drawing.DrawingCanvases;

internal sealed class ProxyDrawingCanvas : IDrawingCanvas, IDisposable
{
    public IDrawingCanvas Target { get; set; }

    #region IDisposable
    
    ~ProxyDrawingCanvas()
    {
        this.WarnThatFinalizerIsReached();
        Dispose();
    }
    
    public void Dispose()
    {
        (Target as IDisposable)?.Dispose();
        GC.SuppressFinalize(this);
    }
    
    #endregion
    
    #region IDrawingCanvas
    
    public DocumentPageSnapshot GetSnapshot()
    {
        return Target.GetSnapshot();
    }

    public void DrawSnapshot(DocumentPageSnapshot snapshot)
    {
        Target.DrawSnapshot(snapshot);
    }

    public void Save()
    {
        Target.Save();
    }

    public void Restore()
    {
        Target.Restore();
    }

    public void SetZIndex(int index)
    {
        Target.SetZIndex(index);
    }

    public int GetZIndex()
    {
        return Target.GetZIndex();
    }
    
    public SkCanvasMatrix GetCurrentMatrix()
    {
        return Target.GetCurrentMatrix();
    }

    public void SetMatrix(SkCanvasMatrix matrix)
    {
        Target.SetMatrix(matrix);
    }
    
    public void Translate(Position vector)
    {
        Target.Translate(vector);
    }
    
    public void Scale(float scaleX, float scaleY)
    {
        Target.Scale(scaleX, scaleY);
    }
    
    public void Rotate(float angle)
    {
        Target.Rotate(angle);
    }

    public void DrawFilledRectangle(Position vector, Size size, Color color)
    {
        Target.DrawFilledRectangle(vector, size, color);
    }

    public void DrawStrokeRectangle(Position vector, Size size, float strokeWidth, Color color)
    {
        Target.DrawStrokeRectangle(vector, size, strokeWidth, color);
    }

    public void DrawParagraph(SkParagraph paragraph, int lineFrom, int lineTo)
    {
        Target.DrawParagraph(paragraph, lineFrom, lineTo);
    }

    public void DrawImage(SkImage image, Size size)
    {
        Target.DrawImage(image, size);
    }

    public void DrawPicture(SkPicture picture)
    {
        Target.DrawPicture(picture);
    }

    public void DrawSvgPath(string path, Color color)
    {
        Target.DrawSvgPath(path, color);
    }

    public void DrawSvg(SkSvgImage svgImage, Size size)
    {
        Target.DrawSvg(svgImage, size);
    }

    public void DrawOverflowArea(SkRect area)
    {
        Target.DrawOverflowArea(area);
    }

    public void ClipOverflowArea(SkRect availableSpace, SkRect requiredSpace)
    {
        Target.ClipOverflowArea(availableSpace, requiredSpace);
    }

    public void ClipRectangle(SkRect clipArea)
    {
        Target.ClipRectangle(clipArea);
    }

    public void DrawHyperlink(string url, Size size)
    {
        Target.DrawHyperlink(url, size);
    }

    public void DrawSectionLink(string sectionName, Size size)
    {
        Target.DrawSectionLink(sectionName, size);
    }

    public void DrawSection(string sectionName)
    {
        Target.DrawSection(sectionName);
    }
    
    #endregion
}