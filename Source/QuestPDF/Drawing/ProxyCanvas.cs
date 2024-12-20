using QuestPDF.Infrastructure;
using QuestPDF.Skia;
using QuestPDF.Skia.Text;

namespace QuestPDF.Drawing;

internal class ProxyCanvas : ICanvas
{
    public ICanvas Target { get; set; }

    public void Save()
    {
        Target.Save();
    }

    public void Restore()
    {
        Target.Restore();
    }

    public void Translate(Position vector)
    {
        Target.Translate(vector);
    }

    public void DrawFilledRectangle(Position vector, Size size, Color color)
    {
        Target.DrawFilledRectangle(vector, size, color);
    }

    public void DrawStrokeRectangle(Position vector, Size size, float strokeWidth, Color color)
    {
        Target.DrawStrokeRectangle(vector, size, strokeWidth, color);
    }

    public void DrawParagraph(SkParagraph paragraph)
    {
        Target.DrawParagraph(paragraph);
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

    public void Rotate(float angle)
    {
        Target.Rotate(angle);
    }

    public void Scale(float scaleX, float scaleY)
    {
        Target.Scale(scaleX, scaleY);
    }
}