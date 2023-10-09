using SkiaSharp;

namespace QuestPDF.Previewer;

internal sealed class DocumentSnapshot
{
    public bool DocumentContentHasLayoutOverflowIssues { get; set; }
    public ICollection<PageSnapshot> Pages { get; set; }

    public class PageSnapshot
    {
        public string Id { get; set; }
        
        public float Width { get; set; }
        public float Height { get; set; }
        
        public SKPicture Picture { get; set; }
    }
}
