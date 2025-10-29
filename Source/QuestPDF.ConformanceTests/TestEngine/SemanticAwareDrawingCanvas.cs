using QuestPDF.Drawing;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;
using QuestPDF.Skia.Text;

namespace QuestPDF.ConformanceTests.TestEngine;

internal class SemanticAwareDocumentCanvas : IDocumentCanvas
{
    internal SemanticTreeNode? SemanticTree { get; private set; }
    private SemanticAwareDrawingCanvas DrawingCanvas { get; } = new();
    
    public void SetSemanticTree(SemanticTreeNode? semanticTree)
    {
        SemanticTree = semanticTree;
    }

    public void BeginDocument()
    {
        
    }

    public void EndDocument()
    {
        
    }

    public void BeginPage(Size size)
    {
        
    }

    public void EndPage()
    {
        
    }

    public IDrawingCanvas GetDrawingCanvas()
    {
        return DrawingCanvas;
    }
}

internal class SemanticAwareDrawingCanvas : IDrawingCanvas
{
    private int CurrentSemanticNodeId { get; set; }
    
    public DocumentPageSnapshot GetSnapshot()
    {
        return new DocumentPageSnapshot();
    }

    public void DrawSnapshot(DocumentPageSnapshot snapshot)
    {
        
    }

    public void Save()
    {
        
    }

    public void Restore()
    {
        
    }

    public void SetZIndex(int index)
    {
        
    }

    public int GetZIndex()
    {
        return 0;
    }

    public SkCanvasMatrix GetCurrentMatrix()
    {
        return SkCanvasMatrix.Identity;
    }

    public void SetMatrix(SkCanvasMatrix matrix)
    {
        
    }

    public void Translate(Position vector)
    {
        
    }

    public void Scale(float scaleX, float scaleY)
    {
        
    }

    public void Rotate(float angle)
    {
        
    }

    public void DrawLine(Position start, Position end, SkPaint paint)
    {
        if (CurrentSemanticNodeId != SkSemanticNodeSpecialId.LayoutArtifact)
            Assert.Fail("Detected a line drawing operation outside of layout artifact");
    }

    public void DrawRectangle(Position vector, Size size, SkPaint paint)
    {
        if (CurrentSemanticNodeId is not (SkSemanticNodeSpecialId.BackgroundArtifact or SkSemanticNodeSpecialId.LayoutArtifact))
            Assert.Fail("Detected a rectangle drawing operation outside of layout artifact");
    }

    public void DrawComplexBorder(SkRoundedRect innerRect, SkRoundedRect outerRect, SkPaint paint)
    {
        if (CurrentSemanticNodeId != SkSemanticNodeSpecialId.LayoutArtifact)
            Assert.Fail("Detected a complex-border drawing operation outside of layout artifact");
    }

    public void DrawShadow(SkRoundedRect shadowRect, SkBoxShadow shadow)
    {
        if (CurrentSemanticNodeId != SkSemanticNodeSpecialId.BackgroundArtifact)
            Assert.Fail("Detected a shadow drawing operation outside of background artifact");
    }

    public void DrawParagraph(SkParagraph paragraph, int lineFrom, int lineTo)
    {
        
    }

    public void DrawImage(SkImage image, Size size)
    {
        
    }

    public void DrawPicture(SkPicture picture)
    {
        
    }

    public void DrawSvgPath(string path, Color color)
    {
        
    }

    public void DrawSvg(SkSvgImage svgImage, Size size)
    {
        
    }

    public void DrawOverflowArea(SkRect area)
    {
        
    }

    public void ClipOverflowArea(SkRect availableSpace, SkRect requiredSpace)
    {
        
    }

    public void ClipRectangle(SkRect clipArea)
    {
        
    }

    public void ClipRoundedRectangle(SkRoundedRect clipArea)
    {
        
    }

    public void DrawHyperlink(Size size, string url, string? description)
    {
        
    }

    public void DrawSectionLink(Size size, string sectionName, string? description)
    {
        
    }

    public void DrawSection(string sectionName)
    {
        
    }

    public void SetSemanticNodeId(int nodeId)
    {
        CurrentSemanticNodeId = nodeId;
    }
}