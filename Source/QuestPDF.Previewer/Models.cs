using SkiaSharp;

namespace QuestPDF.Previewer;

class DocumentStructure
{
    public bool DocumentContentHasLayoutOverflowIssues { get; set; }
    public ICollection<PageSize> Pages { get; set; }
    
    public class PageSize
    {
        public float Width { get; set; }
        public float Height { get; set; }
    }
}

class PageSnapshotIndex
{
    public int PageIndex { get; set; }
    public int ZoomLevel { get; set; }

    public override string ToString() => $"{ZoomLevel}/{PageIndex}";
}

class RenderedPageSnapshot : PageSnapshotIndex
{
    public SKImage Image { get; set; }
}