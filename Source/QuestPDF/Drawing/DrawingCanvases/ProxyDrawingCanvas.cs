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

    public void DrawLine(Position start, Position end, SkPaint paint)
    {
        Target.DrawLine(start, end, paint);
    }
    
    public void DrawRectangle(Position vector, Size size, SkPaint paint)
    {
        Target.DrawRectangle(vector, size, paint);
    }
    
    public void DrawComplexBorder(SkRoundedRect innerRect, SkRoundedRect outerRect, SkPaint paint)
    {
        Target.DrawComplexBorder(innerRect, outerRect, paint);
    }
    
    public void DrawShadow(SkRoundedRect shadowRect, SkBoxShadow shadow)
    {
        Target.DrawShadow(shadowRect, shadow);
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
    
    public void ClipRoundedRectangle(SkRoundedRect clipArea)
    {
        Target.ClipRoundedRectangle(clipArea);
    }

    public void DrawHyperlink(Size size, string url, string? description)
    {
        Target.DrawHyperlink(size, url, description);
    }

    public void DrawSectionLink(Size size, string sectionName, string? description)
    {
        Target.DrawSectionLink(size, sectionName, description);
    }

    public void DrawSection(string sectionName)
    {
        Target.DrawSection(sectionName);
    }
    
    public void SetSemanticNodeId(int nodeId)
    {
        Target.SetSemanticNodeId(nodeId);
    }
    
    #endregion
}